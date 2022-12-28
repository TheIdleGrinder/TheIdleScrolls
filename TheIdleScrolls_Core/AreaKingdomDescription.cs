using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core
{
    public class AreaKingdomDescription
    {
        public List<DungeonDescription> Dungeons { get; set; } = new();

        public ZoneDescription GetZoneDescription(string areaId, int zoneNumber)
        {
            if (areaId == "") // Wilderness
            {
                return GetWildernessZoneDescription(zoneNumber);
            }
            else 
            { 
                var dungeon = Dungeons.Find(d => d.Id == areaId);
                if (dungeon == null)
                    throw new Exception($"Invalid dungeon id: {areaId}");
                if (zoneNumber < 0 || zoneNumber >= dungeon.Floors.Count)
                    throw new Exception($"Dungeon '{dungeon.Name}' has no floor {zoneNumber}");
                var floor = dungeon.Floors[zoneNumber];
                return new ZoneDescription()
                {
                    Name = $"{dungeon.Name} - Floor {zoneNumber}",
                    Level = dungeon.Level,
                    MobTypes = floor.MobTypes,
                    MobCount = floor.MobCount,
                    TimeMultiplier = floor.TimeMultiplier
                };
            }
        }

        ZoneDescription GetWildernessZoneDescription(int level)
        {
            return new ZoneDescription()
            {
                Name = "Wilderness",
                Level = level,
                MobTypes = new(),
                MobCount = Int32.MaxValue,
                TimeMultiplier = 1.0
            };
        }

        public List<MobDescription> GetLocalEnemies(string areaId)
        {
            foreach (var dungeon in Dungeons)
            {
                if (dungeon.Id == areaId)
                {
                    return dungeon.LocalMobs;
                }
            }
            return new();
        }
    }

    /// <summary>
    /// Description of a dungeon and its floors
    /// </summary>
    public class DungeonDescription
    {
        public string Name { get; set; } = "";
        public string Id { get; set; } = "";
        public int Level { get; set; } = 1;
        public List<DungeonFloorDescription> Floors { get; set; } = new();
        public List<MobDescription> LocalMobs { get; set; } = new();
    }

    /// <summary>
    /// Description of a single zone within a dungeon
    /// </summary>
    public class DungeonFloorDescription
    {
        public int MobCount { get; set; } = 1;
        public double TimeMultiplier { get; set; } = 1.0;
        public List<string> MobTypes { get; set; } = new();
    }

    /// <summary>
    /// Description of a generic zone (both wilderness and dungeon)
    /// </summary>
    public class ZoneDescription
    {
        public string Name { get; set; } = "??";
        public int Level { get; set; } = 1;
        public double TimeMultiplier { get; set; } = 1.0;
        public int MobCount { get; set; } = 1;
        public List<string> MobTypes { get; set; } = new();
    }
}
