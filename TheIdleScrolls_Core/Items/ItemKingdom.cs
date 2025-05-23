﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.Items
{
    public record EquippableDescription(List<EquipmentSlot> Slots, double Encumbrance);

    public record WeaponGenus(double BaseDamage, double BaseCooldown)
    {
        public double Dps => (BaseCooldown != 0.0) ? BaseDamage / BaseCooldown : 0.0;
    }

    public record ArmorGenus(double BaseArmor, double BaseEvasion);

    public class ItemGenusDescription
    {
        public string Name { get; set; } = "Missing genus name";
        public EquippableDescription? Equippable { get; set; } = null;
        public WeaponGenus? Weapon { get; set; } = null;
        public ArmorGenus? Armor { get; set; } = null;
        public int DropLevel { get; set; } = 1;
        public List<MaterialId> ValidMaterials { get; set; } = new();

        public ItemGenusDescription(string name, EquippableDescription? equippable, 
            WeaponGenus? weapon, ArmorGenus? armor, int dropLevel, List<MaterialId> validMaterials)
        {
            Name = name;
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
        public string Name { get; set; } = "Missing family name";

        public List<ItemGenusDescription> Genera { get; set; } = new();

        public string RelatedAbilityId { get; set; } = "";

        public ItemFamilyDescription()
        {

        }

        public ItemFamilyDescription(string id, string name, string ability, List<ItemGenusDescription> genera)
        {
            Id = id;
            Name = name;
            Genera = genera;
            RelatedAbilityId = ability;
        }

        public ItemGenusDescription? GetGenusAt(int index)
        {
            return Genera.ElementAtOrDefault(index) ?? null;
        }
    }

    public class ItemQualityDescription
    {
        public int MinLevel { get; set; } = 1;
        public double InverseWeight { get; set; } = 10.0;
    }

    public class ItemKingdom
    {
        public static List<ItemFamilyDescription> Families { get; } = ItemList.ItemFamilies;
        public static List<ItemQualityDescription> Qualities { get; } = new()
        {
            new() { MinLevel = 10,  InverseWeight =     5.0 },
            new() { MinLevel = 30,  InverseWeight =    25.0 },
            new() { MinLevel = 50,  InverseWeight =   125.0 },
            new() { MinLevel = 70,  InverseWeight =   625.0 },
            new() { MinLevel = 90,  InverseWeight =  3000.0 },
            new() { MinLevel = 110, InverseWeight = 15000.0 },
            new() { MinLevel = 150, InverseWeight = 75000.0 },
        };
        public static int MaxItemQuality => Qualities.Count;

        public static List<ItemMaterial> Materials { get; } = Definitions.Materials.MaterialList;

        public ItemKingdom()
        {

        }

        public static ItemGenusDescription? GetGenusDescriptionByIdAndIndex(string idString, int index)
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

        public static ItemGenusDescription? GetGenusDescription(ItemBlueprint blueprint)
        {
            return GetGenusDescriptionByIdAndIndex(blueprint.FamilyId, blueprint.GenusIndex);
        }

        public static ItemFamilyDescription? GetFamilyDescription(ItemBlueprint blueprint)
        {
            return Families.FirstOrDefault(x => x.Id == blueprint.FamilyId);
        }

        public static bool HasGenus(string familyId, int genusIndex)
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

        public static ItemMaterial? GetMaterial(MaterialId id)
        {
            return Materials.FirstOrDefault(x => x.Id == id);
        }

        public static List<ItemGenusDescription> GetAllItemGenusDescriptions()
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

        public static List<ItemSpeciesDescription> GetAllItemSpeciesDescriptions()
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
        public ItemMaterial Material { get; set; }

        public ItemSpeciesDescription(ItemGenusDescription genus, ItemMaterial material)
        {
            Genus = genus;
            Material = material;
        }
    }
}
