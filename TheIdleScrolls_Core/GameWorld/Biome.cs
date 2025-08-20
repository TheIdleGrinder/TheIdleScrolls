using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    public enum Biome
    {
        Dungeon,
        Grassland,
        Forest,
        Rainforest,
        Swamp,
        Savannah,
        Desert,
        Oasis,
        Tundra,
        IcyDesert,
        Wasteland,
        Graveyard,
        Coast,
        Foothills,
        Mountains,
        Summit
    }

    public static class BiomeExtensions
    {
        public static string ToLocalizedString(this Biome biome)
        {
            return biome switch
            {
                Biome.Dungeon => Properties.LocalizedStrings.BIOME_DUNGEON,
                Biome.Grassland => Properties.LocalizedStrings.BIOME_GRASSLAND,
                Biome.Forest => Properties.LocalizedStrings.BIOME_FOREST,
                Biome.Rainforest => Properties.LocalizedStrings.BIOME_RAINFOREST,
                Biome.Swamp => Properties.LocalizedStrings.BIOME_SWAMP,
                Biome.Savannah => Properties.LocalizedStrings.BIOME_SAVANNAH,
                Biome.Desert => Properties.LocalizedStrings.BIOME_DESERT,
                Biome.Oasis => Properties.LocalizedStrings.BIOME_OASIS,
                Biome.Tundra => Properties.LocalizedStrings.BIOME_TUNDRA,
                Biome.IcyDesert => Properties.LocalizedStrings.BIOME_ICE,
                Biome.Wasteland => Properties.LocalizedStrings.BIOME_WASTELAND,
                Biome.Graveyard => Properties.LocalizedStrings.BIOME_GRAVEYARD,
                Biome.Coast => Properties.LocalizedStrings.BIOME_COAST,
                Biome.Foothills => Properties.LocalizedStrings.BIOME_FOOTHILLS,
                Biome.Mountains => Properties.LocalizedStrings.BIOME_MOUNTAINS,
                Biome.Summit => Properties.LocalizedStrings.BIOME_SUMMIT,
                _ => "??"
            };
        }
    }
}
