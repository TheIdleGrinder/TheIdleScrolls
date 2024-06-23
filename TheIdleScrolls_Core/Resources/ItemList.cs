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
                new(Definitions.ItemFamilies.ShortBlade, Properties.LocalizedStrings.SBL,
                    new()
                    {
                        MakeWeapon(Properties.LocalizedStrings.SBL0,  0, false,  4.0, 1.0 , simple),
                        MakeWeapon(Properties.LocalizedStrings.SBL1, 10, false,  5.0, 0.75, metals),
                        MakeWeapon(Properties.LocalizedStrings.SBL2, 20, false,  8.0, 1.05, metals),
                        MakeWeapon(Properties.LocalizedStrings.SBL3, 30, false,  8.0, 0.95, metals.Skip(1).ToList()),
                    }                    
                ),
                new(Definitions.ItemFamilies.LongBlade, Properties.LocalizedStrings.LBL,
                    new()
                    {
                        MakeWeapon(Properties.LocalizedStrings.LBL0,  0,  true,  7.0, 1.4 , simple),
                        MakeWeapon(Properties.LocalizedStrings.LBL1, 10, false, 11.0, 1.5 , metals),
                        MakeWeapon(Properties.LocalizedStrings.LBL2, 20,  true, 18.0, 1.85, metals),
                        MakeWeapon(Properties.LocalizedStrings.LBL3, 30,  true, 18.0, 1.65, metals.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.Axe, Properties.LocalizedStrings.AXE,
                    new()
                    {
                        MakeWeapon(Properties.LocalizedStrings.AXE0,  0, false,  5.0, 1.25, simple),
                        MakeWeapon(Properties.LocalizedStrings.AXE1, 10, false,  9.0, 1.25, metals),
                        MakeWeapon(Properties.LocalizedStrings.AXE2, 20,  true, 18.0, 1.85, metals),
                        MakeWeapon(Properties.LocalizedStrings.AXE3, 30, false, 12.0, 1.35, metals.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.Blunt, Properties.LocalizedStrings.BLN,
                    new()
                    {
                        MakeWeapon(Properties.LocalizedStrings.BLN0,  0, false,  7.0, 1.65, simple),
                        MakeWeapon(Properties.LocalizedStrings.BLN1, 10, false, 10.0, 1.4 , metals),
                        MakeWeapon(Properties.LocalizedStrings.BLN2, 20,  true, 19.0, 1.95, metals),
                        MakeWeapon(Properties.LocalizedStrings.BLN3, 30, false, 15.0, 1.65, metals.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.Polearm, Properties.LocalizedStrings.POL,
                    new()
                    {
                        MakeWeapon(Properties.LocalizedStrings.POL0,  0,  true, 11.0, 2.1 , simple),
                        MakeWeapon(Properties.LocalizedStrings.POL1, 10,  true, 20.0, 2.15, woods),
                        MakeWeapon(Properties.LocalizedStrings.POL2, 20,  true, 13.0, 1.4 , woods),
                        MakeWeapon(Properties.LocalizedStrings.POL3, 30,  true, 26.0, 2.3 , woods.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.LightArmor, Properties.LocalizedStrings.LAR,
                    new()
                    {
                        MakeArmor(Properties.LocalizedStrings.LAR0,  0, EquipmentSlot.Chest,  10.0,  6.0, simple),
                        MakeArmor(Properties.LocalizedStrings.LAR1, 15, EquipmentSlot.Chest,  14.0,  6.0, leathers),
                        MakeArmor(Properties.LocalizedStrings.LAR2, 16, EquipmentSlot.Head,   10.0,  4.0, leathers),
                        MakeArmor(Properties.LocalizedStrings.LAR3, 13, EquipmentSlot.Arms,   8.0,  2.5, leathers),
                        MakeArmor(Properties.LocalizedStrings.LAR4, 14, EquipmentSlot.Legs,   8.0,  2.5, leathers),
                        MakeArmor(Properties.LocalizedStrings.LAR5, 17, EquipmentSlot.Hand,   10.0,  3.0, woods),
                        MakeArmor(Properties.LocalizedStrings.LAR6, 25, EquipmentSlot.Chest,  16.0,  6.0, leathers),
                        MakeArmor(Properties.LocalizedStrings.LAR7, 26, EquipmentSlot.Head,   12.0,  4.0, leathers),
                        MakeArmor(Properties.LocalizedStrings.LAR8, 23, EquipmentSlot.Arms,   10.0,  2.5, leathers),
                        MakeArmor(Properties.LocalizedStrings.LAR9, 24, EquipmentSlot.Legs,   10.0,  2.5, leathers),
                        MakeArmor(Properties.LocalizedStrings.LAR10,27, EquipmentSlot.Hand,   12.0,  3.0, woods),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyArmor, Properties.LocalizedStrings.HAR,
                    new()
                    {
                        MakeArmor(Properties.LocalizedStrings.HAR0,  0, EquipmentSlot.Chest,  16.0, 12.0, simple),
                        MakeArmor(Properties.LocalizedStrings.HAR1, 15, EquipmentSlot.Chest,  22.0, 12.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR2, 16, EquipmentSlot.Head,   16.0,  8.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR3, 13, EquipmentSlot.Arms,   13.0,  5.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR4, 14, EquipmentSlot.Legs,   13.0,  5.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR5, 17, EquipmentSlot.Hand,   16.0,  6.0, woods),
                        MakeArmor(Properties.LocalizedStrings.HAR6, 25, EquipmentSlot.Chest,  26.0, 12.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR7, 26, EquipmentSlot.Head,   19.0,  8.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR8, 23, EquipmentSlot.Arms,   16.0,  5.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR9, 24, EquipmentSlot.Legs,   16.0,  5.0, metals),
                        MakeArmor(Properties.LocalizedStrings.HAR10,27, EquipmentSlot.Hand,   19.0,  6.0, woods),
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
