using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Skills;
using TheIdleScrolls_Core.Skills.SkillEffects;
using static System.Net.Mime.MediaTypeNames;

namespace TheIdleScrolls_Core.Systems
{
    public class BattleSystem : AbstractSystem
    {
        int _SkipFrames = 2;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            // Don't start battles immediately after loading the game to give time to update stats
            if (_SkipFrames > 0)
            {
                _SkipFrames--;
                return;
            }

            // Remove previously defeated mobs from coordinator
            coordinator.GetEntities<MobComponent, KilledComponent>()
                .Select(e => e.Id).ToList()
                .ForEach(id => coordinator.RemoveEntity(id));

            // Cancel battles if player moved to different area
            if (coordinator.MessageTypeIsOnBoard<AreaChangedMessage>())
            {
                // CornerCut: for multiple players, we would have to check which player moved
                coordinator.GetEntities<PlayerComponent, BattlerComponent>()
                    .ForEach(e =>
                    {
                        var battle = e.GetComponent<BattlerComponent>()!.Battle;
                        battle.State = Battle.BattleState.Cancelled;
                        coordinator.PostMessage(this, new BattleStateChangedMessage(battle));
                    });
            }

            // Clean up previously finished and cancelled battles
            foreach (var battler in coordinator.GetEntities<BattlerComponent>())
            {
                var battle = battler.GetComponent<BattlerComponent>()!.Battle;

                if (battle.IsFinished)
                {
                    battler.RemoveComponent<BattlerComponent>();
                    if (battler.IsMob()) // Despawn mobs from finished battles
                    {
                        coordinator.RemoveEntity(battler.Id);
                    }
                }
            }

            // Process active battles
            HashSet<Battle> battles = coordinator.GetEntities<BattlerComponent>()
                                        .Select(e => e.GetComponent<BattlerComponent>()!.Battle)
                                        .ToHashSet();
            
            foreach (var battle in battles)
            {
                Entity player = battle.Player;

                // Mob was spawned and the fight can begin
                if ((battle.State == Battle.BattleState.Initialized || battle.State == Battle.BattleState.BetweenFights) 
                    && battle.Mob != null)
                {
                    battle.State = Battle.BattleState.InProgress;
                    coordinator.PostMessage(this, new BattleStateChangedMessage(battle));
                }
                
                // All battles that exist at this point should be in progress
                if (battle.State != Battle.BattleState.InProgress)
                {
                    throw new Exception("battle is not in progress");
                }

                battle.Duration += dt;
                Entity mob = battle.Mob!; // has to be present if battle is in progress

                // Player may have been defeated through a status effect (e.g. poison). In that case, don't process skills, but
                // let the player win the fight if the mob was defeated during the same frame (also from a status effect).
                bool playerDefeated = player.GetComponent<TimeShieldComponent>()?.IsDepleted ?? false;

                if (!playerDefeated)
                {
                    ProcessSkills(player, mob, dt, coordinator);
                }

                bool mobDefeated = mob.GetComponent<LifePoolComponent>()?.IsDead 
                    ?? throw new Exception($"Mob {mob.GetName()} has no life pool");

                if (mobDefeated)
                {
                    coordinator.PostMessage(this, new DeathMessage(mob));
                    mob.AddComponent(new KilledComponent { Killer = player.Id });
                }

                // Process mob skills and time loss if mob has not been defeated
                if (!mobDefeated)
                {
                    ProcessSkills(mob, player, dt, coordinator);

                    // Apply time loss
                    double damage = mob.GetComponent<MobDamageComponent>()?.Multiplier ?? 0.0;
                    double armor = player.GetComponent<DefenseComponent>()?.Armor ?? 0.0;
                    double armorBonus = Functions.CalculateArmorBonusMultiplier(armor, mob.GetLevel(), damage);
                    double timeLoss = dt * damage / armorBonus;

                    timeLoss = player.GetComponent<ModifierComponent>()
                        ?.ApplyApplicableModifiers(timeLoss, [Tags.TimeLoss], player.GetTags())
                        ?? timeLoss;
                    mob.GetComponent<BattlerComponent>()!.DamageDealt += timeLoss;

                    var shieldComp = player.GetComponent<TimeShieldComponent>();
                    if (shieldComp != null) // Players without time shield are invincible
                    {
                        shieldComp.Drain(timeLoss);
                        playerDefeated = shieldComp.IsDepleted;
                    }
                }

                // Update battle state
                if (mobDefeated)
                {
                    battle.State = (battle.MobsRemaining == 0) 
                        ? Battle.BattleState.PlayerWon 
                        : Battle.BattleState.BetweenFights;
                    coordinator.PostMessage(this, new BattleStateChangedMessage(battle));
                    player.GetComponent<BattlerComponent>()!.AttacksPerformed = 0; // Reset attack counter to enable FirstStrike for next mob
                }
                else if (playerDefeated)
                {
                    battle.State = Battle.BattleState.PlayerLost;
                    coordinator.PostMessage(this, new BattleLostMessage(player, mob.GetName(), mob.GetLevel()));
                    coordinator.PostMessage(this, new BattleStateChangedMessage(battle));
                }                
            }

