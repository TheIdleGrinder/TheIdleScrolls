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
        static ItemKingdomDescription? s_itemKingdom = null;

        public ItemFactory()
        {
            try
            {
                if (s_itemKingdom == null)
                    s_itemKingdom = ResourceAccess.ParseResourceFile<ItemKingdomDescription>("TheIdleScrolls_Core", "Items.json");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public static List<ItemDescription> GetAllItemDescriptions(ItemKingdomDescription itemKingdom)
        {
            List<ItemDescription> items = new();
            foreach (var family in itemKingdom.Families)
            {
                foreach (var genus in family.Genera)
                {
                    items.Add(new ItemDescription
                    {
                        Family = family.Name,
                        Genus = genus.Name,
                        Weapon = genus.Weapon,
                        Armor = genus.Armor
                    });
                }
            }
            return items;
        }

        public static Entity MakeItem(ItemDescription description)
        {
            Entity item = new();
            item.AddComponent(new NameComponent(description.Genus));
            item.AddComponent(new ItemComponent(description.Family, description.Genus));
            if (description.Slot != null)
            {
                item.AddComponent(new EquippableComponent(description.Slot.Slot));
            }
            if (description.Weapon != null)
            {
                item.AddComponent(new WeaponComponent(
                    description.Family,
                    description.Genus,
                    description.Weapon.BaseDamage,
                    description.Weapon.BaseCooldown));
            }
            if (description.Armor != null)
            {
                item.AddComponent(new ArmorComponent(description.Armor.BaseArmor));
            }
            return item;
        }

        public static Entity? MakeItem(string fullId, ItemKingdomDescription items)
        {
            string familyId = fullId.Substring(0, 3);
            int genusIndex = int.Parse(fullId.Substring(3));
            var description = items.GetDescriptionByIdAndIndex<ItemDescription>(familyId, genusIndex);
            if (description == null)
                return null;

            return MakeItem(description);
        }

        public static string? GetIdString(Entity item, ItemKingdomDescription items)
        {
            var weaponComp = item.GetComponent<WeaponComponent>();
            if (weaponComp == null)
                return null;
            string familyName = weaponComp.Family;
            string genusName = weaponComp.Genus;
            foreach (var family in items.Families)
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

        public List<ItemDescription> GetAllItemDescriptions()
        {
            if (s_itemKingdom == null)
                return new();
            return GetAllItemDescriptions(s_itemKingdom);
        }

        public List<string> GetAllItemFamilyIds()
        {
            if (s_itemKingdom == null)
                return new();
            return s_itemKingdom.Families.Select(w => w.Id).ToList();
        }

        public string? GetItemFamilyName(string id)
        {
            if (s_itemKingdom == null)
                return null;
            return s_itemKingdom.Families.Where(w => w.Id == id).FirstOrDefault()?.Name;
        }

        public string? GetItemFamilyIdFromName(string name)
        {
            if (s_itemKingdom == null)
                return null;
            return s_itemKingdom.Families.Where(w => w.Name == name).FirstOrDefault()?.Id;
        }

        public Entity? ExpandCode(string code)
        {
            if (s_itemKingdom == null)
                return null;
            return MakeItem(code, s_itemKingdom);
        }

        public string? GenerateItemCode(Entity item)
        {
            if (s_itemKingdom == null)
                return null;
            return GetIdString(item, s_itemKingdom);
        }
    }
}
