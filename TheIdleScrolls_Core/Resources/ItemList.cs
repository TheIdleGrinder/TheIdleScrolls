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
                new(Definitions.ItemFamilies.ShortBlade,
                    new()
                    {
                        MakeWeapon( 0, false,  4.0, 1.0 , simple),
                        MakeWeapon(10, false,  5.0, 0.75, metals),
                        MakeWeapon(20, false,  8.0, 1.05, metals),
                        MakeWeapon(30, false,  8.0, 0.95, metals.Skip(1).ToList()),
                    }                    
                ),
                new(Definitions.ItemFamilies.LongBlade,
                    new()
                    {
                        MakeWeapon( 0,  true,  7.0, 1.4 , simple),
                        MakeWeapon(10, false, 11.0, 1.5 , metals),
                        MakeWeapon(20,  true, 18.0, 1.85, metals),
                        MakeWeapon(30,  true, 18.0, 1.65, metals.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.Axe,
                    new()
                    {
                        MakeWeapon( 0, false,  5.0, 1.25, simple),
                        MakeWeapon(10, false,  9.0, 1.25, metals),
                        MakeWeapon(20,  true, 18.0, 1.85, metals),
                        MakeWeapon(30, false, 12.0, 1.35, metals.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.Blunt,
                    new()
                    {
                        MakeWeapon( 0, false,  7.0, 1.65, simple),
                        MakeWeapon(10, false, 10.0, 1.4 , metals),
                        MakeWeapon(20,  true, 19.0, 1.95, metals),
                        MakeWeapon(30, false, 15.0, 1.65, metals.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.Polearm,
                    new()
                    {
                        MakeWeapon( 0,  true, 11.0, 2.1 , simple),
                        MakeWeapon(10,  true, 20.0, 2.15, woods),
                        MakeWeapon(20,  true, 13.0, 1.4 , woods),
                        MakeWeapon(30,  true, 26.0, 2.3 , woods.Skip(1).ToList()),
                    }
                ),
                new(Definitions.ItemFamilies.LightArmor,
                    new()
                    {
                        MakeArmor( 0, EquipmentSlot.Chest,  10.0,  6.0, simple),
                        MakeArmor(15, EquipmentSlot.Chest,  14.0,  6.0, leathers),
                        MakeArmor(16, EquipmentSlot.Head,   10.0,  4.0, leathers),
                        MakeArmor(13, EquipmentSlot.Arms,   8.0,  2.5, leathers),
                        MakeArmor(14, EquipmentSlot.Legs,   8.0,  2.5, leathers),
                        MakeArmor(17, EquipmentSlot.Hand,   10.0,  3.0, woods),
                        MakeArmor(25, EquipmentSlot.Chest,  16.0,  6.0, leathers),
                        MakeArmor(26, EquipmentSlot.Head,   12.0,  4.0, leathers),
                        MakeArmor(23, EquipmentSlot.Arms,   10.0,  2.5, leathers),
                        MakeArmor(24, EquipmentSlot.Legs,   10.0,  2.5, leathers),
                        MakeArmor(27, EquipmentSlot.Hand,   12.0,  3.0, woods),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyArmor,
                    new()
                    {
                        MakeArmor( 0, EquipmentSlot.Chest,  16.0, 12.0, simple),
                        MakeArmor(15, EquipmentSlot.Chest,  22.0, 12.0, metals),
                        MakeArmor(16, EquipmentSlot.Head,   16.0,  8.0, metals),
                        MakeArmor(13, EquipmentSlot.Arms,   13.0,  5.0, metals),
                        MakeArmor(14, EquipmentSlot.Legs,   13.0,  5.0, metals),
                        MakeArmor(17, EquipmentSlot.Hand,   16.0,  6.0, woods),
                        MakeArmor(25, EquipmentSlot.Chest,  26.0, 12.0, metals),
                        MakeArmor(26, EquipmentSlot.Head,   19.0,  8.0, metals),
                        MakeArmor(23, EquipmentSlot.Arms,   16.0,  5.0, metals),
                        MakeArmor(24, EquipmentSlot.Legs,   16.0,  5.0, metals),
                        MakeArmor(27, EquipmentSlot.Hand,   19.0,  6.0, woods),
                    }
                )
            };
        }

        private static ItemGenusDescription MakeItemDescription(
            int level, List<EquipmentSlot> slots, WeaponGenus? weapon, ArmorGenus? armor, double encumbrance, List<MaterialId> materials)
        {
            return new(new(slots, encumbrance), weapon, armor, level, materials);
        }

        private static ItemGenusDescription MakeWeapon(int level, bool twohanded, double damage, double cooldown, List<MaterialId> materials)
        {
            List<EquipmentSlot> slots = Enumerable.Repeat(EquipmentSlot.Hand, twohanded ? 2 : 1).ToList();
            return MakeItemDescription(level, slots, new(damage, cooldown), null, 0.0, materials);
        }

        private static ItemGenusDescription MakeArmor(int level, EquipmentSlot slot, double armor, double encumbrance, List<MaterialId> materials)
        {
            return MakeItemDescription(level, new() { slot }, null, new(armor, 0.0), encumbrance, materials);
        }
    }
}
