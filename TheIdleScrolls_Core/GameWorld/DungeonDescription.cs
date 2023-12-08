using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    /// <summary>
    /// Description of a dungeon and its floors
    /// </summary>
    public class DungeonDescription
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int Level { get; set; } = 1;
        public string Condition { get; set; } = "";
        public int Rarity { get; set; } = 0;
        public List<DungeonFloorDescription> Floors { get; set; } = new();
        public List<MobDescription> LocalMobs { get; set; } = new();
        public DungeonRewardsDescription Rewards { get; set; } = new();
    }

    /// <summary>
    /// Description of a single zone within a dungeon
    /// </summary>
    public class DungeonFloorDescription
    {
        public int MobCount { get; set; } = 1;
        public double TimeMultiplier { get; set; } = 1.0;
        public List<string> MobTypes { get; set; } = new();

        public DungeonFloorDescription() { }

        public DungeonFloorDescription(int mobCount, double timeMultiplier, List<string> mobTypes)
        {
            MobCount = mobCount;
            TimeMultiplier = timeMultiplier;
            MobTypes = mobTypes;
        }
    }

    public struct DungeonRewardsDescription
    {
        public int MinDropLevel { get; set; } = 1;
        public bool UseLeveledLoot { get; set; } = true;
        public List<string> SpecialRewards { get; set; } = new();

        public DungeonRewardsDescription() { }

        public DungeonRewardsDescription(int minDropLevel, bool useLeveledLoot, List<string> specialRewards)
        {
            MinDropLevel = minDropLevel;
            UseLeveledLoot = useLeveledLoot;
            SpecialRewards = specialRewards;
        }
    }
}
