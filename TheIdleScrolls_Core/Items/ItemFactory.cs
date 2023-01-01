using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
                foreach (var genus in family.Genera)
                {
                    items.Add(new ItemDescription(family.Name, genus));
                }
            }
            return items;
        }

        public static Entity MakeItem(ItemDescription description)
        {
            Entity item = new();
            item.AddComponent(new NameComponent(description.Genus));
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

            return MakeItem(description);
        }

        public static string? GetItemCode(Entity item)
        {
            var itemComp = item.GetComponent<ItemComponent>();
            if (itemComp == null)
                return null;
            string familyName = itemComp.FamilyName;
            string genusName = itemComp.GenusName;
            foreach (var family in ItemKingdom.Families)
            {
                if (family.Name != familyName)
                    continue;
                for (int i = 0; i < family.Genera.Count; i++)
                {
                    if (family.Genera[i].Name != genusName)
                        continue;
                    return family.Id + i.ToString();
                }
            }
            return null;
        }

        public static string? GetItemCode(ItemDescription description)
        {
            var family = ItemKingdom.Families.Find(f => f.Name == description.Family);
            if (family == null)
                return null;
            var familyId = family.Id;
            var genusIndex = family.Genera.FindIndex(g => g.Name == description.Genus);
            if (genusIndex == -1)
                return null;
            return familyId + genusIndex.ToString();
        }

        public static List<string> GetAllItemFamilyIds()
        {
            return ItemKingdom.Families.Select(w => w.Id).ToList();
        }

        public static string? GetItemFamilyName(string id)
        {
            return ItemKingdom.Families.Where(w => w.Id == id).FirstOrDefault()?.Name;
        }

        public static string? GetItemFamilyIdFromName(string name)
        {
            return ItemKingdom.Families.Where(w => w.Name == name).FirstOrDefault()?.Id;
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
            return GetItemCode(item);
        }
    }
}
