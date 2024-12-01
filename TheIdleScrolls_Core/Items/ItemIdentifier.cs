using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;

namespace TheIdleScrolls_Core.Items
{
    public class ItemIdentifier
    {
        private static string FullRegexString = @"([a-zA-Z0-9]+-)?([a-zA-Z]+)([0-9]+)(\+[0-9]+)?";
        public string Code { get; set; } = string.Empty;

        public ItemIdentifier(string code)
        {
            Code = code;
/*            if (!ValidateItemCode(Code))
            {
                throw new ArgumentException($"Invalid item code: {code}");
            }*/
        }

        public ItemIdentifier(string family, int genusIndex, string material)
        {
            Code = AssembleItemCode(family, genusIndex, material);
/*            if (!ValidateItemCode(Code))
            {
                throw new ArgumentException($"Invalid item code: {Code}");
            }*/
        }

        public string FamilyId { get { return ExtractFamilyId(Code); } }

        public int GenusIndex { get { return ExtractGenusIndex(Code); } }

        public string GenusId { get { return ExtractGenusId(Code); } }

        public string SpeciesId { get { return ExtractSpeciesId(Code); } }

        public int RarityLevel {
            get { return ExtractRarityLevel(Code); }
            set { Code = UpdateRarityLevel(Code, value); }
        }

        public MaterialId MaterialId
        {
            get { return ExtractMaterialId(Code); }
        }

        public ItemFamilyDescription GetFamilyDescription()
        {                 
            return ItemKingdom.Families.Where(f => f.Id == FamilyId).First(); // First() works because code is validated
        }

        public ItemGenusDescription GetGenusDescription()
        { 
            return GetFamilyDescription().Genera[GenusIndex]; // Works because code is validated
        }

        public ItemMaterial GetMaterial()
        {
            return ItemKingdom.GetMaterial(MaterialId!)!; // ! works because the material is validated at construction
        }

        public static string AssembleItemCode(string familyId, int genusIndex, string? material, int rarity = 0)
        {
            string code = familyId + genusIndex.ToString();
            code = UpdateMaterial(code, MaterialIdFromString(material ?? ""));
            code = UpdateRarityLevel(code, rarity);
            return code;
        }

        public static string ExtractFamilyId(string itemCode)
        {
            int offset = Math.Max(itemCode.IndexOf('-') + 1, 0);
            return itemCode[offset..(offset + 3)];
        }

        public static int ExtractGenusIndex(string itemCode)
        {
            int offset = Math.Max(itemCode.IndexOf('-') + 4, 0);
            int end = itemCode.IndexOf('+', offset);
            if (end < 0)
                end = itemCode.Length;
            return Int32.Parse(itemCode[offset..end]);
        }

        public static string ExtractGenusId(string itemCode)
        {
            int offset = Math.Max(itemCode.IndexOf('-') + 1, 0);
            int end = itemCode.IndexOf('+', offset);
            if (end < 0)
                end = itemCode.Length;
            return itemCode[offset..end]; // Leave out material and rarity
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

        public static MaterialId MaterialIdFromString(string materialString)
        {
            if (materialString.StartsWith("SIMPLE"))
                return Definitions.MaterialId.Simple;
            if (materialString.Length != 2)
                throw new Exception($"Invalid material string: {materialString}");
            int type = materialString[0] switch
            {
                'L' => 0x10,
                'M' => 0x20,
                'W' => 0x30,
                _ => 0
            };
            int tier = Int32.Parse(materialString[1].ToString());
            return (MaterialId)(type + tier);
        }

        public static string MaterialStringFromId(MaterialId id)
        {
            if (id == Definitions.MaterialId.Simple)
                return "SIMPLE";
            return id switch
            {
                Definitions.MaterialId.Leather1 => "L0",
                Definitions.MaterialId.Leather2 => "L1",
                Definitions.MaterialId.Leather3 => "L2",
                Definitions.MaterialId.Metal1 => "M0",
                Definitions.MaterialId.Metal2 => "M1",
                Definitions.MaterialId.Metal3 => "M3",
                Definitions.MaterialId.Wood1 => "W0",
                Definitions.MaterialId.Wood2 => "W1",
                Definitions.MaterialId.Wood3 => "W2",
                _ => "SIMPLE"
            };
        }

        public static MaterialId ExtractMaterialId(string itemCode)
        {
            if (!itemCode.Contains('-'))
                throw new Exception($"Item code does not contain material: {itemCode}");

            return MaterialIdFromString(itemCode.Split('-')[0]);
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

        public static string UpdateMaterial(string itemCode, MaterialId material)
        {
            int index = itemCode.IndexOf('-');
            string substr = itemCode[(index + 1)..]; // string without the material id
            
            return $"{MaterialStringFromId(material)}-{substr}";
        }

        public static bool ValidateItemCode(string itemCode)
        {
            try
            {
                var regex = new Regex(FullRegexString);
                var match = regex.Match(itemCode);
                if (match.Value != itemCode)
                    return false;
                MaterialId material = ExtractMaterialId(itemCode)!;
                if (ItemKingdom.GetMaterial(material) == null)
                    return false;
                var family = ExtractFamilyId(itemCode);
                var genusIdx = ExtractGenusIndex(itemCode);
                return ItemKingdom.HasGenus(family, genusIdx);
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
