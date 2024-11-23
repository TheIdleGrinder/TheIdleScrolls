using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Definitions
{
    public enum MaterialId
    {
        Simple          = 0x00,
        Leather1        = 0x11,
        Leather2        = 0x12,
        Leather3        = 0x13,
        Metal1          = 0x21,
        Metal2          = 0x22,
        Metal3          = 0x23,
        Wood1           = 0x31,
        Wood2           = 0x32,
        Wood3           = 0x33
    }

    public record ItemMaterial(MaterialId Id, string Name, double PowerMultiplier, int MinimumLevel)
    {
        public int Tier => (int)Id & 0x0F;
    }

    public static class Materials
    {
        public static readonly int LevelT0 = 0;
        public static readonly int LevelT1 = 10;
        public static readonly int LevelT2 = 30;
        public static readonly int LevelT3 = 50;
        public static readonly int LevelT4 = 80;

        public static List<ItemMaterial> MaterialList { get; } = new()
        {
            new(MaterialId.Simple,   Properties.LocalizedStrings.MAT_SIMPLE, 1.0, LevelT0),

            new(MaterialId.Leather1, Properties.LocalizedStrings.MAT_L0, 1.0,  LevelT1),
            new(MaterialId.Leather2, Properties.LocalizedStrings.MAT_L1, 1.5,  LevelT2),
            new(MaterialId.Leather3, Properties.LocalizedStrings.MAT_L2, 2.25, LevelT3),
            
            new(MaterialId.Metal1,   Properties.LocalizedStrings.MAT_M0, 1.0,  LevelT1),
            new(MaterialId.Metal2,   Properties.LocalizedStrings.MAT_M1, 1.5,  LevelT2),
            new(MaterialId.Metal3,   Properties.LocalizedStrings.MAT_M2, 2.25, LevelT3),
            
            new(MaterialId.Wood1,    Properties.LocalizedStrings.MAT_W0, 1.0,  LevelT1),
            new(MaterialId.Wood2,    Properties.LocalizedStrings.MAT_W1, 1.5,  LevelT2),
            new(MaterialId.Wood3,    Properties.LocalizedStrings.MAT_W2, 2.25, LevelT3)
        };
    }
}
