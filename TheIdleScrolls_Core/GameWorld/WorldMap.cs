using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    public class WorldMap : IWorldMap
    {
        private const int MinLevel = 1;

        private IMapProgressPath _progressPath = new SpiralMapPath(MinLevel);

        public List<DungeonDescription> Dungeons { get; set; } = [];

        public List<DungeonDescription> GetDungeons()
        {
            return Dungeons;
        }

        public List<DungeonDescription> GetDungeonsAtLocation(Location location)
        {
            return Dungeons;
        }

        public ZoneDescription? GetDungeonZone(string dungeonId, int dungeonLevel, int floor)
        {
            var dungeon = Dungeons.Where(d => d.Id == dungeonId).FirstOrDefault();
            if (dungeon == null || floor < 0 || floor >= dungeon.Floors.Count)
            {
                return null;
            }
            var dungeonFloor = dungeon.Floors[floor];

            string floorName = dungeon.Name;
            if (dungeonFloor.Name == null)
            {
                floorName += $" - Floor {floor + 1}";
            }
            else if (dungeonFloor.Name != string.Empty)
            {
                floorName += $" - {dungeonFloor.Name}";
            }


            return new ZoneDescription()
            {
                Name = floorName,
                Biome = Biome.Dungeon,
                Level = dungeonLevel,
                MobTypes = dungeonFloor.MobTypes,
                MobCount = dungeonFloor.MobCount,
                TimeMultiplier = dungeonFloor.TimeMultiplier,
                SpecialDrops = dungeon.Rewards.SpecialRewards
            };
        }

        public Location? GetNextLocation(Location location)
        {
            return _progressPath.NextLocation(location);
        }

        public Location? GetPreviousLocation(Location location)
        {
            return _progressPath.PreviousLocation(location);
        }

        public ZoneDescription? GetZone(Location location)
        {
            int level = _progressPath.LocationLevel(location) ?? -1;
            if (level < 0)
                return null;

            Biome biome = CalculateBiome(level);
            string name = (biome == Biome.Dungeon) 
                ? "Training Grounds" // First 5 zones are marked as dungeon
                : biome.ToLocalizedString();
            return new()
            {
                Name = $"{name} - Level {level}",
                Biome = biome,
                Level = level,
                MobTypes = [],
                MobCount = 1,
                TimeMultiplier = 1.0
            };
        }

        private static Biome CalculateBiome(int areaLevel)
        {
            if (areaLevel <= 5)
            {
                return Biome.Dungeon;
            }
            int biomeCount = Enum.GetValues(typeof(Biome)).Length - 1; // -1 for dungeon
            return Enum.GetValues<Biome>()[((areaLevel - 5) / 10) % biomeCount + 1];
        }
    }
}
