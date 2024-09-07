using MiniECS;
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

            // Apply rarity
            if (blueprint.Rarity > 0)
            {
                SetItemRarity(item, blueprint.Rarity);
            }

            // Add drop level
            item.AddComponent(new LevelComponent() { Level = GetItemDropLevel(blueprint) });

            DetermineTags(item);
            UpdateItemValue(item);
            UpdateReforgingCost(item);


            return item;
        }

        public static void SetItemRarity(Entity item, int rarityLevel)
        {
            var itemComp = item.GetComponent<ItemComponent>() ?? throw new Exception($"Entity {item.GetName()} is not an item");
            itemComp.Blueprint = itemComp.Blueprint with { Rarity = rarityLevel };
            item.AddComponent(new ItemRarityComponent(rarityLevel));
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
            double baseValue = 5.0;
            int tier = (int)Math.Sqrt(blueprint.GetGenusDescription().DropLevel);
            int rarity = blueprint.Rarity;
            double matMulti = blueprint.GetMaterial().PowerMultiplier;

            int value = (int)Math.Ceiling(baseValue * tier * matMulti * Math.Pow(1.25, rarity));
            item.AddComponent(new ItemValueComponent() { Value = value });
        }

        public static void UpdateReforgingCost(Entity item)
        {
            ItemBlueprint blueprint = item.GetComponent<ItemComponent>()?.Blueprint ?? throw new Exception($"Entity {item.GetName()} is not an item");
            double baseCost = 10.0;
            int tier = (int)Math.Sqrt(blueprint.GetGenusDescription().DropLevel);
            double matMulti = blueprint.GetMaterial().PowerMultiplier;

            int totalCost = (int)Math.Ceiling(baseCost * (tier + 1) * matMulti);
            item.AddComponent(new ItemReforgeableComponent() { Cost = totalCost });
        }

        public static void DetermineTags(Entity item)
        {
            var tagsComp = new TagsComponent();

            ItemBlueprint blueprint = item.GetComponent<ItemComponent>()!.Blueprint; // CornerCut: better pass an item...
            tagsComp.AddTag(blueprint.FamilyId);
            tagsComp.AddTag(blueprint.GetMaterial().Name);
            tagsComp.AddTag(blueprint.GetFamilyDescription().RelatedAbilityId);

            if (item.HasComponent<ItemRarityComponent>())
            {
                tagsComp.AddTag($"{Tags.RarityPrefix}{blueprint.Rarity}");
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

        public static int GetRandomRarity(int itemLevel, double rarityMultiplier)
        {
            int n = ItemKingdom.Rarities.Count;
            double[] weights = new double[n];

            // Build list of weights in reverse order
            // This ensures that low rarities get 'pushed' out of range first at high bonuses
            for (int i = 0; i < n; i++)
            {
                double weight = rarityMultiplier / ItemKingdom.Rarities[i].InverseWeight;
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

        public static List<double> GetRarityWeights(int itemLevel, double rarityMultiplier)
        {
            int n = ItemKingdom.Rarities.Count;
            double[] weights = new double[n + 1]; // +1 for rarity 0
            double remaining = 1.0;

            // Build list of weights in reverse order
            // This ensures that low rarities get 'pushed' out of range first at high bonuses
            for (int i = n - 1; i >= 0; i--)
            {
                double weight = Math.Min(rarityMultiplier / ItemKingdom.Rarities[i].InverseWeight, remaining);
                if (itemLevel < ItemKingdom.Rarities[i].MinLevel)
                    weight = 0.0;
                remaining -= weight;
                weights[i + 1] = weight;
            }
            weights[0] = remaining;
            return weights.ToList();
        }

        private static void CalculateItemStats(Entity item)
        {
            const double rarityScaling = 1.25;
            var itemComp = item.GetComponent<ItemComponent>() 
                ?? throw new Exception($"Entity {item.GetName()} is not an item");
            var description = itemComp.Blueprint.GetGenusDescription();
            int rarityLevel = item.GetComponent<ItemRarityComponent>()?.RarityLevel ?? 0;
            double materialMulti = itemComp.Blueprint.GetMaterial().PowerMultiplier;

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
            var blueprint = itemComp.Blueprint;
            var name = $"{blueprint.GetMaterial().Name} {blueprint.GetGenusDescription().Name}{(blueprint.Rarity > 0 ? $" + {blueprint.Rarity}" : "")}";

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
            //Console.WriteLine($"Expanding item code: {code}");
            return MakeItem(code);
        }

        public static string? GenerateItemCode(Entity item)
        {
            return item.GetBlueprintCode();
        }
    }
}
