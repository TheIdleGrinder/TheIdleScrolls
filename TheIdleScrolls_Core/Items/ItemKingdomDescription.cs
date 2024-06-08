using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Items
{
    public class EquippableDescription
    {
        public List<string> Slots { get; set; } = new();
        public double Encumbrance { get; set; } = 0.0;

        public EquippableDescription(List<string> slots, double encumbrance)
        {
            Slots = slots;
            Encumbrance = encumbrance;
        }

        public EquippableDescription(string slots, double encumbrance = 0.0)
        {
            Slots = slots.Split(' ').ToList();
            Encumbrance = encumbrance;
        }
    }

    public class WeaponGenus
    {
        public double BaseDamage { get; set; }
        public double BaseCooldown { get; set; }

        public WeaponGenus(double baseDamage, double baseCooldown)
        {
            BaseDamage = baseDamage;
            BaseCooldown = baseCooldown;
        }
    }

    public class ArmorGenus
    {
        public double BaseArmor { get; set; }
        public double BaseEvasion { get; set; }

        public ArmorGenus(double baseArmor, double baseEvasion)
        {
            BaseArmor = baseArmor;
            BaseEvasion = baseEvasion;
        }
    }

    public class ItemGenusDescription
    {
        public EquippableDescription? Equippable { get; set; } = null;
        public WeaponGenus? Weapon { get; set; } = null;
        public ArmorGenus? Armor { get; set; } = null;
        public int DropLevel { get; set; } = 1;
        public List<string> ValidMaterials { get; set; } = new();

        public ItemGenusDescription(
            EquippableDescription? equippable, WeaponGenus? weapon, ArmorGenus? armor, int dropLevel, List<string> validMaterials)
        {
            Equippable = equippable;
            Weapon = weapon;
            Armor = armor;
            DropLevel = dropLevel;
            ValidMaterials = validMaterials;
        }
    }

    public class ItemFamilyDescription
    {
        public string Id { get; set; } = "";

        public List<ItemGenusDescription> Genera { get; set; } = new();

        public ItemFamilyDescription()
        {

        }

        public ItemFamilyDescription(string id, List<ItemGenusDescription> genera)
        {
            Id = id;
            Genera = genera;
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

    public class ItemMaterialDescription
    {
        public string Id { get; set; } = "";
        public double PowerMultiplier { get; set; } = 1.0;
        public int MinimumLevel { get; set; } = 0;

        // CornerCut: Material id ends with the index, so extract it. If the last character is not a digit, it's a T0 training item
        // Training material is T0, other materials start at T1
        public int Tier => int.TryParse(Id[^1].ToString(), out int t) ? t + 1 : 0;
    }

    public class ItemKingdomDescription
    {
        public List<ItemFamilyDescription> Families { get; set; } = new();
        public List<ItemRarityDescription> Rarities { get; set; } = new();
        public List<ItemMaterialDescription> Materials { get; set; } = new();

        public ItemKingdomDescription()
        {

        }

        public ItemGenusDescription? GetGenusDescriptionByIdAndIndex(string idString, int index)
        {
            foreach (var family in Families)
            {
                if (family.Id == idString)
                {
                    return family.GetGenusAt(index);
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

        public ItemMaterialDescription? GetMaterial(string id)
        {
            return Materials.FirstOrDefault(x => x.Id == id);
        }

        public List<ItemGenusDescription> GetAllItemGenusDescriptions()
        {
            List<ItemGenusDescription> descriptions = new();
            foreach (var family in Families)
            {
                foreach (var genus in family.Genera)
                {
                    descriptions.Add(genus);
                }
            }
            return descriptions;
        }

        public List<ItemSpeciesDescription> GetAllItemSpeciesDescriptions()
        {
            List<ItemSpeciesDescription> descriptions = new();
            foreach (var genus in GetAllItemGenusDescriptions())
            {
                foreach (var materialId in genus.ValidMaterials)
                {
                    var material = GetMaterial(materialId) ?? throw new InvalidCastException($"Invalid material id: {materialId}");
                    descriptions.Add(new(genus, material));
                }
            }
            return descriptions;
        }
    }

    /// <summary>
    /// Item Species ^= Genus + Material
    /// This class is not a direct part of the item kingdom.
    /// </summary>
    public class ItemSpeciesDescription
    {
        public ItemGenusDescription Genus { get; set; }
        public ItemMaterialDescription Material { get; set; }

        public ItemSpeciesDescription(ItemGenusDescription genus, ItemMaterialDescription material)
        {
            Genus = genus;
            Material = material;
        }
    }
}
