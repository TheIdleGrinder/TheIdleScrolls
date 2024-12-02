using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Definitions
{
    public static class Abilities
    {
        public const string Axe         = "AXE";
        public const string Blunt       = "BLN";
        public const string LongBlade   = "LBL";
        public const string Polearm     = "POL";
        public const string ShortBlade  = "SBL";


        public const string LightArmor = "LAR";
        public const string HeavyArmor = "HAR";

        public const string Crafting = "ABL_CRAFT";

        public static readonly string[] Weapons = [ Axe, Blunt, LongBlade, Polearm, ShortBlade ];

        public static readonly string[] Armors = [ LightArmor, HeavyArmor ];
    }
}
