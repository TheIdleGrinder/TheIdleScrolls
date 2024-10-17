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
        Leather         = 0x10,
        HardLeather     = 0x11,
        Elvish          = 0x12,
        Iron            = 0x20,
        Steel           = 0x21,
        Dwarven         = 0x22,
        Beech           = 0x30,
        Oak             = 0x31,
        Ash             = 0x32
    }

    public record ItemMaterial(MaterialId Id, string Name, double PowerMultiplier, int MinimumLevel)
    {
        public int Tier => (int)Id & 0x0F;
    }

    public static class Materials
    {
        public static List<ItemMaterial> MaterialList { get; } = new()
        {
            new(MaterialId.Simple,      Properties.LocalizedStrings.MAT_SIMPLE, 1.0, 0),

            new(MaterialId.Leather,     Properties.LocalizedStrings.MAT_L0, 1.0,  10),
            new(MaterialId.HardLeather, Properties.LocalizedStrings.MAT_L1, 1.5,  30),
            new(MaterialId.Elvish,      Properties.LocalizedStrings.MAT_L2, 2.25, 50),
            
            new(MaterialId.Iron,        Properties.LocalizedStrings.MAT_M0, 1.0,  10),
            new(MaterialId.Steel,       Properties.LocalizedStrings.MAT_M1, 1.5,  30),
            new(MaterialId.Dwarven,     Properties.LocalizedStrings.MAT_M2, 2.25, 50),
            
            new(MaterialId.Beech,       Properties.LocalizedStrings.MAT_W0, 1.0,  10),
            new(MaterialId.Oak,         Properties.LocalizedStrings.MAT_W1, 1.5,  30),
            new(MaterialId.Ash,         Properties.LocalizedStrings.MAT_W2, 2.25, 50)
        };
    }
}
