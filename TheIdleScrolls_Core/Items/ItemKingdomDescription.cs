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
        public ItemIdentifier Identifier { get; set; }
        public EquippableDescription? Equippable { get; set; } = null;
        public WeaponGenus? Weapon { get; set; } = null;
        public ArmorGenus? Armor { get; set; } = null;
        public int DropLevel { get; set; } = 1;

        public ItemDescription(ItemIdentifier identifier, ItemGenusDescription genus)
        {
            Identifier = identifier;
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
        public EquippableDescription? Equippable { get; set; } = null;

        public WeaponGenus? Weapon { get; set; } = null;

        public ArmorGenus? Armor { get; set; } = null;

        public int DropLevel { get; set; } = 1;
    }

    public class ItemFamilyDescription
    {
        public string Id { get; set; }

        public List<ItemGenusDescription> Genera { get; set; }

        public ItemFamilyDescription()
        {
            Id = "";
            Genera = new();
        }

        public ItemGenusDescription? GetGenusAt(int index)
        {
            return Genera.ElementAtOrDefault(index) ?? null;
        }
    }

    public class ItemRarityDescription
    {
        public int MinLevel { get; set; } = 1;

        public double InverseWeight { get; set; } = 10.0;
    }

    public class ItemKingdomDescription
    {
        public List<ItemFamilyDescription> Families { get; set; } = new();

        public List<ItemRarityDescription> Rarities { get; set; } = new();

        public ItemKingdomDescription()
        {

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

                    return new ItemDescription(new ItemIdentifier(idString, index), genus);
                }
            }
            return null;
        }

        public bool HasGenus(string familyId, int genusIndex)
        {
            foreach (var family in Families)
            {
                if (family.Id == familyId)
                {
                    return family.Genera.Count > genusIndex && genusIndex >= 0;
                }
            }
            return false;
        }
    }
}
