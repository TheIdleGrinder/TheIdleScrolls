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

        public static List<ItemDescription> GetAllItemDescriptions()
        {
            List<ItemDescription> items = new();
            foreach (var family in ItemKingdom.Families)
            {
                for (int i = 0; i < family.Genera.Count; i++)
                {
                    items.Add(new ItemDescription(new ItemIdentifier(family.Id, i), family.Genera[i]));
                }
            }
            return items;
        }

        private static Entity MakeItem(ItemDescription description)
        {
            Entity item = new();
            item.AddComponent(new NameComponent(description.Identifier.GenusId.Localize()));
            item.AddComponent(new ItemComponent(GetItemCode(description)!));
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
            return item;
        }

        public static Entity? MakeItem(ItemIdentifier itemCode)
        {
            var description = itemCode.GetItemDescription();
            if (description == null)
                return null;

            var item = MakeItem(description);
            if (itemCode.RarityLevel > 0)
            {
                SetItemRarity(item, itemCode.RarityLevel);
            }

            return item;
        }

        public static void SetItemRarity(Entity item, int rarityLevel)
        {
            var itemComp = item.GetComponent<ItemComponent>() ?? throw new Exception($"Entity {item.GetName()} is not an item");
            itemComp.Code.RarityLevel = rarityLevel;
            item.AddComponent(new ItemRarityComponent(rarityLevel));
            CalculateItemStats(item);
            UpdateItemName(item);
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
            var description = ItemKingdom.GetDescriptionByIdAndIndex(itemComp.Code.FamilyId, itemComp.Code.GenusIndex);
            if (description == null)
                throw new Exception($"Invalid item code: {itemComp.Code}");

            int rarityLevel = item.GetComponent<ItemRarityComponent>()?.RarityLevel ?? 0;

            if (description.Weapon != null)
            {
                double dmg = Math.Round(description.Weapon.BaseDamage * Math.Pow(rarityScaling, rarityLevel), 1);
                item.AddComponent(new WeaponComponent(dmg, description.Weapon.BaseCooldown));
            }

            if (description.Armor != null)
            {
                double armor = Math.Round(description.Armor.BaseArmor * Math.Pow(rarityScaling, rarityLevel), 1);
                double evasion = Math.Round(description.Armor.BaseEvasion * Math.Pow(rarityScaling, rarityLevel), 1);
                item.AddComponent(new ArmorComponent(armor, evasion));
            }
        }

        private static void UpdateItemName(Entity item)
        {
            var itemComp = item.GetComponent<ItemComponent>() ?? throw new Exception($"Entity {item.GetName()} is not an item");
            var name = itemComp.GenusName;
            
            var rarity = itemComp.Code.RarityLevel;
            if (rarity > 0)
            {
                name += $" + {rarity}";
            }

            item.AddComponent(new NameComponent(name));
        }

        public static string? GetItemCode(Entity item)
        {
            var itemComp = item.GetComponent<ItemComponent>();
            if (itemComp == null)
                return null;
            return itemComp.Code.Code;
        }

        public static string? GetItemCode(ItemDescription description)
        {
            return description.Identifier.Code;
        }

        public static List<string> GetAllItemFamilyIds()
        {
            return ItemKingdom.Families.Select(w => w.Id).ToList();
        }

        public static string? GetItemFamilyName(string id)
        {
            return ItemKingdom.Families.Where(w => w.Id == id).FirstOrDefault()?.Id.Localize();
        }

        public static string? GetItemFamilyIdFromName(string name)
        {
            return ItemKingdom.Families.Where(w => w.Id.Localize() == name).FirstOrDefault()?.Id;
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
