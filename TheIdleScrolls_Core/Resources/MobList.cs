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
        public static readonly MobDescription Dummy   = new("DUMMY",   LocalizedStrings.DUMMY,   AreaLevelCondition(  1,   5));
        public static readonly MobDescription Wolf    = new("WOLF",    LocalizedStrings.WOLF,    AreaLevelCondition(  6,  15));
        public static readonly MobDescription Boar    = new("BOAR",    LocalizedStrings.BOAR,    AreaLevelCondition(  9,  22));
        public static readonly MobDescription Bear    = new("BEAR",    LocalizedStrings.BEAR,    AreaLevelCondition( 16,  32));
        public static readonly MobDescription Ogre    = new("OGRE",    LocalizedStrings.OGRE,    AreaLevelCondition( 25,  50));
        public static readonly MobDescription Wyvern  = new("WYVERN",  LocalizedStrings.WYVERN,  AreaLevelCondition( 40,  74));
        public static readonly MobDescription Dragon  = new("DRAGON",  LocalizedStrings.DRAGON,  AreaLevelCondition( 75, 120));
        public static readonly MobDescription Dragon2 = new("DRAGON2", LocalizedStrings.DRAGON2, AreaLevelCondition(121, 150));
        public static readonly MobDescription Dragon3 = new("DRAGON3", LocalizedStrings.DRAGON3, AreaLevelCondition(151, 9999));

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

        public static Func<ZoneDescription, bool> BiomeCondition(Biome biome)
        {
            return (zone) => zone.Biome == biome;
        }

        public static Func<ZoneDescription, bool> BiomeLevelCondition(Biome biome, int minLevel, int maxLevel)
        {
            return (zone) => zone.Biome == biome && zone.Level >= minLevel && zone.Level <= maxLevel;
        }
    }
}