            // Create new battles for player
            foreach (var player in coordinator.GetEntities<PlayerComponent>())
            {
                if (player.HasComponent<BattlerComponent>())
                    continue; // Player is already in a battle

                LocationComponent locationComp = player.GetComponent<LocationComponent>() 
                    ?? throw new Exception("Players lacks location component");
                ZoneDescription zone = locationComp.GetCurrentZone(world.Map) 
                    ?? throw new Exception($"{player.GetName()} is not in a valid zone");

                Battle battle = new(player, zone.MobCount);
                player.AddComponent(new BattlerComponent(battle));
                coordinator.PostMessage(this, new BattleStateChangedMessage(battle));

                SetupPlayerTimeShield(player, zone);
                player.GetComponent<TimeShieldComponent>()?.Refill();
                player.GetComponent<AttackComponent>()?.Cooldown?.Reset();
                player.GetComponent<ActiveSkillComponent>()?.ResetSkills();
            }
        }

        static void SetupPlayerTimeShield(Entity player, ZoneDescription zone)
        {
            if (player.GetComponent<BattlerComponent>()?.Battle?.CustomTimeLimit ?? false)
                return; // Skip time shield setup if custom time limit is in use (i.e. final boss)

            double baseDuration = Functions.CalculateBaseTimeLimit(player.GetLevel(), zone.Level);
            double duration = zone.TimeMultiplier * player.ApplyAllApplicableModifiers(baseDuration, [Tags.TimeShield], player.GetTags());
            player.GetComponent<TimeShieldComponent>()?.Rescale(duration);
        }

        static DamageDoneMessage? ApplyAttack(Entity attacker, Entity target)
        {
            var attackComp = attacker.GetComponent<AttackComponent>() ?? throw new Exception("Missing attack component");
            LifePoolComponent? hpComp = target.GetComponent<LifePoolComponent>();
            if (hpComp == null)
                return null;
            if (hpComp.Current == 0)
                return null; // Skip damage if target is already dead
            int damage = (int)Math.Round(attackComp.RawDamage, 0);
            hpComp.ApplyDamage(damage);
            return new DamageDoneMessage(attacker, target, damage);
        }

        void ProcessSkills(Entity entity, Entity opponent, double dt, Coordinator coordinator)
        {
			// Process player skills
			var skillComp = entity.GetComponent<ActiveSkillComponent>();
			if (skillComp is null)
                return;

			double totalElapsed = 0.0;
			while (totalElapsed < dt)
			{
				double previouslyRemaining = dt - totalElapsed;
				if (skillComp.CurrentSkill is not null
					&& skillComp.CurrentSkill.CurrentState == SkillTimer.State.NotStarted)
				{
					skillComp.CurrentSkill.Timer.Start();
				}
				double remaining = skillComp.CurrentSkill?.UpdateTimer(previouslyRemaining) ?? 0.0;
				double elapsed = previouslyRemaining - remaining;
				totalElapsed += elapsed;
				// Also update timer for all other skills
				foreach (var skill in skillComp.Skills)
				{
					if (skill != skillComp.CurrentSkill)
						skill.UpdateTimer(elapsed);
				}
				if (remaining > 0.0) // Means that the skill has finished charging
				{
					double damage = 0;
					foreach (var effect in skillComp.CurrentSkill!.Effects)
					{
						if (effect.Target == ISkillEffect.TargetingMode.SingleEnemy)
						{
							effect.ApplyToTarget(opponent);
							if (effect is DamageSkillEffect dmgEffect)
							{
								damage += dmgEffect.Damage;
								coordinator.PostMessage(this, new DamageDoneMessage(entity, opponent, (int)damage));
							}
						}
						else
						{
							effect.ApplyToTarget(entity);
						}
					}
					entity.GetComponent<BattlerComponent>()!.DamageDealt += (int)damage;
					entity.GetComponent<BattlerComponent>()!.AttacksPerformed++;
					skillComp.SwitchToNext();
					skillComp.CurrentSkill.Timer.Start();
				}
			}
		}
    }

    public record BattleStateChangedMessage(Battle Battle) : IMessage
    {
        string IMessage.BuildMessage() => $"State changed for battle of {Battle.Player.GetName()}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Medium;
    }

    public record DamageDoneMessage(Entity Attacker, Entity Target, int Damage) : IMessage
    {
        string IMessage.BuildMessage()
        {
            string attackerName = Attacker.GetName();
            string targetName = Target.GetName();
            int remainingHP = Target.GetComponent<LifePoolComponent>()?.Current ?? 0;
            int fullHP = Target.GetComponent<LifePoolComponent>()?.Maximum ?? 0;
            return $"{attackerName} did {Damage} damage to {targetName} ({remainingHP}/{fullHP} HP remaining)";
        }

        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.VeryLow;
    }

    public record DeathMessage(Entity Victim) : IMessage
    {
        string IMessage.BuildMessage() => $"{Victim.GetName()} was defeated";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Medium;
    }

    public record BattleLostMessage(Entity Player, string MobName, int Level) : IMessage
    {
        string IMessage.BuildMessage() => $"{Player.GetName()} lost the fight against {MobName} (Level {Level})";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.High;
    }
}
