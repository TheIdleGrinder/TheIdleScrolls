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
    internal class TimeLimitSystem : AbstractSystem
    {
        const double BaseDuration = 10.0;
        const double DifficultyScaling = 1.0;

        Entity? m_player = null;

        bool m_inCombat = false;
        double m_evasionUsed = 0.0;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;

            var defComp = m_player.GetComponent<DefenseComponent>();

            bool previouslyInCombat = m_inCombat;
            m_inCombat = coordinator.GetEntities<MobComponent>().Count > 0;

            var attackValues = coordinator.GetEntities<MobDamageComponent>()
                .Select(m => m.GetComponent<MobDamageComponent>()?.Multiplier ?? 1.0);
            var locationComp = m_player.GetComponent<LocationComponent>() ?? new();
            ZoneDescription? zone = locationComp.GetCurrentZone(world.Map);

            // Check if time limit needs to be reset
            bool mobSpawned = coordinator.MessageTypeIsOnBoard<MobSpawnMessage>();
            bool newDungeonFloor = locationComp.InDungeon && (locationComp.RemainingEnemies == locationComp.GetCurrentZone(world.Map)?.MobCount);
            bool newTimeLimit = mobSpawned && (!locationComp.InDungeon || newDungeonFloor);

            // Recalculate and reset time limit if necessary
            if (newTimeLimit)
            {
                int level = m_player.GetLevel();
                double levelMulti = level / Math.Pow(zone?.Level ?? level, DifficultyScaling);  

                double duration = BaseDuration * levelMulti * (zone?.TimeMultiplier ?? 1.0);
                if (!attackValues.Any())
                    duration = 0.0;
                world.TimeLimit.Reset(duration);
                coordinator.PostMessage(this, new TextMessage($"New time limit: {duration:0.###} s"));
            }

            // Recalculate time limit if evasion rating changed too much (more than 10%)
            // Might be exploitable, but prevents evasion builds from being hurt by the fact system order:
            // 1. mob spawn
            // 2. stats => no evasion, because achievements are not loaded yet
            // 3. achievements
            // 4. time limit => sets time limit duration
            // 5. stats => now evasion is set correctly
            if (defComp != null && (newTimeLimit || (Math.Abs(defComp.Evasion - m_evasionUsed) > m_evasionUsed * 0.5)))
            {
                double evasion = defComp.Evasion;
                double evasionBonus = CalculateEvasionBonusMultiplier(evasion); // Evasion increases amount of time
                if (!newTimeLimit)
                {
                    double previousBonus = CalculateEvasionBonusMultiplier(m_evasionUsed);
                    evasionBonus /= previousBonus; // Can't be 0
                }
                double newDuration = world.TimeLimit.Duration * evasionBonus;
                world.TimeLimit.ChangeDuration(newDuration);
                m_evasionUsed = evasion;
            }

            if (m_inCombat)
            {
                if (attackValues.Any())
                {
                    double armor = defComp?.Armor ?? 0.0;
                    double armorBonus = CalculateArmorBonusMultiplier(armor);

                    var multi = attackValues.Average();
                    world.TimeLimit.Update(multi * dt / armorBonus); // armor 'slows time'

                    if (world.TimeLimit.HasFinished) // Player lost the fight
                    {
                        var mobName = coordinator.GetEntities<MobComponent>().FirstOrDefault()?.GetName() ?? "??";
                        coordinator.PostMessage(this, new BattleLostMessage(m_player, mobName, zone?.Level ?? 1));

                        coordinator.GetEntities<MobComponent>().ForEach(e => coordinator.RemoveEntity(e.Id)); // Despawn all mobs
                        m_inCombat = false;
                    }
                }
            }
        }

        static double CalculateEvasionBonusMultiplier(double evasion)
        {
            return 1.0 + evasion * Definitions.Stats.EvasionBonusPerPoint;
        }

        static double CalculateArmorBonusMultiplier(double armor)
        {
            return 1.0 + armor * Definitions.Stats.ArmorSlowdownPerPoint;
        }
    }

    internal class BattleLostMessage : IMessage
    {
        public Entity Player;

        public string MobName;

        public int Level;

        public BattleLostMessage(Entity player, string mobName, int level)
        {
            Player = player;
            MobName = mobName;
            Level = level;
        }

        string IMessage.BuildMessage()
        {
            return $"{Player.GetName()} lost the fight against {MobName} (Level {Level})";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.High;
        }
    }
}
