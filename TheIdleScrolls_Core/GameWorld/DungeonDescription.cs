﻿using MiniECS;
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
        public Func<Entity, World, int[]> AvailableLevels { get; set; } = (e, w) => [];
        public int Rarity { get; set; } = 0;
        public string Description { get; set; } = "";
        public List<DungeonFloorDescription> Floors { get; set; } = [];
        public List<MobDescription> LocalMobs { get; set; } = [];
        public DungeonRewardsDescription Rewards { get; set; } = new();
    }

    /// <summary>
    /// Description of a single zone within a dungeon
    /// </summary>
    public class DungeonFloorDescription
    {
        public string? Name { get; set; } = string.Empty;
        public int MobCount { get; set; } = 1;
        public double TimeMultiplier { get; set; } = 1.0;
        public List<string> MobTypes { get; set; } = [];

        public DungeonFloorDescription() { }

        public DungeonFloorDescription(int mobCount, double timeMultiplier, List<string> mobTypes, string? name = null)
        {
            MobCount = mobCount;
            TimeMultiplier = timeMultiplier;
            MobTypes = mobTypes;
            Name = name;
        }
    }

    public struct DungeonRewardsDescription
    {
        public int DropLevelRange { get; set; } = Definitions.Stats.DungeonDropLevelRange;
        public bool UseLeveledLoot { get; set; } = true;
        public List<string> SpecialRewards { get; set; } = [];

        public DungeonRewardsDescription() { }

        public DungeonRewardsDescription(int dropLevelRange, bool useLeveledLoot, List<string> specialRewards)
        {
            DropLevelRange = dropLevelRange;
            UseLeveledLoot = useLeveledLoot;
            SpecialRewards = specialRewards;
        }
    }
}
