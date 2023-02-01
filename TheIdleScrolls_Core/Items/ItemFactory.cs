﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Items
{
    public class ItemFactory : IItemCodeExpander
    {
        private static ItemKingdomDescription? s_itemKingdom = null;

        public static ItemKingdomDescription ItemKingdom { get
            {
                s_itemKingdom ??= ResourceAccess.ParseResourceFile<ItemKingdomDescription>("TheIdleScrolls_Core", "Items.json");
                return s_itemKingdom;
            } }

        public ItemFactory()
        {

        }

        public static List<string> GetAllItemGenusCodes()
        {
            List<ItemIdentifier> codes = new();
            foreach (var family in ItemKingdom.Families)
            {
                for (int i = 0; i < family.Genera.Count; i++)
                {
                    codes.Add(new ItemIdentifier(family.Id, i));
                }
            }
            return codes.Select(c => c.Code).ToList();
        }

        public static Entity? MakeItem(ItemIdentifier itemIdentifier)
        {
            var description = itemIdentifier.GetGenusDescription();
            if (description == null)
                return null;

            // Build base item
            Entity item = new();
            item.AddComponent(new NameComponent(itemIdentifier.GenusId.Localize()));
            item.AddComponent(new ItemComponent(itemIdentifier));
            if (description.Equippable != null)
            {
                item.AddComponent(new EquippableComponent(
                    EquipSlot.Parse(description.Equippable.Slot),
                    description.Equippable.Encumbrance));
            }
            if (description.Weapon != null)
            {
                item.AddComponent(new WeaponComponent(
                    description.Weapon.BaseDamage,
                    description.Weapon.BaseCooldown));
            }
            if (description.Armor != null)
            {
                item.AddComponent(new ArmorComponent(
                    description.Armor.BaseArmor,
                    description.Armor.BaseEvasion));
            }

            // Apply material
            string? materialId = itemIdentifier.MaterialId;
            if (materialId != null)
            {
                var material = ItemKingdom.Materials.Where(m => m.Id == materialId).First();
                SetItemMaterial(item, material);
            }

            // Apply rarity
            if (itemIdentifier.RarityLevel > 0)
            {
                SetItemRarity(item, itemIdentifier.RarityLevel);
            }

            UpdateItemValue(item);

            return item;
        }

        public static void SetItemRarity(Entity item, int rarityLevel)
        {
            var itemComp = item.GetComponent<ItemComponent>() ?? throw new Exception($"Entity {item.GetName()} is not an item");
            itemComp.Code.RarityLevel = rarityLevel;
            item.AddComponent(new ItemRarityComponent(rarityLevel));
            CalculateItemStats(item);
            UpdateItemName(item);
            UpdateItemValue(item);
        }

        public static void SetItemMaterial(Entity item, ItemMaterialDescription material)
        {
            item.AddComponent(new ItemMaterialComponent(material.Id));
            CalculateItemStats(item);
            UpdateItemName(item);
            UpdateItemValue(item);
        }

        public static void UpdateItemValue(Entity item)
        {
            ItemIdentifier id = item.GetComponent<ItemComponent>()?.Code ?? throw new Exception($"Entity {item.GetName()} is not an item");
            double baseValue = 10.0;
            int tier = id.GenusIndex;
            int rarity = id.RarityLevel;
            double matMulti = id.GetMaterial().PowerMultiplier;

            int value = (int)Math.Ceiling(baseValue * tier * matMulti * Math.Pow(1.25, rarity));
            item.AddComponent(new ItemValueComponent() { Value = value });
        }

        public static int GetRandomRarity(int itemLevel, double multiplier)
        {
            int n = ItemKingdom.Rarities.Count;
            double[] weights = new double[n];

            // Build list of weights in reverse order
            // This ensures that low rarities get 'pushed' out of range first at high bonuses
            for (int i = 0; i < n; i++)
            {
                double weight = multiplier / ItemKingdom.Rarities[i].InverseWeight;
                if (itemLevel < ItemKingdom.Rarities[i].MinLevel)
                    weight = 0.0;
                weights[n - i - 1] = weight;
            }

            double draw = new Random().NextDouble();
            int rarity = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                draw -= weights[i];
                if (draw < 0)
                {
                    rarity = n - i; // i'th highest rarity => level == n - i (instead of n - i - 1 since rarities start at 1)
                    break;
                }
            }
            return rarity;
        }

        private static void CalculateItemStats(Entity item)
        {
            const double rarityScaling = 1.25;
            var itemComp = item.GetComponent<ItemComponent>();
            if (itemComp == null)
                throw new Exception($"Entity {item.GetName()} is not an item");
            var description = ItemKingdom.GetGenusDescriptionByIdAndIndex(itemComp.Code.FamilyId, itemComp.Code.GenusIndex);
            if (description == null)
                throw new Exception($"Invalid item code: {itemComp.Code}");

            int rarityLevel = item.GetComponent<ItemRarityComponent>()?.RarityLevel ?? 0;
            double materialMulti = itemComp.Code.GetMaterial().PowerMultiplier;

            if (description.Weapon != null)
            {
                double dmg = Math.Round(description.Weapon.BaseDamage * Math.Pow(rarityScaling, rarityLevel) * materialMulti, 1);
                item.AddComponent(new WeaponComponent(dmg, description.Weapon.BaseCooldown));
            }

            if (description.Armor != null)
            {
                double armor = Math.Round(description.Armor.BaseArmor * Math.Pow(rarityScaling, rarityLevel) * materialMulti, 1);
                double evasion = Math.Round(description.Armor.BaseEvasion * Math.Pow(rarityScaling, rarityLevel) * materialMulti, 1);
                item.AddComponent(new ArmorComponent(armor, evasion));
            }
        }

        private static void UpdateItemName(Entity item)
        {
            var itemComp = item.GetComponent<ItemComponent>() ?? throw new Exception($"Entity {item.GetName()} is not an item");
            var name = itemComp.Code.GetItemName();

            item.AddComponent(new NameComponent(name));
        }

        public static List<string> GetAllItemFamilyIds()
        {
            return ItemKingdom.Families.Select(w => w.Id).ToList();
        }

        public Entity? ExpandCode(string code)
        {
            return ExpandCode(new ItemIdentifier(code));
        }

        public Entity? ExpandCode(ItemIdentifier code)
        {
            return MakeItem(code);
        }

        public string? GenerateItemCode(Entity item)
        {
            return item.GetItemCode();
        }
    }
}
