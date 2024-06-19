using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Definitions
{
    public enum EquipmentSlot
    {
        Hand, Chest, Head, Arms, Legs
    }

    public static class ItemFamilies
    {
        public const string Axe         = "AXE";
        public const string Blunt       = "BLN";
        public const string LongBlade   = "LBL";
        public const string Polearm     = "POL";
        public const string ShortBlade  = "SBL";

        public const string HeavyArmor  = "HAR";
        public const string LightArmor  = "LAR";

        public readonly static string[] Weapons = new string[] { Axe, Blunt, LongBlade, Polearm, ShortBlade };
        public readonly static string[] Armors = new string[] { HeavyArmor, LightArmor };
    }
}
