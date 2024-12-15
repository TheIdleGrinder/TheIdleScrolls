using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Definitions
{
    public static class Abilities
    {
        // Offense
        public const string Axe         = "AXE";
        public const string Blunt       = "BLN";
        public const string LongBlade   = "LBL";
        public const string Polearm     = "POL";
        public const string ShortBlade  = "SBL";
        public const string Unarmed     = "UNARMED";

        // Defense
        public const string LightArmor = "LAR";
        public const string HeavyArmor = "HAR";
        public const string Unarmored  = "UNARMORED";

        // Crafting
        public const string Crafting   = "ABL_CRAFT";

        // Fighting Styles
        public const string TwoHanded     = "TWOHANDED";
        public const string DualWield     = "DUALWIELD";
        public const string Shielded      = "SHIELDED";
        public const string SingleHanded  = "SINGLEHANDED";


        public static List<string> Weapons { get; } = [Axe, Blunt, LongBlade, Polearm, ShortBlade];
        public static List<string> Attack { get; } = [.. Weapons, Unarmed];

        public static List<string> Armors { get; } = [LightArmor, HeavyArmor];
        public static List<string> Defense { get; } = [.. Armors, Unarmored];

        public static List<string> Styles { get; } = [TwoHanded, DualWield, Shielded, SingleHanded];
    }
}
