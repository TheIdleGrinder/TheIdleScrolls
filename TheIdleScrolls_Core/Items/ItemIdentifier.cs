using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Items
{
    public class ItemIdentifier
    {
        private static string FullRegexString = @"([a-zA-Z][0-9]+-)?([a-zA-Z]+)([0-9]+)(\+[0-9]+)?";
        public string Code { get; set; } = string.Empty;

        public ItemIdentifier(string code)
        {
            Code = code;
            if (!ValidateItemCode(code))
            {
                throw new ArgumentException($"Invalid item code: {code}");
            }
        }

        public ItemIdentifier(string family, int genusIndex, string? material = null) : this(family + genusIndex.ToString())
        {
            if (material != null)
            {
                MaterialId = material;
                if (!ValidateItemCode(Code))
                {
                    throw new ArgumentException($"Invalid item code: {Code}");
                }
            }
        }

        public string FamilyId { get { return ExtractFamilyId(Code); } }

        public int GenusIndex { get { return ExtractGenusIndex(Code); } }

        public string GenusId { get { return ExtractGenusId(Code); } }

        public string SpeciesId { get { return ExtractSpeciesId(Code); } }

        public int RarityLevel {
            get { return ExtractRarityLevel(Code); }
            set { Code = UpdateRarityLevel(Code, value); }
        }

        public string? MaterialId
        {
            get { return ExtractMaterialId(Code); }
            set { Code = UpdateMaterial(Code, value); }
        }

        public ItemFamilyDescription GetFamilyDescription()
        {                 
            return ItemFactory.ItemKingdom.Families.Where(f => f.Id == FamilyId).First(); // First() works because code is validated
        }

        public ItemGenusDescription GetGenusDescription()
        { 
            return GetFamilyDescription().Genera[GenusIndex]; // Works because code is validated
        }

        public ItemMaterialDescription GetMaterial()
        {
            if (MaterialId == null)
            {
                return ItemFactory.ItemKingdom.Materials[0]; // TODO: return first valid material for genus
            }
            else
            {
                return ItemFactory.ItemKingdom.GetMaterial(MaterialId!)!; // ! works because the material is validated at construction
            }
        }

        public string GetItemName()
        {
            return ($"{MaterialId?.Localize()} " ?? "") 
                + GenusId.Localize() 
                + (RarityLevel > 0 ? $" + {RarityLevel}" : "");
        }

        public static string ExtractFamilyId(string itemCode)
        {
            int offset = Math.Max(itemCode.IndexOf('-') + 1, 0);
            return itemCode[offset..(offset + 3)];
        }

        public static int ExtractGenusIndex(string itemCode)
        {
            int offset = Math.Max(itemCode.IndexOf('-') + 1, 0);
            return Int32.Parse(itemCode[(offset + 3)..(offset + 4)]);
        }

        public static string ExtractGenusId(string itemCode)
        {
            int offset = Math.Max(itemCode.IndexOf('-') + 1, 0);
            return itemCode[offset..(offset + 4)]; // Leave out material and rarity
        }

        public static string ExtractSpeciesId(string itemCode)
        {
            return itemCode.Split('+')[0]; // Everything except the rarity
        }

        public static int ExtractRarityLevel(string itemCode)
        {
            if (!itemCode.Contains('+'))
                return 0;
            return Int32.Parse(itemCode.Split('+')[1]);
        }

        public static string? ExtractMaterialId(string itemCode)
        {
            if (!itemCode.Contains('-'))
                return null;
            return "MAT_" + itemCode.Split('-')[0];
        }

        public static string UpdateRarityLevel(string itemCode, int newRarityLevel)
        {
            string newCode = ExtractSpeciesId(itemCode);
            if (newRarityLevel != 0)
            {
                newCode += $"+{newRarityLevel}";
            }
            return newCode;
        }

        public static string UpdateMaterial(string itemCode, string? material)
        {
            int index = itemCode.IndexOf('-');
            string substr = itemCode[(index + 1)..]; // string without the material id
            
            if (material == null)
            {
                return substr;
            }

            if (!ItemFactory.ItemKingdom.Materials.Any(m => m.Id == material))
                throw new Exception($"Invalid material id: {material}");

            return $"{material.Split('_')[1]}-{substr}";
        }

        public static bool ValidateItemCode(string itemCode)
        {
            try
            {
                var regex = new Regex(FullRegexString);
                var match = regex.Match(itemCode);
                if (match.Value != itemCode)
                    return false;
                var material = ExtractMaterialId(itemCode);
                if (material != null && ItemFactory.ItemKingdom.GetMaterial(material) == null)
                    return false;
                var family = ExtractFamilyId(itemCode);
                var genusIdx = ExtractGenusIndex(itemCode);
                return ItemFactory.ItemKingdom.HasGenus(family, genusIdx);
            }
            catch (Exception)
            {
                return false;
            }
        }


        public override bool Equals(object? obj)
        {
            return obj is ItemIdentifier identifier &&
                   Code == identifier.Code;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Code);
        }

        public static bool operator ==(ItemIdentifier a, ItemIdentifier b)
        {
            return a.Code == b.Code;
        }

        public static bool operator !=(ItemIdentifier a, ItemIdentifier b)
        {
            return !(a == b);
        }

        
    }
}
