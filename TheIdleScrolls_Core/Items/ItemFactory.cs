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
            foreach (var weaponClass in items.Weapons)
            {
                foreach (var weaponFamily in weaponClass.Families)
                {
                    weapons.Add(new WeaponDescription
                    {
                        Class = weaponClass.Name,
                        Family = weaponFamily.Name,
                        BaseDamage = weaponFamily.BaseDamage,
                        BaseCooldown = weaponFamily.BaseCooldown
                    });
                }
            }
            return weapons;
        }

        public static Entity MakeWeapon(WeaponDescription description)
        {
            Entity weapon = new();
            weapon.AddComponent(new NameComponent(description.Family));
            weapon.AddComponent(new ItemComponent(ItemPhylum.Weapon, description.Class));
            weapon.AddComponent(new WeaponComponent(
                description.Class,
                description.Family,
                description.BaseDamage,
                description.BaseCooldown));
            return weapon;
        }

        public static Entity? MakeWeapon(string fullId, ItemKingdomDescription items)
        {
            string classId = fullId.Substring(0, 3);
            int familyIndex = int.Parse(fullId.Substring(3));
            var description = items.GetDescriptionByIdAndIndex<WeaponDescription>(classId, familyIndex);
            if (description == null)
                return null;

            return MakeWeapon(description);
        }

        public static string? GetIdString(Entity item, ItemKingdomDescription items)
        {
            var weaponComp = item.GetComponent<WeaponComponent>();
            if (weaponComp == null)
                return null;
            string className = weaponComp.Class;
            string familyName = weaponComp.Family;
            foreach (var weapClass in items.Weapons)
            {
                if (weapClass.Name != className)
                    continue;
                for (int i = 0; i < weapClass.Families.Count; i++)
                {
                    if (weapClass.Families[i].Name != familyName)
                        continue;
                    return weapClass.Id + i.ToString();
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

        public List<string> GetAllItemClassIds()
        {
            if (s_itemKingdom == null)
                return new();
            return s_itemKingdom.Weapons.Select(w => w.Id).ToList(); // CornerCut: Assumes single phylum
        }

        public string? GetItemClassName(string id)
        {
            if (s_itemKingdom == null)
                return null;
            return s_itemKingdom.Weapons.Where(w => w.Id == id).FirstOrDefault()?.Name; // CornerCut: Assumes single phylum
        }

        public string? GetItemClassIdFromName(string name)
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
        public string Class { get; set; }
        public string Family { get; set; }
        public double BaseDamage { get; set; }
        public double BaseCooldown { get; set; }

        public WeaponDescription()
        {
            Class = "";
            Family = "";
            BaseDamage = 1.0;
            BaseCooldown = 1.0;
        }
    }

    public class WeaponFamilyDescription
    {
        public string Name { get; set; }

        public double BaseDamage { get; set; }

        public double BaseCooldown { get; set; }

        public WeaponFamilyDescription()
        {
            Name = "";
            BaseDamage = 1.0;
            BaseCooldown = 1.0;
        }
    }

    public class ItemClassDescription<T> where T : WeaponFamilyDescription
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public List<T> Families { get; set; }

        public ItemClassDescription()
        {
            Name = "";
            Id = "";
            Families = new();
        }

        public T? GetFamilyAt(int index)
        {
            return Families.ElementAtOrDefault(index) ?? null;
        }
    }

    public class ItemKingdomDescription
    {
        public List<ItemClassDescription<WeaponFamilyDescription>> Weapons { get; set; }

        public ItemKingdomDescription()
        {
            Weapons = new();
        }

        public T? GetDescriptionByIdAndIndex<T>(string idString, int index) where T : WeaponDescription
        {
            if (typeof(T) == typeof(WeaponDescription)) // CornerCut: Let's see how well this scales with other item types...
            {
                foreach (var weaponClass in Weapons)
                {
                    if (weaponClass.Id == idString)
                    {
                        var family = weaponClass.GetFamilyAt(index);
                        if (family == null)
                            return null;
                        return (T)new WeaponDescription()
                        {
                            Class = weaponClass.Name,
                            Family = family.Name,
                            BaseCooldown = family.BaseCooldown,
                            BaseDamage = family.BaseDamage
                        };
                    }
                }
            }
            return null;
        }

        public string? GetClassIdFromItemClassName(string className)
        {
            foreach (var weapon in Weapons)
            {
                if (weapon.Name == className)
                    return weapon.Id;
            }
            return null;
        }
    }
}
