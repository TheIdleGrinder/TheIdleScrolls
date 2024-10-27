using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Systems
{
    public class BattleSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            // Remove previously defeated mobs from coordinator
            coordinator.GetEntities<MobComponent, KilledComponent>()
                .Select(e => e.Id).ToList()
                .ForEach(id => coordinator.RemoveEntity(id));

            // Cancel battles if player moved to different area
            if (coordinator.MessageTypeIsOnBoard<AreaChangedMessage>())
            {
                // CornerCut: for multiple players, we would have to check which player moved
                coordinator.GetEntities<PlayerComponent, BattlerComponent>()
                    .ForEach(e => e.GetComponent<BattlerComponent>()!.Battle.State = Battle.BattleState.Cancelled);
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
                if (battle.State == Battle.BattleState.WaitingForMob && battle.Mob != null)
                {
                    SetupPlayerTimeShield(player, battle.Mob!);
                    player.GetComponent<TimeShieldComponent>()?.Refill();
                    player.GetComponent<AttackComponent>()?.Cooldown?.Reset();
                    battle.State = Battle.BattleState.InProgress;
                }
                
                // All battles that exist at this point should be in progress
                if (battle.State != Battle.BattleState.InProgress)
                {
                    //continue;
                    throw new Exception("battle is not in progress");
                }

                Entity mob = battle.Mob!; // has to be present if battle is in progress

                // Process player attacks
                var attackComp = player.GetComponent<AttackComponent>() 
                    ?? throw new Exception("Player lacks attack component");
                int attacks = attackComp.Cooldown.Update(dt);
                for (int i = 0; i < attacks; i++)
                {
                    var message = ApplyAttack(player, mob);
                    if (message != null)
                        coordinator.PostMessage(this, message);
                    player.GetComponent<BattlerComponent>()!.AttacksPerformed++;
                }

                bool mobDefeated = mob.GetComponent<LifePoolComponent>()?.IsDead 
                    ?? throw new Exception($"Mob {mob.GetName()} has no life pool");

                if (mobDefeated)
                {
                    coordinator.PostMessage(this, new DeathMessage(mob));
                    mob.AddComponent(new KilledComponent { Killer = player.Id });
                }

                bool playerDefeated = false;
                // Process time loss if mob has not been defeated
                if (!mobDefeated)
                {
                    double damage = mob.GetComponent<MobDamageComponent>()?.Multiplier ?? 1.0;
                    double armor = player.GetComponent<DefenseComponent>()?.Armor ?? 0.0;
                    double armorBonus = Functions.CalculateArmorBonusMultiplier(armor, damage);
                    double timeLoss = damage / armorBonus;
                    var shieldComp = player.GetComponent<TimeShieldComponent>();
                    if (shieldComp != null) // Players without time shield are invincible
                    {
                        shieldComp.Drain(timeLoss);
                        playerDefeated = shieldComp.IsDepleted;
                    }
                }

                // Check for changed evasion rating
                // CornerCut: Do this every frame for now, could be optimized
                SetupPlayerTimeShield(player, mob);

                // Update battle state
                if (mobDefeated)
                {
                    battle.State = (battle.MobsRemaining > 0) 
                        ? Battle.BattleState.WaitingForMob 
                        : Battle.BattleState.PlayerWon;
                }
                else if (playerDefeated)
                {
                    battle.State = Battle.BattleState.PlayerLost;
                    coordinator.PostMessage(this, new BattleLostMessage(player, mob.GetName(), mob.GetLevel()));
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
            }
        }

        static void SetupPlayerTimeShield(Entity player, Entity opponent)
        {
            const double BaseDuration = 10.0;
            double duration = BaseDuration * player.GetLevel() / opponent.GetLevel();
            double evasion = player.GetComponent<DefenseComponent>()?.Evasion ?? 0.0;
            double accuracy = opponent.GetComponent<AccuracyComponent>()?.Accuracy ?? 1.0;
            duration *= Functions.CalculateEvasionBonusMultiplier(evasion, accuracy);
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
    }
}
