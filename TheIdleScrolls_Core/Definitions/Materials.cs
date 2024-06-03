using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Definitions
{
    public static class Materials
    {
        public static readonly ushort Simple        = 0x00;

        public static readonly ushort Leather       = 0x10;
        public static readonly ushort HardLeather   = 0x11;
        public static readonly ushort Elvish        = 0x12;

        public static readonly ushort Iron          = 0x20;
        public static readonly ushort Steel         = 0x21;
        public static readonly ushort Dwarven       = 0x22;

        public static readonly ushort Beech         = 0x30;
        public static readonly ushort Oak           = 0x31;
        public static readonly ushort Ash           = 0x32;
    }

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
}
