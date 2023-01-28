using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Items
{
    public class ItemIdentifier
    {
        private static string FullRegexString = @"([a-zA-Z]+)([0-9]+)(\+[0-9]+)?";
        public string Code { get; set; } = string.Empty;

        public ItemIdentifier(string code)
        {
            Code = code;
            if (!ValidateItemCode(code))
            {
                throw new ArgumentException($"Invalid item code: {code}");
            }
        }

        public ItemIdentifier(string family, int genusIndex) : this(family + genusIndex.ToString())
        {
            
        }

        public string FamilyId { get { return ExtractFamilyId(Code); } }

        public int GenusIndex { get { return ExtractGenusIndex(Code); } }

        public string GenusId { get { return ExtractGenusId(Code); } }

        public string SpeciesId { get { return ExtractSpeciesId(Code); } }

        public int RarityLevel {
            get { return ExtractRarityLevel(Code); }
            set { Code = UpdateRarityLevel(Code, value); }
        }

        public ItemFamilyDescription GetFamilyDescription()
        {                 
            return ItemFactory.ItemKingdom.Families.Where(f => f.Id == FamilyId).First(); // First() works because code is validated
        }

        public ItemGenusDescription GetGenusDescription()
        { 
            return GetFamilyDescription().Genera[GenusIndex]; // Works because code is validated
        }

        public ItemDescription GetItemDescription()
        {
            return ItemFactory.ItemKingdom.GetDescriptionByIdAndIndex(FamilyId, GenusIndex) 
                    ?? throw new Exception($"Item code was invalidated: {Code}");
        }

        public static string ExtractFamilyId(string itemCode)
        {
            return itemCode[..3];
        }

        public static int ExtractGenusIndex(string itemCode)
        {
            return Int32.Parse(itemCode[3..4]);
        }

        public static string ExtractGenusId(string itemCode)
        {
            return itemCode[..4];
        }

        public static string ExtractSpeciesId(string itemCode)
        {
            return ExtractGenusId(itemCode);
        }

        public static int ExtractRarityLevel(string itemCode)
        {
            if (!itemCode.Contains('+'))
                return 0;
            return Int32.Parse(itemCode.Split('+')[1]);
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

        public static bool ValidateItemCode(string itemCode)
        {
            try
            {
                var regex = new Regex(FullRegexString);
                var match = regex.Match(itemCode);
                if (match.Value != itemCode)
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
