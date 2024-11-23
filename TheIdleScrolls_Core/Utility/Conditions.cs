﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Utility
{
    using ConditionFunction = Func<Entity, World, bool>;

    public static class Conditions
    {
        public static bool HasClearedWildernessLevel(Entity e, int level)
        {
            return (e.GetComponent<PlayerProgressComponent>()?.Data?.HighestWildernessKill ?? 0) >= level;
        }

        public static bool IsDungeonAvailable(Entity e, string dungeonId)
        {
            return e.GetComponent<TravellerComponent>()?.AvailableDungeons?.ContainsKey(dungeonId) ?? false;
        }

        public static bool IsDungeonLevelAvailable(Entity e, string dungeonId, int level)
        {
            return (e.GetComponent<TravellerComponent>()?.AvailableDungeons?.TryGetValue(dungeonId, out int[]? levels) ?? false) &&
                levels.Contains(level);
        }

        public static bool HasCompletedDungeon(Entity e, string dungeonId)
        {
            return e.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeons().Contains(dungeonId) ?? false;
        }

        public static bool HasCompletedDungeonLevel(Entity e, string dungeonId, int level)
        {
            return e.GetComponent<PlayerProgressComponent>()?.Data?.GetClearedDungeonLevels()
                ?.Any(dl => dl.Dungeon == dungeonId && dl.Level >= level) ?? false;
        }

        public static bool IsAchievementUnlocked(World w, string achievementId)
        {
            return w.GlobalEntity.GetComponent<AchievementsComponent>()?.Achievements
                ?.Any(a => a.Id == achievementId && a.Status == Achievements.AchievementStatus.Awarded) ?? false;
        }

        public static bool HasDefeatedMobs(Entity e, IEnumerable<string> mobTypes)
        {
            return mobTypes.All(m => e.GetComponent<PlayerProgressComponent>()?.Data?.DefeatedMobTypes.Contains(m) ?? false);
        }

        public static ConditionFunction WildernessLevelClearedCondition(int level)
        {
            return (e, w) => HasClearedWildernessLevel(e, level);
        }

        public static ConditionFunction DungeonAvailableCondition(string dungeonId)
        {
            return (e, w) => IsDungeonAvailable(e, dungeonId);
        }

        public static ConditionFunction DungeonLevelAvailableCondition(string dungeonId, int level)
        {
            return (e, w) => IsDungeonLevelAvailable(e, dungeonId, level);
        }

        public static ConditionFunction DungeonCompletedCondition(string dungeonId)
        {
            return (e, w) => HasCompletedDungeon(e, dungeonId);
        }

        public static ConditionFunction DungeonLevelCompletedCondition(string dungeonId, int level)
        {
            return (e, w) => HasCompletedDungeonLevel(e, dungeonId, level);
        }

        public static ConditionFunction AchievementUnlockedCondition(string achievementId)
        {
            return (e, w) => IsAchievementUnlocked(w, achievementId);
        }

        public static ConditionFunction MobsDefeatedCondition(IEnumerable<string> mobTypes)
        {
            return (e, w) => HasDefeatedMobs(e, mobTypes);
        }
    }
}
