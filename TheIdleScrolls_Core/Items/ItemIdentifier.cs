﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Items
{
    public class ItemIdentifier
    {
        public string Code { get; set; } = string.Empty;

        public ItemIdentifier(string code)
        {
            Code = code;
            if (!ValidateItemCode(code))
            {
                throw new ArgumentException($"Invalid item code: {code}");
            }
        }

        public string FamilyId { get { return ExtractFamilyId(Code); } }

        public int GenusIndex { get { return ExtractGenusIndex(Code); } }

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

        public static bool ValidateItemCode(string itemCode)
        {
            try
            {
                var family = ExtractFamilyId(itemCode);
                var genusIdx = ExtractGenusIndex(itemCode);
                return ItemFactory.ItemKingdom.GetDescriptionByIdAndIndex(family, genusIdx) != null;
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
