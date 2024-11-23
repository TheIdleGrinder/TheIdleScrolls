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

        public readonly static string[] Weapons = new string[] { OneHandedAxe, TwoHandedAxe, 
                                                                 OneHandedMace, TwoHandedMace, 
                                                                 LongSword, TwoHandedSword, 
                                                                 Spear, Polearm,
                                                                 Dagger, ShortSword };
        public readonly static string[] Armors  = new string[] { HeavyChest, HeavyHelmet, HeavyGloves, HeavyBoots, HeavyShield,
                                                                 LightChest, LightHelmet, LightGloves, LightBoots, LightShield };
    }

    public static class ItemTiers
    {
        public const int LevelT0            = 0;
        public const int LevelT1            = 0;
        public const int LevelT2            = 10;
        public const int LevelT3            = 20;

        public const int LevelOffsetWeapon  = 0;
        public const int LevelOffsetChest   = 5;
        public const int LevelOffsetHelmet  = 6;
        public const int LevelOffsetGloves  = 3;
        public const int LevelOffsetBoots   = 4;
        public const int LevelOffsetShield  = 7;
    }
}
