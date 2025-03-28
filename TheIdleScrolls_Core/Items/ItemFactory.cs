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
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Resources;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Items
{
    public class ItemFactory
    {
        public ItemFactory()
        {

        }

        public static Entity? MakeItem(ItemBlueprint blueprint)
        {
            var description = ItemKingdom.GetGenusDescription(blueprint);
            if (description == null)
                return null;

            if (!description.ValidMaterials.Contains(blueprint.MaterialId))
                return null;

            // Build base item
            Entity item = new();
            item.AddComponent(new NameComponent(description.Name ?? "??"));
            item.AddComponent(new ItemComponent(blueprint));
            if (description.Equippable != null)
            {
                item.AddComponent(new EquippableComponent(
                    description.Equippable.Slots,
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
            var material = ItemKingdom.Materials.Where(m => m.Id == blueprint.MaterialId).First();
            SetItemMaterial(item, material);

            // Apply quality
            if (blueprint.Quality > 0)
            {
                SetItemQuality(item, blueprint.Quality);
            }

            // Add drop level
            item.AddComponent(new LevelComponent() { Level = GetItemDropLevel(blueprint) });

            DetermineTags(item);
            UpdateItemValue(item);
            UpdateRefiningCost(item);


            return item;
        }

        public static void SetItemQuality(Entity item, int quality)
        {
            var itemComp = item.GetComponent<ItemComponent>() ?? throw new Exception($"Entity {item.GetName()} is not an item");
            itemComp.Blueprint = itemComp.Blueprint with { Quality = quality };
            item.AddComponent(new ItemQualityComponent(quality));
            CalculateItemStats(item);
            UpdateItemName(item);
            UpdateItemValue(item);
        }

        public static void SetItemMaterial(Entity item, ItemMaterial material)
        {
            item.AddComponent(new ItemMaterialComponent(material.Name, material.Tier));
            CalculateItemStats(item);
            UpdateItemName(item);
            UpdateItemValue(item);
        }

        public static void UpdateItemValue(Entity item)
        {
            ItemBlueprint blueprint = item.GetComponent<ItemComponent>()?.Blueprint ?? throw new Exception($"Entity {item.GetName()} is not an item");
            int value = 0;
            if (blueprint.MaterialId != MaterialId.Simple)
            {
                int tier = (int)Math.Sqrt(blueprint.GetGenusDescription().DropLevel + 10); // +10 because first tier has drop level 0 (+10 from material) 
                double matMulti = blueprint.GetMaterial().PowerMultiplier;
                value = (int)Math.Ceiling(Stats.ItemBaseValue * tier * matMulti * Math.Pow(Stats.ItemValueQualityMultiplier, blueprint.Quality));
            }            
            item.AddComponent(new ItemValueComponent() { Value = value });
        }

        public static void UpdateRefiningCost(Entity item)
        {
            ItemBlueprint blueprint = item.GetComponent<ItemComponent>()?.Blueprint ?? throw new Exception($"Entity {item.GetName()} is not an item");
            double baseCost = Stats.CraftingBaseCost;
            int totalCost = (int)baseCost;
            if (blueprint.MaterialId != MaterialId.Simple)
            {
                //int tier = (int)Math.Sqrt(blueprint.GetGenusDescription().DropLevel + 10);
                //double matMulti = blueprint.GetMaterial().PowerMultiplier;
                //totalCost = (int)Math.Ceiling(baseCost * (tier + 1) * matMulti);
                int tier = blueprint.GetDropLevel() / 10;
                double matMulti = blueprint.GetMaterial().PowerMultiplier;
                double typeMulti = item.IsWeapon() ? Stats.CraftingCostWeaponMultiplier : 1.0;
                totalCost = (int)Math.Ceiling(Stats.CraftingBaseCost * typeMulti 
                    * Math.Pow(tier, Stats.CraftingCostTierExponent) 
                    * Math.Pow(matMulti, Stats.CraftingCostMaterialExponent));
            }
            item.AddComponent(new ItemRefinableComponent() { Cost = totalCost });
        }

        public static void DetermineTags(Entity item)
        {
            var tagsComp = new TagsComponent();

            ItemBlueprint blueprint = item.GetComponent<ItemComponent>()!.Blueprint; // CornerCut: better pass an item...
            tagsComp.AddTag(blueprint.FamilyId);
            tagsComp.AddTag(blueprint.GetMaterial().Name);
            tagsComp.AddTag(blueprint.GetFamilyDescription().RelatedAbilityId);

            if (item.HasComponent<ItemQualityComponent>())
            {
                tagsComp.AddTag($"{Tags.QualityPrefix}{blueprint.Quality}");
            }
            if (item.IsWeapon())
            {
                tagsComp.AddTags(new List<string>() { Tags.Weapon, Tags.Melee }); // CornerCut: no ranged weapons exist yet
            }
            if (item.IsArmor())
            {
                tagsComp.AddTag(Tags.Armor);
            }
            
            var slots = item.GetRequiredSlots();
            foreach (var slot in slots.Where(s => s != EquipmentSlot.Hand))
            {
                tagsComp.AddTag(slot.ToString());
            }
            int hands = slots.Count(s => s == EquipmentSlot.Hand);
            if (hands > 0)
            {
                if (item.IsShield())
                {
                    tagsComp.AddTag(Tags.Shield);
                }
                else // Weapon
                {
                    tagsComp.AddTag($"{hands}{Tags.HandSuffix}");
                }
                
            }

            item.AddComponent(tagsComp);
        }

        public static int GetItemDropLevel(ItemBlueprint blueprint)
        {
            int genusLevel = ItemKingdom.GetGenusDescription(blueprint)?.DropLevel ?? 0;
            int materialLevel = ItemKingdom.GetMaterial(blueprint.MaterialId)?.MinimumLevel ?? 0;  //blueprint..MinimumLevel;
            return genusLevel + materialLevel;
        }

        public static int GetRandomQuality(int itemLevel, double rarityMultiplier)
        {
            int n = ItemKingdom.Qualities.Count;
            double[] weights = new double[n];

            // Build list of weights in reverse order
            // This ensures that low rarities get 'pushed' out of range first at high bonuses
            for (int i = 0; i < n; i++)
            {
                double weight = rarityMultiplier / ItemKingdom.Qualities[i].InverseWeight;
                if (itemLevel < ItemKingdom.Qualities[i].MinLevel)
                    weight = 0.0;
                weights[n - i - 1] = weight;
            }

            double draw = new Random().NextDouble();
            int quality = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                draw -= weights[i];
                if (draw < 0)
                {
                    quality = n - i; // i'th highest quality => level == n - i (instead of n - i - 1 since quality levels start at 1)
                    break;
                }
            }
            return quality;
        }

        public static List<double> GetQualityWeights(int itemLevel, double rarityMultiplier)
        {
            int n = ItemKingdom.Qualities.Count;
            double[] weights = new double[n + 1]; // +1 for quality 0
            double remaining = 1.0;

            // Build list of weights in reverse order
            // This ensures that low rarities get 'pushed' out of range first at high bonuses
            for (int i = n - 1; i >= 0; i--)
            {
                double weight = Math.Min(rarityMultiplier / ItemKingdom.Qualities[i].InverseWeight, remaining);
                if (itemLevel < ItemKingdom.Qualities[i].MinLevel)
                    weight = 0.0;
                remaining -= weight;
                weights[i + 1] = weight;
            }
            weights[0] = remaining;
            return weights.ToList();
        }

        private static void CalculateItemStats(Entity item)
        {
            var itemComp = item.GetComponent<ItemComponent>() 
                ?? throw new Exception($"Entity {item.GetName()} is not an item");
            var description = itemComp.Blueprint.GetGenusDescription();
            int qualityLevel = item.GetComponent<ItemQualityComponent>()?.Quality ?? 0;
            double materialMulti = itemComp.Blueprint.GetMaterial().PowerMultiplier;

            if (description.Weapon != null)
            {
                double dmg = Math.Round(description.Weapon.BaseDamage * Math.Pow(Stats.QualityMultiplier, qualityLevel) * materialMulti, 1);
                item.AddComponent(new WeaponComponent(dmg, description.Weapon.BaseCooldown));
            }

            if (description.Armor != null)
            {
                double armor = Math.Round(description.Armor.BaseArmor * Math.Pow(Stats.QualityMultiplier, qualityLevel) * materialMulti, 1);
                double evasion = Math.Round(description.Armor.BaseEvasion * Math.Pow(Stats.QualityMultiplier, qualityLevel) * materialMulti, 1);
                item.AddComponent(new ArmorComponent(armor, evasion));
            }
        }

        private static void UpdateItemName(Entity item)
        {
            var itemComp = item.GetComponent<ItemComponent>() ?? throw new Exception($"Entity {item.GetName()} is not an item");
            var blueprint = itemComp.Blueprint;
            var name = $"{blueprint.GetMaterial().Name} {blueprint.GetGenusDescription().Name}{(blueprint.Quality > 0 ? $" + {blueprint.Quality}" : "")}";

            item.AddComponent(new NameComponent(name));
        }

        public static List<string> GetAllItemFamilyIds()
        {
            return ItemKingdom.Families.Select(w => w.Id).ToList();
        }

        public static Entity? ExpandCode(string code)
        {
            return ExpandCode(ItemBlueprint.Parse(code));
        }

        public static Entity? ExpandCode(ItemBlueprint code)
        {
            return MakeItem(code);
        }

        public static string? GenerateItemCode(Entity item)
        {
            return item.GetBlueprintCode();
        }
    }
}
