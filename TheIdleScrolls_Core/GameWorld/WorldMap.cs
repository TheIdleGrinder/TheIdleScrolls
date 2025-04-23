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
            return areaLevel switch
            {
                <= 5 => Biome.Dungeon,
                <= 14 => Biome.Grassland,
                <= 18 => Biome.Graveyard,
                <= 23 => Biome.Grassland,
                <= 34 => Biome.Forest,
                <= 38 => Biome.Grassland,
                <= 44 => Biome.Savannah,
                <= 53 => Biome.Desert,
                <= 60 => Biome.Savannah,
                <= 67 => Biome.Grassland,
                <= 74 => Biome.Coast,
                <= 81 => Biome.Grassland,
                <= 87 => Biome.Tundra,
                <= 94 => Biome.IcyDesert,
                <= 99 => Biome.Tundra,
                <= 106 => Biome.Grassland,
                <= 114 => Biome.Swamp,
                <= 119 => Biome.Grassland,
                <= 124 => Biome.Forest,
                <= 129 => Biome.Rainforest,
                <= 138 => Biome.Forest,
                <= 143 => Biome.Savannah,
                <= 149 => Biome.Desert,
                <= 156 => Biome.Oasis,
                <= 164 => Biome.Desert,
                <= 172 => Biome.Savannah,
                <= 199 => Biome.Wasteland,
                <= 209 => Biome.Tundra,
                <= 219 => Biome.IcyDesert,
                <= 229 => Biome.Tundra,
                <= 239 => Biome.Wasteland,
                <= 249 => Biome.Swamp,
                <= 259 => Biome.Grassland,
                <= 269 => Biome.Forest,
                <= 279 => Biome.Rainforest,
                <= 289 => Biome.Forest,
                <= 299 => Biome.Savannah,
                <= 309 => Biome.Desert,
                <= 319 => Biome.Oasis,
                <= 329 => Biome.Desert,
                <= 339 => Biome.Savannah,
                _ => Biome.Wasteland
            };
        }
    }
}
