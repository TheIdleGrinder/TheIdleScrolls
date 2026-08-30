using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Skills;
using TheIdleScrolls_Core.Skills.SkillEffects;
using TheIdleScrolls_Core.StatusEffects;

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

            foreach (var restThresholdRequest in coordinator.FetchMessagesByType<SetRestingHpThresholdRequest>())
            {
                var player = coordinator.GetEntity(restThresholdRequest.Player);
                if (player is not null)
                {
                    var adventureComp = player.GetComponent<AdventurerComponent>();
                    if (adventureComp is not null)
                    {
                        adventureComp.RestHpThreshold = restThresholdRequest.Threshold;
                    }
                }
            }

            // Remove previously defeated mobs from battle and coordinator
            var defeatedMobs = coordinator.GetEntities<MobComponent, KilledComponent>();
            foreach (var mob in defeatedMobs)
            {
                var battleComp = mob.GetComponent<BattlerComponent>();
                if (battleComp is not null)
                {
                    var battle = battleComp.Battle;
                    battle.Mobs.Remove(mob);
                    coordinator.RemoveEntity(mob.Id);
                }
            }

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
                    battler.RemoveComponent<KilledComponent>(); // Can only happen for player characters
                    battler.GetComponent<AdventurerComponent>()?.SetState(AdventurerState.Idle);
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
                    && battle.Mobs.Count > 0)
                {
                    PositionMobs(battle);

                    battle.State = Battle.BattleState.InProgress;
                    coordinator.PostMessage(this, new BattleStateChangedMessage(battle));
                }
                
                // All battles that exist at this point should be in progress
                if (battle.State != Battle.BattleState.InProgress)
                {
                    throw new Exception("battle is not in progress");
                }

                battle.Duration += dt;

                // Player may have been defeated through a status effect (e.g. poison). In that case, don't process skills, but
                // let the player win the fight if the mob was defeated during the same frame (also from a status effect).
                //bool playerDefeated = player.IsDefeated();

                //if (!playerDefeated)
                //{
                //    ProcessSkills(player, dt, coordinator);
                //}

                //foreach (var mob in battle.Mobs)
                //{
                //    if (mob.IsDefeated())
                //    {
                        
                //    }
                //    else
                //    {
                //        ProcessSkills(mob, dt, coordinator);
                //        var hpComp = player.GetComponent<LifePoolComponent>();
                //        // Players without HP are invincible
                //        playerDefeated = hpComp?.IsDead ?? false;
                //        if (playerDefeated)
                //        {
                //            break; // No need to process further mobs if player is already defeated
                //        }
                //    }
                //}

                // Update battle state
                if (battle.Mobs.All(mob => mob.IsDefeated()))
                {
                    battle.State = (battle.MobsRemaining == 0) 
                        ? Battle.BattleState.PlayerWon 
                        : Battle.BattleState.BetweenFights;
                    coordinator.PostMessage(this, new BattleStateChangedMessage(battle));
                    player.GetComponent<BattlerComponent>()!.SkillsUsed = 0; // Reset attack counter to enable FirstStrike for next mob
                }
                else if (battle.Player.IsDefeated())
                {
                    battle.State = Battle.BattleState.PlayerLost;
                    coordinator.PostMessage(this, new BattleLostMessage(player, 
                                                    battle.Mobs.FirstOrDefault()?.GetName() ?? "??", 
                                                    battle.Mobs.FirstOrDefault()?.GetLevel() ?? 0));
                    coordinator.PostMessage(this, new BattleStateChangedMessage(battle));
                }                
            }

            // Create new battles for player
            foreach (var player in coordinator.GetEntities<PlayerComponent>())
            {
                if (player.HasComponent<BattlerComponent>())
                    continue; // Player is already in a battle

                var adventureComp = player.GetComponent<AdventurerComponent>();
                if (adventureComp is null)
                {
                    continue; // Player does not have an AdventurerComponent
                }
                AdventurerState state = adventureComp.State;


                if (state == AdventurerState.Idle)
                {
                    var hpComp = player.GetComponent<LifePoolComponent>();
                    double hpRatio = (1.0 * hpComp?.Current / hpComp?.Maximum) ?? 1.0;
                    if (hpRatio > adventureComp.RestHpThreshold)
                    {
                        LocationComponent locationComp = player.GetComponent<LocationComponent>()
                            ?? throw new Exception("Players lacks location component");
                        ZoneDescription zone = locationComp.GetCurrentZone(world.Map)
                            ?? throw new Exception($"{player.GetName()} is not in a valid zone");

                        Battle battle = new(player, zone.MobCount);
                        player.AddComponent(new BattlerComponent(battle));
                        adventureComp.SetState(AdventurerState.Fighting);
                        coordinator.PostMessage(this, new BattleStateChangedMessage(battle));

                        player.GetComponent<ActiveSkillComponent>()?.ResetSkills();
                        player.GetComponent<StatusEffectComponent>()?.DeactivateAll();
                    }
                    else
                    {
                        adventureComp.SetState(AdventurerState.Resting);
                        var restEffect = new RestingStatusEffect();
                        restEffect.ActivateOnEntity(player);
                    }
                }
                else if (state == AdventurerState.Resting && (player.GetComponent<LifePoolComponent>()?.IsFull ?? true))
                {
                    adventureComp.SetState(AdventurerState.Idle);
                    var effects = player.GetComponent<StatusEffectComponent>()?.StatusEffects ?? [];
                    var effect = effects.FirstOrDefault(e => e is RestingStatusEffect);
                    if (effect is not null)
                        effect.Deactivate();
                }
            }
        }

        private static void PositionMobs(Battle battle)
        {
            foreach (var mob in battle.Mobs)
            {
                var battleComp = mob.GetComponent<BattlerComponent>();
                if (battleComp is null)
                    continue;
                BattlePosition playerPos = battle.Player.GetComponent<BattlerComponent>()?.Position ?? new BattlePosition(0.0, 0.0);
                // Distance is maximum of individual ranges and base distance
                double distance = new[] { Stats.BattleBaseDistance, battle.Player.GetRange(), mob.GetRange() }.Max();
                battleComp.Position = new BattlePosition(playerPos.X + distance, playerPos.Y);
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
    }

    public record BattleStateChangedMessage(Battle Battle) : IMessage
    {
        string IMessage.BuildMessage() => $"State changed for battle of {Battle.Player.GetName()}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Medium;
    }

    public record SkillStateChangedMessage(Entity User, ActiveSkill Skill, SkillTimer.TimerUpdateResult Changes) : IMessage
    {
        string IMessage.BuildMessage() => $"{User.GetName()}'s skill {Skill.Name} changed state to {Skill.GetState()}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Low;
    }

    public record DamageDoneMessage(Entity Attacker, Entity Target, int Damage, int DamagePrevented = 0) : IMessage
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

    public record HitBlockedMessage(Entity Attacker, Entity Target, double PreventionPercentage) : IMessage
    {
        string IMessage.BuildMessage() => $"{Target.GetName()} blocked {PreventionPercentage:0.##%} incoming damage";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.VeryHigh;
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

    public record SetRestingHpThresholdRequest(uint Player, double Threshold) : IMessage
    {
        string IMessage.BuildMessage() => $"Set resting HP threshold to {Threshold} for player with ID {Player}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }
}
