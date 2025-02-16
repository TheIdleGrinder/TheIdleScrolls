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
            List<MaterialId> simple   = [ MaterialId.Simple ];
            List<MaterialId> leathers = [ MaterialId.Leather1, MaterialId.Leather2, MaterialId.Leather3, MaterialId.Leather4 ];
            List<MaterialId> metals   = [ MaterialId.Metal1,   MaterialId.Metal2,   MaterialId.Metal3,   MaterialId.Metal4 ];
            List<MaterialId> woods    = [ MaterialId.Wood1,    MaterialId.Wood2,    MaterialId.Wood3,    MaterialId.Wood4 ];

            return new()
            {
                new(Definitions.ItemFamilies.OneHandedAxe, Properties.Items.Family_OneHandAxe, Abilities.Axe,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_OneHandAxe0, ItemTiers.LevelT0, false,  6.0, 1.45, simple),
                        MakeWeapon(Properties.Items.Genus_OneHandAxe1, ItemTiers.LevelT1, false, 10.0, 1.4 , metals),
                        MakeWeapon(Properties.Items.Genus_OneHandAxe2, ItemTiers.LevelT3, false, 13.0, 1.5, metals.Skip(2).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.TwoHandedAxe, Properties.Items.Family_TwoHandAxe, Abilities.Axe,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_TwoHandAxe1, ItemTiers.LevelT2,  true, 17.0, 1.7, metals),
                    }
                ),
                new(Definitions.ItemFamilies.OneHandedMace, Properties.Items.Family_OneHandMace, Abilities.Blunt,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_OneHandMace0, ItemTiers.LevelT0, false,  7.0, 1.65, simple),
                        MakeWeapon(Properties.Items.Genus_OneHandMace1, ItemTiers.LevelT1, false, 12.0, 1.65, metals),
                        MakeWeapon(Properties.Items.Genus_OneHandMace2, ItemTiers.LevelT3, false, 16.0, 1.8 , metals.Skip(2).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.TwoHandedMace, Properties.Items.Family_TwoHandMace, Abilities.Blunt,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_TwoHandMace1, ItemTiers.LevelT2,  true, 19.0, 1.9, metals),
                    }
                ),
                new(Definitions.ItemFamilies.LongSword, Properties.Items.Family_LongSword, Abilities.LongBlade,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_LongSword1,   ItemTiers.LevelT2, false, 11.0, 1.35, metals),
                    }
                ),
                new(Definitions.ItemFamilies.TwoHandedSword, Properties.Items.Family_TwoHandSword, Abilities.LongBlade,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_TwoHandSword0, ItemTiers.LevelT0,  true,  5.0, 1.25, simple),
                        MakeWeapon(Properties.Items.Genus_TwoHandSword1, ItemTiers.LevelT1,  true, 13.0, 1.5 , metals),
                        MakeWeapon(Properties.Items.Genus_TwoHandSword2, ItemTiers.LevelT3,  true, 15.0, 1.4 , metals.Skip(2).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.Spear, Properties.Items.Family_Spear, Abilities.Polearm,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_Spear0, ItemTiers.LevelT0,  true,  9.0, 2.05, simple),
                        MakeWeapon(Properties.Items.Genus_Spear1, ItemTiers.LevelT2,  true, 14.0, 1.4 , woods),
                        MakeWeapon(Properties.Items.Genus_Spear2, ItemTiers.LevelT3,  true, 25.0, 2.35, woods.Skip(2).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.Polearm, Properties.Items.Family_Polearm, Abilities.Polearm,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_Polearm1, ItemTiers.LevelT1, true, 20.0, 2.2, woods),
                    }
                ),
                new(Definitions.ItemFamilies.Dagger, Properties.Items.Family_Dagger, Abilities.ShortBlade,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_Dagger0, ItemTiers.LevelT0, false,  4.0, 1.0 , simple),
                        MakeWeapon(Properties.Items.Genus_Dagger1, ItemTiers.LevelT1, false,  5.0, 0.75, metals),
                    }                    
                ),
                new(Definitions.ItemFamilies.ShortSword, Properties.Items.Family_ShortSword, Abilities.ShortBlade,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_ShortSword1, ItemTiers.LevelT2, false,  9.0, 1.1 , metals),
                        MakeWeapon(Properties.Items.Genus_ShortSword2, ItemTiers.LevelT3, false,  8.0, 0.95, metals.Skip(2).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.LightChest, Properties.Items.Family_LightChest, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightChest0, ItemTiers.LevelT0, EquipmentSlot.Chest,  10.0,  6.0, simple),
                        MakeArmor(Properties.Items.Genus_LightChest1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetChest, 
                            EquipmentSlot.Chest,  18.0,  6.0, leathers),
                        MakeArmor(Properties.Items.Genus_LightChest2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetChest, 
                            EquipmentSlot.Chest,  21.0,  6.0, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightHelmet, Properties.Items.Family_LightHelmet, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightHelmet0, ItemTiers.LevelT1 + ItemTiers.LevelOffsetHelmet, 
                            EquipmentSlot.Head,   12.0,  4.0, leathers),
                        MakeArmor(Properties.Items.Genus_LightHelmet1, ItemTiers.LevelT2 + ItemTiers.LevelOffsetHelmet, 
                            EquipmentSlot.Head,   15.0,  4.0, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightGloves, Properties.Items.Family_LightGloves, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightGloves0, ItemTiers.LevelT1 + ItemTiers.LevelOffsetGloves, 
                            EquipmentSlot.Arms,   9.0,  2.5, leathers),
                        MakeArmor(Properties.Items.Genus_LightGloves1, ItemTiers.LevelT2 + ItemTiers.LevelOffsetGloves, 
                            EquipmentSlot.Arms,   11.0,  2.5, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightBoots, Properties.Items.Family_LightBoots, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightBoots0, ItemTiers.LevelT1 + ItemTiers.LevelOffsetBoots, 
                            EquipmentSlot.Legs,   9.0,  2.5, leathers),
                        MakeArmor(Properties.Items.Genus_LightBoots1, ItemTiers.LevelT2 + ItemTiers.LevelOffsetBoots,
                            EquipmentSlot.Legs,   11.0,  2.5, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightShield, Properties.Items.Family_LightShield, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightShield0, ItemTiers.LevelT1 + ItemTiers.LevelOffsetShield, 
                            EquipmentSlot.Hand,   12.0,  3.0, woods),
                        MakeArmor(Properties.Items.Genus_LIghtShield1, ItemTiers.LevelT2 + ItemTiers.LevelOffsetShield, 
                            EquipmentSlot.Hand,   14.0,  3.0, woods),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyChest, Properties.Items.Family_HeavyChest, Abilities.HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_HeavyChest0, ItemTiers.LevelT0, EquipmentSlot.Chest,  16.0, 12.0, simple),
                        MakeArmor(Properties.Items.Genus_HeavyChest1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetChest, 
                            EquipmentSlot.Chest,  22.0, 16.0, metals),
                        MakeArmor(Properties.Items.Genus_HeavyChest2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetChest, 
                            EquipmentSlot.Chest,  26.0, 16.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyHelmet, Properties.Items.Family_HeavyHelmet, Abilities.HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_HeavyHelmet0, ItemTiers.LevelT1 + ItemTiers.LevelOffsetHelmet, 
                            EquipmentSlot.Head,   16.0,  11.0, metals),
                        MakeArmor(Properties.Items.Genus_HeavyHelmet1, ItemTiers.LevelT2 + ItemTiers.LevelOffsetHelmet, 
                            EquipmentSlot.Head,   19.0,  11.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyGloves, Properties.Items.Family_HeavyGloves, Abilities.HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_HeavyGloves0, ItemTiers.LevelT1 + ItemTiers.LevelOffsetGloves, 
                            EquipmentSlot.Arms,   13.0,  6.0, metals),
                        MakeArmor(Properties.Items.Genus_HeavyGloves1, ItemTiers.LevelT2 + ItemTiers.LevelOffsetGloves, 
                            EquipmentSlot.Arms,   16.0,  6.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyBoots, Properties.Items.Family_HeavyBoots, Abilities.HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_HeavyBoots0, ItemTiers.LevelT1 + ItemTiers.LevelOffsetBoots, 
                            EquipmentSlot.Legs,   13.0,  6.0, metals),
                        MakeArmor(Properties.Items.Genus_HeavyBoots1, ItemTiers.LevelT2 + ItemTiers.LevelOffsetBoots, 
                            EquipmentSlot.Legs,   16.0,  6.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyShield, Properties.Items.Family_HeavyShield, Abilities.HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_HeavyShield0, ItemTiers.LevelT1 + ItemTiers.LevelOffsetShield, 
                            EquipmentSlot.Hand,   16.0,  9.0, woods),
                        MakeArmor(Properties.Items.Genus_HeavyShield1, ItemTiers.LevelT2 + ItemTiers.LevelOffsetShield, 
                            EquipmentSlot.Hand,   19.0,  9.0, woods),
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
