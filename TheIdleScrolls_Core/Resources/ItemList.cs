using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Modifiers;

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
            List<MaterialId> cloths   = [ MaterialId.Cloth1,   MaterialId.Cloth2,   MaterialId.Cloth3,   MaterialId.Cloth4 ];

            var spellCooldownMod = (double value) => new ModifierTemplate(new Modifier("spellCooldown_", ModifierType.More, value, 
                [Tags.SpellSkill, Tags.CooldownRecovery], []), 1.1, 1.25);
            var blockMod = (double value) => new ModifierTemplate(new Modifier("blockChance_", ModifierType.AddBase, value,
                [Tags.BlockChance], []) { AlwaysPercentage = true }, 1.1, 1.25);

            return new()
            {
                new(Definitions.ItemFamilies.OneHandedAxe, Properties.Items.Family_OneHandAxe, Abilities.Axe,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_OneHandAxe0, ItemTiers.LevelT0, false,  6.0, 1.45, simple),
                        MakeWeapon(Properties.Items.Genus_OneHandAxe1, ItemTiers.LevelT1, false, 10.0, 1.4 , metals),
                        MakeWeapon(Properties.Items.Genus_OneHandAxe2, ItemTiers.LevelT3, false, 13.0, 1.5, [.. metals.Skip(2)]),
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
                        MakeWeapon(Properties.Items.Genus_OneHandMace2, ItemTiers.LevelT3, false, 16.0, 1.8 , [.. metals.Skip(2)]),
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
                        MakeWeapon(Properties.Items.Genus_TwoHandSword2, ItemTiers.LevelT3,  true, 15.0, 1.4 , [.. metals.Skip(2)]),
                    }
                ),
                new(Definitions.ItemFamilies.Spear, Properties.Items.Family_Spear, Abilities.Polearm,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_Spear0, ItemTiers.LevelT0,  true,  9.0, 2.05, simple),
                        MakeWeapon(Properties.Items.Genus_Spear1, ItemTiers.LevelT2,  true, 14.0, 1.4 , woods),
                        MakeWeapon(Properties.Items.Genus_Spear2, ItemTiers.LevelT3,  true, 25.0, 2.35, [.. woods.Skip(2)]),
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
                        MakeWeapon(Properties.Items.Genus_ShortSword2, ItemTiers.LevelT3, false,  8.0, 0.95, [.. metals.Skip(2)]),
                    }
                ),
                new(Definitions.ItemFamilies.ShortBow, Properties.Items.Family_ShortBow, Abilities.Archery,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_ShortBow0, ItemTiers.LevelT0,  true,  7.0, 1.9, simple, 7.0),
                        MakeWeapon(Properties.Items.Genus_ShortBow1, ItemTiers.LevelT1,  true, 13.0, 1.75 , woods,  9.0),
                        MakeWeapon(Properties.Items.Genus_ShortBow2, ItemTiers.LevelT3,  true, 17.0, 1.9 , [.. woods.Skip(2)], 10.0),
                    }
                )
                {
                    DropRestrictions = [DropRestrictions.Bow]
                },
                new(Definitions.ItemFamilies.LongBow, Properties.Items.Family_LongBow, Abilities.Archery,
                    new()
                    {
                        MakeWeapon(Properties.Items.Genus_LongBow1, ItemTiers.LevelT2, true, 20.0, 2.35, woods, 12.0),
                    }
                )
                {
                    DropRestrictions = [DropRestrictions.Bow]
                },
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
                        MakeArmor(Properties.Items.Genus_LightHelmet1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetHelmet, 
                            EquipmentSlot.Head,   12.0,  4.0, leathers),
                        MakeArmor(Properties.Items.Genus_LightHelmet2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetHelmet, 
                            EquipmentSlot.Head,   15.0,  4.0, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightGloves, Properties.Items.Family_LightGloves, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightGloves1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetGloves, 
                            EquipmentSlot.Arms,   9.0,  2.5, leathers),
                        MakeArmor(Properties.Items.Genus_LightGloves2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetGloves, 
                            EquipmentSlot.Arms,   11.0,  2.5, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightBoots, Properties.Items.Family_LightBoots, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightBoots1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetBoots, 
                            EquipmentSlot.Legs,   9.0,  2.5, leathers),
                        MakeArmor(Properties.Items.Genus_LightBoots2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetBoots,
                            EquipmentSlot.Legs,  11.0,  2.5, leathers),
                    }
                ),
                new(Definitions.ItemFamilies.LightShield, Properties.Items.Family_LightShield, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_LightShield0, ItemTiers.LevelT0,
                            EquipmentSlot.Hand,   4.0,  2.0, simple)
                            .WithModifiers([blockMod(0.12)]),
                        MakeArmor(Properties.Items.Genus_LightShield1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetShield, 
                            EquipmentSlot.Hand,  6.0,  3.0, woods)
                            .WithModifiers([blockMod(0.16)]),
                        MakeArmor(Properties.Items.Genus_LightShield2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetShield, 
                            EquipmentSlot.Hand,  7.0,  3.0, woods)
                            .WithModifiers([blockMod(0.2)])
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
                        MakeArmor(Properties.Items.Genus_HeavyHelmet1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetHelmet, 
                            EquipmentSlot.Head,   16.0,  11.0, metals),
                        MakeArmor(Properties.Items.Genus_HeavyHelmet2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetHelmet, 
                            EquipmentSlot.Head,   19.0,  11.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyGloves, Properties.Items.Family_HeavyGloves, Abilities.HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_HeavyGloves1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetGloves, 
                            EquipmentSlot.Arms,   13.0,  6.0, metals),
                        MakeArmor(Properties.Items.Genus_HeavyGloves2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetGloves, 
                            EquipmentSlot.Arms,   16.0,  6.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyBoots, Properties.Items.Family_HeavyBoots, Abilities.HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_HeavyBoots1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetBoots, 
                            EquipmentSlot.Legs,   13.0,  6.0, metals),
                        MakeArmor(Properties.Items.Genus_HeavyBoots2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetBoots, 
                            EquipmentSlot.Legs,   16.0,  6.0, metals),
                    }
                ),
                new(Definitions.ItemFamilies.HeavyShield, Properties.Items.Family_HeavyShield, Abilities.HeavyArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_HeavyShield0, ItemTiers.LevelT0,
                            EquipmentSlot.Hand,   6.0,  6.0, simple)
                            .WithModifiers([blockMod(0.12)]),
                        MakeArmor(Properties.Items.Genus_HeavyShield1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetShield, 
                            EquipmentSlot.Hand,   8.0,  9.0, woods)
                            .WithModifiers([blockMod(0.16)]),
                        MakeArmor(Properties.Items.Genus_HeavyShield2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetShield, 
                            EquipmentSlot.Hand,   10.0,  9.0, woods)
                            .WithModifiers([blockMod(0.20)])
                    }
                ),
                new(Definitions.ItemFamilies.ClothChest, Properties.Items.Family_ClothChest, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_ClothChest0, ItemTiers.LevelT0, EquipmentSlot.Chest,  5.0,  0.0, simple)
                            .WithModifiers([spellCooldownMod(0.08)]),
                        MakeArmor(Properties.Items.Genus_ClothChest1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetChest,
                            EquipmentSlot.Chest,  9.0,  6.0, cloths)
                            .WithModifiers([spellCooldownMod(0.1)]),
                        MakeArmor(Properties.Items.Genus_ClothChest2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetChest,
                            EquipmentSlot.Chest,  11.0,  6.0, cloths)
                            .WithModifiers([spellCooldownMod(0.12)]),
                    }
                )
                {
                    DropRestrictions = [DropRestrictions.ClothItems]
                },
                new(Definitions.ItemFamilies.ClothHelmet, Properties.Items.Family_ClothHelmet, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_ClothHelmet1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetHelmet,
                            EquipmentSlot.Head,   6.0,  0.0, cloths)
                            .WithModifiers([spellCooldownMod(0.08)]),
                        MakeArmor(Properties.Items.Genus_ClothHelmet2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetHelmet,
                            EquipmentSlot.Head,   7.0,  0.0, cloths)
                            .WithModifiers([spellCooldownMod(0.1)]),
                    }
                )
                {
                    DropRestrictions = [DropRestrictions.ClothItems]
                },
                new(Definitions.ItemFamilies.ClothGloves, Properties.Items.Family_ClothGloves, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_ClothGloves1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetGloves,
                            EquipmentSlot.Arms,   5.0,  0.0, cloths)
                            .WithModifiers([spellCooldownMod(0.08)]),
                        MakeArmor(Properties.Items.Genus_ClothGloves2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetGloves,
                            EquipmentSlot.Arms,   6.0,  0.0, cloths)
                            .WithModifiers([spellCooldownMod(0.1)]),
                    }
                )
                {
                    DropRestrictions = [DropRestrictions.ClothItems]
                },
                new(Definitions.ItemFamilies.ClothBoots, Properties.Items.Family_ClothBoots, Abilities.LightArmor,
                    new()
                    {
                        MakeArmor(Properties.Items.Genus_ClothBoots1, ItemTiers.LevelT1 + ItemTiers.LevelOffsetBoots,
                            EquipmentSlot.Legs,   5.0,  0.0, cloths)
                            .WithModifiers([spellCooldownMod(0.08)]),
                        MakeArmor(Properties.Items.Genus_ClothBoots2, ItemTiers.LevelT2 + ItemTiers.LevelOffsetBoots,
                            EquipmentSlot.Legs,   6.0,  0.0, cloths)
                            .WithModifiers([spellCooldownMod(0.1)]),
                    }
                )
                {
                    DropRestrictions = [DropRestrictions.ClothItems]
                }
            };
        }

        private static ItemGenusDescription MakeWeapon(string name, int level, bool twohanded, double damage, double attackTime, List<MaterialId> materials, double range = 0.0)
        {
            List<EquipmentSlot> slots = Enumerable.Repeat(EquipmentSlot.Hand, twohanded ? 2 : 1).ToList();
            ItemGenusDescription descr = new(name, level, materials)
            {
                Equippable = new(slots, 0.0),
                Weapon = new(new(DamageType.Physical, damage), attackTime, range),
                InherentModifiers =
                [
                    //new("+eva", Modifiers.ModifierType.AddFlat, 100.0, [Tags.EvasionRating], [])
                ]
            };
            return descr;
        }

        private static ItemGenusDescription MakeArmor(string name, int level, EquipmentSlot slot, double armor, double encumbrance, List<MaterialId> materials)
        {
            ItemGenusDescription descr = new(name, level, materials)
            {
                Equippable = new([slot], encumbrance),
                Armor = new(armor, 0.0)
            };
            return descr;
        }
    }
}
