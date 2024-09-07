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
        //public const string Polearm     = "POL";
        public const string ShortBlade  = "SBL";

        public const string HeavyArmor  = "HAR";
        public const string LightArmor  = "LAR";


        public const string OneHandedAxe    = "OAX";
        public const string TwoHandedAxe    = "TAX";
        public const string OneHandedMace   = "OMC";
        public const string TwoHandedMace   = "TMC";
        public const string LongSword       = "LSW";
        public const string TwoHandedSword  = "TSW";
        public const string Spear           = "SPE";
        public const string Polearm         = "POL";
        public const string Dagger          = "DAG";
        public const string ShortSword      = "SSW";

        public const string HeavyChest      = "HCH";
        public const string HeavyHelmet     = "HHE";
        public const string HeavyGloves     = "HGL";
        public const string HeavyBoots      = "HBT";
        public const string HeavyShield     = "HSH";

        public const string LightChest      = "LCH";
        public const string LightHelmet     = "LHE";
        public const string LightGloves     = "LGL";
        public const string LightBoots      = "LBT";
        public const string LightShield     = "LSH";

        public readonly static string[] Weapons = new string[] { Axe, Blunt, LongBlade, Polearm, ShortBlade,
                                                                 OneHandedAxe, TwoHandedAxe, 
                                                                 OneHandedMace, TwoHandedMace, 
                                                                 LongSword, TwoHandedSword, 
                                                                 Spear, /*Polearm, */
                                                                 Dagger, ShortSword };
        public readonly static string[] Armors = new string[] { HeavyArmor, LightArmor,
                                                                HeavyChest, HeavyHelmet, HeavyGloves, HeavyBoots, HeavyShield,
                                                                LightChest, LightHelmet, LightGloves, LightBoots, LightShield };
    }
}
