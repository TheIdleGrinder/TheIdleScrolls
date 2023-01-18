using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Items
{
    public class ItemDescription
    {
        public string Family { get; set; } = "??";
        public string Genus { get; set; } = "??";
        public EquippableDescription? Equippable { get; set; } = null;
        public WeaponGenus? Weapon { get; set; } = null;
        public ArmorGenus? Armor { get; set; } = null;
        public int DropLevel { get; set; } = 1;

        public ItemDescription() { }

        public ItemDescription(string familyName, ItemGenusDescription genus)
        {
            Family = familyName;
            Genus = genus.Name;
            Weapon = genus.Weapon;
            Armor = genus.Armor;
            Equippable = genus.Equippable;
            DropLevel = genus.DropLevel;
        }
    }

    public class EquippableDescription
    {
        public string Slot { get; set; } = "";

        public double Encumbrance { get; set; } = 0.0;
    }

    public class WeaponGenus
    {
        public double BaseDamage { get; set; }

        public double BaseCooldown { get; set; }
    }

    public class ArmorGenus
    {
        public double BaseArmor { get; set; }
        public double BaseEvasion { get; set; }
    }

    public class ItemGenusDescription
    {
        public string Name { get; set; } = "??";

        public EquippableDescription? Equippable { get; set; } = null;

        public WeaponGenus? Weapon { get; set; } = null;

        public ArmorGenus? Armor { get; set; } = null;

        public int DropLevel { get; set; } = 1;
    }

    public class ItemFamilyDescription
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public List<ItemGenusDescription> Genera { get; set; }

        public ItemFamilyDescription()
        {
            Name = "";
            Id = "";
            Genera = new();
        }

        public ItemGenusDescription? GetGenusAt(int index)
        {
            return Genera.ElementAtOrDefault(index) ?? null;
        }
    }

    public class ItemKingdomDescription
    {
        public List<ItemFamilyDescription> Families { get; set; }

        public ItemKingdomDescription()
        {
            Families = new();
        }

        public ItemDescription? GetDescriptionByIdAndIndex(string idString, int index)
        {
            foreach (var family in Families)
            {
                if (family.Id == idString)
                {
                    var genus = family.GetGenusAt(index);
                    if (genus == null)
                        return null;
                    return new ItemDescription(family.Name, genus);
                }
            }
            return null;
        }

        public string? GetFamilyIdFromItemFamilyName(string familyName)
        {
            foreach (var weapon in Families)
            {
                if (weapon.Name == familyName)
                    return weapon.Id;
            }
            return null;
        }
    }
}
