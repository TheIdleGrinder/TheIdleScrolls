using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

using TheIdleScrolls_Core.Properties;

namespace TheIdleScrolls_Core.Resources
{
    internal static class DungeonList
    {
        public static DungeonDescription? GetDungeon(string dungeonId)
        {
            return s_dungeonDescriptions.Where(d => d.Id == dungeonId).FirstOrDefault();
        }

        static List<DungeonDescription> s_dungeonDescriptions = GenerateDungeons();

        public static List<DungeonDescription> GetAllDungeons() => s_dungeonDescriptions;

		public static Func<Entity, World, int[]> UnlockedAtEqualWilderness(int level)
		{
			return (e, w) => Utility.Conditions.HasClearedWildernessLevel(e, level) ? [level] : [];
		}
		public static Func<Entity, World, int[]> UnlockedForEachWildernessLevel(int[] levels)
		{
			return (e, w) => levels.Where(l => Utility.Conditions.HasClearedWildernessLevel(e, l)).ToArray();
		}
		public static Func<Entity, World, int[]> UnlockedAfterDungeon(string dungeonId, int level)
		{
			return (e, w) => Utility.Conditions.HasCompletedDungeon(e, dungeonId) ? [level] : [];
		}
		public static Func<Entity, World, int[]> UnlockedAfterDungeonAndWilderness(string dungeonId, int level)
		{
			return (e, w) => Utility.Conditions.HasCompletedDungeon(e, dungeonId)
							&& Utility.Conditions.HasClearedWildernessLevel(e, level) ? [level] : [];
		}

		static List<DungeonDescription> GenerateDungeons()
        {
            return [];
        }
    }
}
