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

        public static List<WeaponDescription> GetAllWeaponDescriptions(ItemKingdomDescription items)
        {
            List<WeaponDescription> weapons = new();
            foreach (var family in items.Weapons)
            {
                foreach (var genus in family.Genera)
                {
                    weapons.Add(new WeaponDescription
                    {
                        Family = family.Name,
                        Genus = genus.Name,
                        BaseDamage = genus.BaseDamage,
                        BaseCooldown = genus.BaseCooldown
                    });
                }
            }
            return weapons;
        }

        public static Entity MakeWeapon(WeaponDescription description)
        {
            Entity weapon = new();
            weapon.AddComponent(new NameComponent(description.Genus));
            weapon.AddComponent(new ItemComponent(ItemPhylum.Weapon, description.Family));
            weapon.AddComponent(new WeaponComponent(
                description.Family,
                description.Genus,
                description.BaseDamage,
                description.BaseCooldown));
            return weapon;
        }

        public static Entity? MakeWeapon(string fullId, ItemKingdomDescription items)
        {
            string familyId = fullId.Substring(0, 3);
            int genusIndex = int.Parse(fullId.Substring(3));
            var description = items.GetDescriptionByIdAndIndex<WeaponDescription>(familyId, genusIndex);
            if (description == null)
                return null;

            return MakeWeapon(description);
        }

        public static string? GetIdString(Entity item, ItemKingdomDescription items)
        {
            var weaponComp = item.GetComponent<WeaponComponent>();
            if (weaponComp == null)
                return null;
            string familyName = weaponComp.Family;
            string genusName = weaponComp.Genus;
            foreach (var family in items.Weapons)
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

        public List<WeaponDescription> GetAllWeaponDescriptions()
        {
            if (s_itemKingdom == null)
                return new();
            return GetAllWeaponDescriptions(s_itemKingdom);
        }

        public List<string> GetAllItemFamilyIds()
        {
            if (s_itemKingdom == null)
                return new();
            return s_itemKingdom.Weapons.Select(w => w.Id).ToList(); // CornerCut: Assumes single phylum
        }

        public string? GetItemFamilyName(string id)
        {
            if (s_itemKingdom == null)
                return null;
            return s_itemKingdom.Weapons.Where(w => w.Id == id).FirstOrDefault()?.Name; // CornerCut: Assumes single phylum
        }

        public string? GetItemFamilyIdFromName(string name)
        {
            if (s_itemKingdom == null)
                return null;
            return s_itemKingdom.Weapons.Where(w => w.Name == name).FirstOrDefault()?.Id; // CornerCut: Assumes single phylum
        }

        public Entity? ExpandCode(string code)
        {
            if (s_itemKingdom == null)
                return null;
            return MakeWeapon(code, s_itemKingdom);
        }

        public string? GenerateItemCode(Entity item)
        {
            if (s_itemKingdom == null)
                return null;
            return GetIdString(item, s_itemKingdom);
        }
    }

    public class WeaponDescription
    {
        public string Family { get; set; }
        public string Genus { get; set; }
        public double BaseDamage { get; set; }
        public double BaseCooldown { get; set; }

        public WeaponDescription()
        {
            Family = "";
            Genus = "";
            BaseDamage = 1.0;
            BaseCooldown = 1.0;
        }
    }

    public class WeaponGenusDescription
    {
        public string Name { get; set; }

        public double BaseDamage { get; set; }

        public double BaseCooldown { get; set; }

        public WeaponGenusDescription()
        {
            Name = "";
            BaseDamage = 1.0;
            BaseCooldown = 1.0;
        }
    }

    public class ItemFamilyDescription<T> where T : WeaponGenusDescription
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public List<T> Genera { get; set; }

        public ItemFamilyDescription()
        {
            Name = "";
            Id = "";
            Genera = new();
        }

        public T? GetGenusAt(int index)
        {
            return Genera.ElementAtOrDefault(index) ?? null;
        }
    }

    public class ItemKingdomDescription
    {
        public List<ItemFamilyDescription<WeaponGenusDescription>> Weapons { get; set; }

        public ItemKingdomDescription()
        {
            Weapons = new();
        }

        public T? GetDescriptionByIdAndIndex<T>(string idString, int index) where T : WeaponDescription
        {
            if (typeof(T) == typeof(WeaponDescription)) // CornerCut: Let's see how well this scales with other item types...
            {
                foreach (var family in Weapons)
                {
                    if (family.Id == idString)
                    {
                        var genus = family.GetGenusAt(index);
                        if (genus == null)
                            return null;
                        return (T)new WeaponDescription()
                        {
                            Family = family.Name,
                            Genus = genus.Name,
                            BaseCooldown = genus.BaseCooldown,
                            BaseDamage = genus.BaseDamage
                        };
                    }
                }
            }
            return null;
        }

        public string? GetFamilyIdFromItemFamilyName(string familyName)
        {
            foreach (var weapon in Weapons)
            {
                if (weapon.Name == familyName)
                    return weapon.Id;
            }
            return null;
        }
    }
}
