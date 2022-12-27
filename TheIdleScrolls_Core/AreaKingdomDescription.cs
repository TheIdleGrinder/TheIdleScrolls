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

        public ZoneDescription GetWildernessZoneDescription(int level)
        {
            return new ZoneDescription()
            {
                Name = "Wilderness",
                Level = level,
                EnemyTypes = new(),
                RemainingEnemies = Int32.MaxValue,
                TimeMultiplier = 1.0
            };
        }

        public List<MobDescription> GetLocalEnemies(string areaId)
        {
            foreach (var dungeon in Dungeons)
            {
                if (dungeon.Id == areaId)
                {
                    return dungeon.Enemies;
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
        public List<DungeonFloorDescription> Zones { get; set; } = new();
        public List<MobDescription> Enemies { get; set; } = new();
    }

    /// <summary>
    /// Description of a single zone within a dungeon
    /// </summary>
    public class DungeonFloorDescription
    {
        public int Enemies { get; set; } = 1;
        public double TimeMultiplier { get; set; } = 1.0;
        public List<string> EnemyTypes { get; set; } = new();
    }

    /// <summary>
    /// Description of a generic zone (both wilderness and dungeon)
    /// </summary>
    public class ZoneDescription
    {
        public string Name { get; set; } = "??";
        public int Level { get; set; } = 1;
        public double TimeMultiplier { get; set; } = 1.0;
        public int RemainingEnemies { get; set; } = 1;
        public List<string> EnemyTypes { get; set; } = new();
    }
}
