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
                var slots = description.Equippable.Slots.Select(s => EquipSlot.Parse(s)).ToList();
                item.AddComponent(new EquippableComponent(
                    slots,
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

            // Add drop level
            item.AddComponent(new LevelComponent() { Level = GetItemDropLevel(itemIdentifier) });

            DetermineTags(item);
            UpdateItemValue(item);
            UpdateReforgingCost(item);


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
            double baseValue = 5.0;
            int tier = (int)Math.Sqrt(id.GetGenusDescription().DropLevel);
            int rarity = id.RarityLevel;
            double matMulti = id.GetMaterial().PowerMultiplier;

            int value = (int)Math.Ceiling(baseValue * tier * matMulti * Math.Pow(1.25, rarity));
            item.AddComponent(new ItemValueComponent() { Value = value });
        }

        public static void UpdateReforgingCost(Entity item)
        {
            ItemIdentifier id = item.GetComponent<ItemComponent>()?.Code ?? throw new Exception($"Entity {item.GetName()} is not an item");
            double baseCost = 10.0;
            int tier = (int)Math.Sqrt(id.GetGenusDescription().DropLevel);
            double matMulti = id.GetMaterial().PowerMultiplier;

            int totalCost = (int)Math.Ceiling(baseCost * (tier + 1) * matMulti);
            item.AddComponent(new ItemReforgeableComponent() { Cost = totalCost });
        }

        public static void DetermineTags(Entity item)
        {
            var tagsComp = new TagsComponent();

            ItemIdentifier code = item.GetComponent<ItemComponent>()!.Code; // CornerCut: better pass an item...
            tagsComp.AddTag(code.FamilyId);
            if (code.MaterialId != null)
            {
                tagsComp.AddTag(code.MaterialId);
            }
            if (item.HasComponent<ItemRarityComponent>())
            {
                tagsComp.AddTag($"+{item.GetComponent<ItemRarityComponent>()!.RarityLevel}");
            }
            
            var slots = item.GetRequiredSlots();
            foreach (var slot in slots.Where(s => s != EquipmentSlot.Hand))
            {
                tagsComp.AddTag(slot.ToString());
            }
            int hands = slots.Count(s => s == EquipmentSlot.Hand);
            if (hands > 0)
            {
                tagsComp.AddTag($"{hands}H"); // CornerCut: should be based on constant
            }

            item.AddComponent(tagsComp);
        }

        public static int GetItemDropLevel(ItemIdentifier id)
        {
            int genusLevel = id.GetGenusDescription().DropLevel;
            int materialLevel = id.GetMaterial().MinimumLevel;
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
