using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Properties;

namespace TheIdleScrolls_Core.Resources
{
    public static class MobList
    {
        private static Biome[] LushBiomes = [Biome.Grassland, Biome.Forest, Biome.Coast];

        public static readonly List<MobDescription> Mobs =
        [
            new("DUMMY",        MobNames.DUMMY,     LevelCondition(1, 5))
            {
                Damage = 0.0
            },
            new("BOAR",         MobNames.BOAR,      BiomeLevelCondition(LushBiomes,  6, 19)),
            new("PORCUPINE",    MobNames.Porcupine, BiomeLevelCondition(LushBiomes,  9, 34)),
            new("WOLF",         MobNames.WOLF,      BiomeLevelCondition(LushBiomes, 18, 47)),
            new("OGRE",         MobNames.OGRE,      BiomeLevelCondition([Biome.Grassland, Biome.Coast], 35,  70)),
            new("BEAR",         MobNames.BEAR,      BiomeLevelCondition([Biome.Grassland, Biome.Forest], 60, 130)),
            new("HILLGIANT",    MobNames.HillGiant, BiomeLevelCondition([Biome.Grassland], 130,  199)),
            new("WYVERN",       MobNames.WYVERN,    BiomeLevelCondition([Biome.Grassland, Biome.Coast], 149)),
            //new("DRAGON",  MobNames.DRAGON,  LevelCondition(100, 169)),
            //new("DRAGON2", MobNames.DRAGON2, LevelCondition(170, 229)),
            //new("DRAGON3", MobNames.DRAGON3, LevelCondition(230)),

            new("BAT", MobNames.Bat, BiomeCondition([Biome.Graveyard])),
            new("GRAVECRAWLER", MobNames.Gravecrawler, BiomeCondition([Biome.Graveyard])),

            new("FORESTTROLL", MobNames.ForestTroll, BiomeLevelCondition([Biome.Forest], 50, 200)),
            new("TREANT", MobNames.Treant, BiomeLevelCondition([Biome.Forest], 125)),

            new("HYENA", MobNames.Hyena, BiomeLevelCondition([Biome.Savannah], maxLevel: 100)),
            new("BUFFALO", MobNames.Buffalo, BiomeLevelCondition([Biome.Savannah], maxLevel: 200)),
            new("ELEPHANT", MobNames.Elephant, BiomeLevelCondition([Biome.Savannah], 100)),

            new("SCORPION", MobNames.Scorpion, BiomeLevelCondition([Biome.Desert], maxLevel: 100)),
            new("SPHINXHOUND", MobNames.SphinxHound, BiomeLevelCondition([Biome.Desert, Biome.Oasis], maxLevel: 200)),
            new("MANTICORE", MobNames.Manticore, BiomeLevelCondition([Biome.Desert, Biome.Oasis], 100)),

            new("DJINN", MobNames.Djinn, BiomeCondition([Biome.Oasis])),

            new("HARPY", MobNames.Harpy, BiomeLevelCondition([Biome.Coast, Biome.Grassland], 65, 100)),
            new("CRAB", MobNames.Crab, BiomeLevelCondition([Biome.Coast], maxLevel: 100)),

            new("DIREWOLF", MobNames.Direwolf, BiomeLevelCondition([Biome.Tundra], maxLevel: 150)),
            new("GARGANTUAN", MobNames.Gargantuan, BiomeLevelCondition([Biome.Tundra, Biome.Grassland], 90, maxLevel: 200)),
            new("WEREWOLF", MobNames.Werewolf, BiomeLevelCondition([Biome.Tundra], 151)),

            new("POLARBEAR", MobNames.Polarbear, BiomeLevelCondition([Biome.IcyDesert], maxLevel: 200)),
            new("YETI", MobNames.Yeti, BiomeLevelCondition([Biome.IcyDesert], 100)),

            new("UMBERHULK", MobNames.UmberHulk, BiomeCondition([Biome.Swamp])),
            new("SWAMPTROLL", MobNames.SwampTroll, BiomeLevelCondition([Biome.Swamp], maxLevel: 200)),

            new("APE", MobNames.Ape, BiomeLevelCondition([Biome.Rainforest], maxLevel: 200)),
            new("COCKATRICE", MobNames.Cockatrice, BiomeLevelCondition([Biome.Rainforest], 150)),

            new("BLUESLAAD", MobNames.BlueSlaad, BiomeLevelCondition([Biome.Wasteland], 185)),
            new("SHADOWGIANT", MobNames.ShadowGiant, BiomeLevelCondition([Biome.Wasteland], maxLevel: 225)),
            new("WEREBULL", MobNames.Werebull, BiomeLevelCondition([Biome.Wasteland], maxLevel: 200)),

            new("AIRELEMENTAL", MobNames.AirElemental, BiomeLevelCondition([Biome.Tundra], 201)),
            new("ASHELEMENTAL", MobNames.AshElemental, BiomeLevelCondition([Biome.Wasteland], 201)),
            new("EARTHELEMENTAL", MobNames.EarthElemental, BiomeLevelCondition([Biome.Grassland, Biome.Savannah], 201)),
            new("FIREELEMENTAL", MobNames.FireElemental, BiomeLevelCondition([Biome.Desert], 201)),
            new("ICEELEMENTAL", MobNames.IceElemental, BiomeLevelCondition([Biome.IcyDesert], 201)),
            new("WATERELEMENTAL", MobNames.WaterElemental, BiomeLevelCondition([Biome.Swamp, Biome.Oasis], 201)),
            new("WOODELEMENTAL", MobNames.WoodElemental, BiomeLevelCondition([Biome.Forest, Biome.Rainforest], 201)),
        ];

        public static Func<ZoneDescription, bool> LevelCondition(int minLevel = 0, int maxLevel = Int32.MaxValue)
        {
            return (zone) => zone.Level >= minLevel && zone.Level <= maxLevel;
        }

        public static Func<ZoneDescription, bool> BiomeCondition(Biome[] biomes)
        {
            return (zone) => biomes.Contains(zone.Biome);
        }

        public static Func<ZoneDescription, bool> BiomeLevelCondition(Biome[] biomes, int minLevel = 0, int maxLevel = Int32.MaxValue)
        {
            return (zone) => biomes.Contains(zone.Biome) && zone.Level >= minLevel && zone.Level <= maxLevel;
        }
    }
}
