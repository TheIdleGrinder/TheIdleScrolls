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
    /// <summary>
    /// Deprecated, time limit is no longer global and handled by the BattleSystem
    /// </summary>
    internal class TimeLimitSystem : AbstractSystem
    {
        const double BaseDuration = 10.0;
        const double DifficultyScaling = 1.0;

        Entity? m_player = null;

        bool m_inCombat = false;
        double m_evasionUsed = 0.0;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            // Everything disables because it is no longer used of campatible
            //m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            //if (m_player == null)
            //    return;

            //var defComp = m_player.GetComponent<DefenseComponent>();

            //bool previouslyInCombat = m_inCombat;
            //m_inCombat = coordinator.GetEntities<MobComponent>().Count > 0;

            //var attackValues = coordinator.GetEntities<MobDamageComponent>()
            //    .Select(m => m.GetComponent<MobDamageComponent>()?.Multiplier ?? 1.0);
            //var accuracyValues = coordinator.GetEntities<AccuracyComponent>()
            //    .Select(a => a.GetComponent<AccuracyComponent>()?.Accuracy ?? 1.0);
            //var locationComp = m_player.GetComponent<LocationComponent>() ?? new();
            //ZoneDescription? zone = locationComp.GetCurrentZone(world.Map);

            //// Check if time limit needs to be reset
            //bool mobSpawned = coordinator.MessageTypeIsOnBoard<MobSpawnMessage>();
            //bool newDungeonFloor = locationComp.InDungeon && (locationComp.RemainingEnemies == locationComp.GetCurrentZone(world.Map)?.MobCount);
            //bool newTimeLimit = mobSpawned && (!locationComp.InDungeon || newDungeonFloor);

            //// Recalculate and reset time limit if necessary
            //if (newTimeLimit)
            //{
            //    int level = m_player.GetLevel();
            //    double levelMulti = level / Math.Pow(zone?.Level ?? level, DifficultyScaling);  

            //    double duration = BaseDuration * levelMulti * (zone?.TimeMultiplier ?? 1.0);
            //    if (!attackValues.Any())
            //        duration = 0.0;
            //    // Disabled, because time limit is no longer global
            //    //world.TimeLimit.Reset(duration);
            //    coordinator.PostMessage(this, new TextMessage($"New time limit: {duration:0.###} s"));
            //}

            //// Recalculate time limit if evasion rating changed too much (more than 10%)
            //// Might be exploitable, but prevents evasion builds from being hurt by the fact system order:
            //// 1. mob spawn
            //// 2. stats => no evasion, because achievements are not loaded yet
            //// 3. achievements
            //// 4. time limit => sets time limit duration
            //// 5. stats => now evasion is set correctly
            //if (defComp != null && (newTimeLimit 
            //    || (Math.Abs(defComp.Evasion - m_evasionUsed) > Math.Min(defComp.Evasion, m_evasionUsed) * 0.5)))
            //{
            //    double evasion = defComp.Evasion;
            //    double accuracy = 1.0;
            //    if (accuracyValues.Any())
            //    {
            //        accuracy = Math.Max(accuracyValues.Average(), 1.0);
            //    }
            //    double evasionBonus = Functions.CalculateEvasionBonusMultiplier(evasion, accuracy); // Evasion increases amount of time
            //    if (!newTimeLimit)
            //    {
            //        // Evasion pierce should be the same, because this path is only taken for updates during combat
            //        double previousBonus = Functions.CalculateEvasionBonusMultiplier(m_evasionUsed, accuracy);
            //        evasionBonus /= previousBonus; // Can't be 0
            //    }
            //    // Disabled, because time limit is no longer global
            //    double newDuration = world.TimeLimit.Duration * evasionBonus;
            //    world.TimeLimit.ChangeDuration(newDuration);
            //    m_evasionUsed = evasion;
            //}

            //if (m_inCombat)
            //{
            //    if (attackValues.Any())
            //    {
            //        var damage = attackValues.Average();

            //        double armor = defComp?.Armor ?? 0.0;
            //        double armorBonus = Functions.CalculateArmorBonusMultiplier(armor, damage);

            //        world.TimeLimit.Update(damage * dt / armorBonus); // armor 'slows time'

            //        if (world.TimeLimit.HasFinished) // Player lost the fight
            //        {
            //            var mobName = coordinator.GetEntities<MobComponent>().FirstOrDefault()?.GetName() ?? "??";
            //            coordinator.PostMessage(this, new BattleLostMessage(m_player, mobName, zone?.Level ?? 1));

            //            coordinator.GetEntities<MobComponent>().ForEach(e => coordinator.RemoveEntity(e.Id)); // Despawn all mobs
            //            m_inCombat = false;
            //        }
            //    }
            //}
        }

    }
}
