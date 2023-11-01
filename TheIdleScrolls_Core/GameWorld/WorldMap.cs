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

        public List<DungeonDescription> Dungeons { get; set; } = new();

        public List<DungeonDescription> GetDungeons()
        {
            return Dungeons;
        }

        public List<DungeonDescription> GetDungeonsAtLocation(Location location)
        {
            return Dungeons;
        }

        public ZoneDescription? GetDungeonZone(string dungeonId, int floor)
        {
            var dungeon = Dungeons.Where(d => d.Name == dungeonId).FirstOrDefault();
            if (dungeon == null || floor < 0 || floor >= dungeon.Floors.Count)
            {
                return null;
            }
            var dungeonFloor = dungeon.Floors[floor];
            
            return new ZoneDescription()
            {
                Name = $"{dungeon.Name.Localize()} - Floor {floor + 1}",
                Biome = Biome.Dungeon,
                Level = dungeon.Level,
                MobTypes = dungeonFloor.MobTypes,
                MobCount = dungeonFloor.MobCount,
                TimeMultiplier = dungeonFloor.TimeMultiplier
            };
        }

        public Location? GetNextLocation(Location location)
        {
            if (location.Y != 0)
                return null;
            return new(location.X + 1, 0);
        }

        public Location? GetPreviousLocation(Location location)
        {
            if (location.Y != 0 || location.X == 0)
                return null;
            return new(location.X - 1, 0);
        }

        public ZoneDescription? GetZone(Location location)
        {
            if (location.Y != 0)
                return null;

            int level = location.X + MinLevel;
            Biome biome = CalculateBiome(level);
            string name = (biome == Biome.Dungeon) 
                ? "Training Grounds" // First 5 zones are marked as dungeon
                : biome.ToLocalizedString();
            return new()
            {
                Name = $"{name} - Level {level}",
                Biome = biome,
                Level = level,
                MobTypes = new(),
                MobCount = Int32.MaxValue,
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
