using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Resources
{
    public static class MobList
    {
        public static readonly MobDescription Dummy   = new("DUMMY",   Properties.Mobs.DUMMY,   AreaLevelCondition(  1,   5));
        public static readonly MobDescription Wolf    = new("WOLF",    Properties.Mobs.WOLF,    AreaLevelCondition(  6,  15));
        public static readonly MobDescription Boar    = new("BOAR",    Properties.Mobs.BOAR,    AreaLevelCondition(  9,  22));
        public static readonly MobDescription Bear    = new("BEAR",    Properties.Mobs.BEAR,    AreaLevelCondition( 16,  32));
        public static readonly MobDescription Ogre    = new("OGRE",    Properties.Mobs.OGRE,    AreaLevelCondition( 25,  50));
        public static readonly MobDescription Wyvern  = new("WYVERN",  Properties.Mobs.WYVERN,  AreaLevelCondition( 40,  74));
        public static readonly MobDescription Dragon  = new("DRAGON",  Properties.Mobs.DRAGON,  AreaLevelCondition( 75, 120));
        public static readonly MobDescription Dragon2 = new("DRAGON2", Properties.Mobs.DRAGON2, AreaLevelCondition(121, 150));
        public static readonly MobDescription Dragon3 = new("DRAGON3", Properties.Mobs.DRAGON3, AreaLevelCondition(151, 9999));

        public static readonly List<MobDescription> Mobs =
        [
            Dummy,
            Wolf,
            Boar,
            Bear,
            Ogre,
            Wyvern,
            Dragon,
            Dragon2,
            Dragon3
        ];

        public static Func<ZoneDescription, bool> AreaLevelCondition(int minLevel, int maxLevel)
        {
            return (zone) => zone.Level >= minLevel && zone.Level <= maxLevel;
        }

        public static Func<ZoneDescription, bool> BiomeCondition(Biome[] biomes)
        {
            return (zone) => biomes.Contains(zone.Biome);
        }

        public static Func<ZoneDescription, bool> BiomeLevelCondition(int minLevel, int maxLevel, Biome[] biomes)
        {
            return (zone) => biomes.Contains(zone.Biome) && zone.Level >= minLevel && zone.Level <= maxLevel;
        }
    }
}
