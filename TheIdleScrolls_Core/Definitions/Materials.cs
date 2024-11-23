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
        Leather4        = 0x14,
        Metal1          = 0x21,
        Metal2          = 0x22,
        Metal3          = 0x23,
        Metal4          = 0x24,
        Wood1           = 0x31,
        Wood2           = 0x32,
        Wood3           = 0x33,
        Wood4           = 0x34
    }

    public record ItemMaterial(MaterialId Id, string Name, double PowerMultiplier, int MinimumLevel)
    {
        public int Tier => (int)Id & 0x0F;
    }

    public static class Materials
    {
        public const int LevelT0 = 0;
        public const int LevelT1 = 10;
        public const int LevelT2 = 30;
        public const int LevelT3 = 50;
        public const int LevelT4 = 80;

        public static List<ItemMaterial> MaterialList { get; } = new()
        {
            new(MaterialId.Simple,   Properties.Items.Material_Simple, 1.0,  LevelT0),

            new(MaterialId.Leather1, Properties.Items.Material_L1,     1.0,  LevelT1),
            new(MaterialId.Leather2, Properties.Items.Material_L2,     1.5,  LevelT2),
            new(MaterialId.Leather3, Properties.Items.Material_L3,     2.25, LevelT3),
            new(MaterialId.Leather4, Properties.Items.Material_L4,     3.3,  LevelT4),
            
            new(MaterialId.Metal1,   Properties.Items.Material_M1,     1.0,  LevelT1),
            new(MaterialId.Metal2,   Properties.Items.Material_M2,     1.5,  LevelT2),
            new(MaterialId.Metal3,   Properties.Items.Material_M3,     2.25, LevelT3),
            new(MaterialId.Metal4,   Properties.Items.Material_M4,     3.3,  LevelT4),

            new(MaterialId.Wood1,    Properties.Items.Material_W1,     1.0,  LevelT1),
            new(MaterialId.Wood2,    Properties.Items.Material_W2,     1.5,  LevelT2),
            new(MaterialId.Wood3,    Properties.Items.Material_W3,     2.25, LevelT3),
            new(MaterialId.Wood4,    Properties.Items.Material_W4,     3.3,  LevelT4)
        };
    }
}
