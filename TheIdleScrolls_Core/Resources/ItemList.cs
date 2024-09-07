using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Resources
{
    internal static class ItemList
    {
        static readonly List<ItemFamilyDescription> s_items = GenerateItems();

        public static List<ItemFamilyDescription> ItemFamilies { get { return s_items; } }

        static List<ItemFamilyDescription> GenerateItems()
        {
            List<MaterialId> simple = new() { MaterialId.Simple };
            List<MaterialId> leathers = new() { MaterialId.Leather, MaterialId.HardLeather, MaterialId.Elvish };
            List<MaterialId> metals = new() { MaterialId.Iron, MaterialId.Steel, MaterialId.Dwarven };
            List<MaterialId> woods = new() { MaterialId.Beech, MaterialId.Oak, MaterialId.Ash };

            return new()
            {
                new(Definitions.ItemFamilies.OneHandedAxe, Properties.Items.Family_OneHandAxe, Properties.Constants.Key_Ability_Axe,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_OneHandAxe0,  0, false,  5.0, 1.25, simple),
                        MakeWeapon(Properties.Items.Genus_OneHandAxe1,  0, false,  9.0, 1.25, metals),
                        MakeWeapon(Properties.Items.Genus_OneHandAxe2, 20, false, 12.0, 1.35, metals.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.TwoHandedAxe, Properties.Items.Family_TwoHandAxe, Properties.Constants.Key_Ability_Axe,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_TwoHandAxe1, 10,  true, 18.0, 1.85, metals),
                    }
                ),
                new(Definitions.ItemFamilies.OneHandedMace, Properties.Items.Family_OneHandMace, Properties.Constants.Key_Ability_Blunt,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_OneHandMace0,  0, false,  7.0, 1.65, simple),
                        MakeWeapon(Properties.Items.Genus_OneHandMace1,  0, false, 10.0, 1.4 , metals),
                        MakeWeapon(Properties.Items.Genus_OneHandMace2, 20, false, 15.0, 1.65, metals.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.TwoHandedMace, Properties.Items.Family_TwoHandMace, Properties.Constants.Key_Ability_Blunt,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_TwoHandMace1, 10,  true, 19.0, 1.95, metals),
                    }
                ),
                new(Definitions.ItemFamilies.LongSword, Properties.Items.Family_LongSword, Properties.Constants.Key_Ability_LongBlade,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_LongSword1,  0, false, 11.0, 1.5 , metals),
                    }
                ),
                new(Definitions.ItemFamilies.TwoHandedSword, Properties.Items.Family_TwoHandSword, Properties.Constants.Key_Ability_LongBlade,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_TwoHandSword0,  0,  true,  7.0, 1.4 , simple),
                        MakeWeapon(Properties.Items.Genus_TwoHandSword1, 10,  true, 18.0, 1.85, metals),
                        MakeWeapon(Properties.Items.Genus_TwoHandSword2, 20,  true, 18.0, 1.65, metals.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.Spear, Properties.Items.Family_Spear, Properties.Constants.Key_Ability_Polearm,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_Spear0,  0,  true, 11.0, 2.1 , simple),
                        MakeWeapon(Properties.Items.Genus_Spear1, 10,  true, 13.0, 1.4 , woods),
                        MakeWeapon(Properties.Items.Genus_Spear2, 20,  true, 26.0, 2.3 , woods.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.Polearm, Properties.Items.Family_Polearm, Properties.Constants.Key_Ability_Polearm,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_Polearm1,  0,  true, 20.0, 2.15, woods),
                    }
                ),
                new(Definitions.ItemFamilies.Dagger, Properties.Items.Family_Dagger, Properties.Constants.Key_Ability_ShortBlade,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_Dagger0,  0, false,  4.0, 1.0 , simple),
                        MakeWeapon(Properties.Items.Genus_Dagger1,  0, false,  5.0, 0.75, metals),
                    }                    
                ),
                new(Definitions.ItemFamilies.ShortSword, Properties.Items.Family_ShortSword, Properties.Constants.Key_Ability_ShortBlade,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_ShortSword1, 10, false,  8.0, 1.05, metals),
                        MakeWeapon(Properties.Items.Genus_ShortSword2, 20, false,  8.0, 0.95, metals.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.LightChest, Properties.Items.Family_LightChest, Properties.Constants.Key_Ability_LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightChest0,  0, EquipmentSlot.Chest,  10.0,  6.0, simple),
                        MakeArmor(Properties.Items.Genus_LightChest1,  5, EquipmentSlot.Chest,  14.0,  6.0, leathers),
                        MakeArmor(Properties.Items.Genus_LightChest2, 15, EquipmentSlot.Chest,  16.0,  6.0, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightHelmet, Properties.Items.Family_LightHelmet, Properties.Constants.Key_Ability_LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightHelmet0,  6, EquipmentSlot.Head,   10.0,  4.0, leathers),
                        MakeArmor(Properties.Items.Genus_LightHelmet1, 16, EquipmentSlot.Head,   12.0,  4.0, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightGloves, Properties.Items.Family_LightGloves, Properties.Constants.Key_Ability_LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightGloves0,  3, EquipmentSlot.Arms,   8.0,  2.5, leathers),
                        MakeArmor(Properties.Items.Genus_LightGloves1, 13, EquipmentSlot.Arms,   10.0,  2.5, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightBoots, Properties.Items.Family_LightBoots, Properties.Constants.Key_Ability_LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightBoots0,  4, EquipmentSlot.Legs,   8.0,  2.5, leathers),
                        MakeArmor(Properties.Items.Genus_LightBoots1, 14, EquipmentSlot.Legs,   10.0,  2.5, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightShield, Properties.Items.Family_LightShield, Properties.Constants.Key_Ability_LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightShield0,  7, EquipmentSlot.Hand,   10.0,  3.0, woods),
                        MakeArmor(Properties.Items.Genus_LIghtShield1, 17, EquipmentSlot.Hand,   12.0,  3.0, woods),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyChest, Properties.Items.Family_HeavyChest, Properties.Constants.Key_Ability_HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.LocalizedStrings.HAR0,  0, EquipmentSlot.Chest,  16.0, 12.0, simple),
                        MakeArmor(Properties.LocalizedStrings.HAR1,  5, EquipmentSlot.Chest,  22.0, 12.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR6, 15, EquipmentSlot.Chest,  26.0, 12.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyHelmet, Properties.Items.Family_HeavyHelmet, Properties.Constants.Key_Ability_HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.LocalizedStrings.HAR2,  6, EquipmentSlot.Head,   16.0,  8.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR7, 16, EquipmentSlot.Head,   19.0,  8.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyGloves, Properties.Items.Family_HeavyGloves, Properties.Constants.Key_Ability_HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.LocalizedStrings.HAR3,  3, EquipmentSlot.Arms,   13.0,  5.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR8, 13, EquipmentSlot.Arms,   16.0,  5.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyBoots, Properties.Items.Family_HeavyBoots, Properties.Constants.Key_Ability_HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.LocalizedStrings.HAR4,  4, EquipmentSlot.Legs,   13.0,  5.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR9, 14, EquipmentSlot.Legs,   16.0,  5.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyShield, Properties.Items.Family_HeavyShield, Properties.Constants.Key_Ability_HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.LocalizedStrings.HAR5,   7, EquipmentSlot.Hand,   16.0,  6.0, woods),
                        MakeArmor(Properties.LocalizedStrings.HAR10, 17, EquipmentSlot.Hand,   19.0,  6.0, woods),
                    }
                )
            };
        }

        private static ItemGenusDescription MakeItemDescription(
            string name, int level, List<EquipmentSlot> slots, WeaponGenus? weapon, ArmorGenus? armor, double encumbrance, List<MaterialId> materials)
        {
            return new(name, new(slots, encumbrance), weapon, armor, level, materials);
        }

        private static ItemGenusDescription MakeWeapon(string name, int level, bool twohanded, double damage, double cooldown, List<MaterialId> materials)
        {
            List<EquipmentSlot> slots = Enumerable.Repeat(EquipmentSlot.Hand, twohanded ? 2 : 1).ToList();
            return MakeItemDescription(name, level, slots, new(damage, cooldown), null, 0.0, materials);
        }

        private static ItemGenusDescription MakeArmor(string name, int level, EquipmentSlot slot, double armor, double encumbrance, List<MaterialId> materials)
        {
            return MakeItemDescription(name, level, new() { slot }, null, new(armor, 0.0), encumbrance, materials);
        }
    }
}
