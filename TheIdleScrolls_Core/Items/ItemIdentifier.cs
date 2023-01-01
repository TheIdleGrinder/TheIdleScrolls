using System;
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

        public ItemFamilyDescription FamilyDescription 
        { get
            {
                return ItemFactory.ItemKingdom.Families.Where(f => f.Id == FamilyId).First();
            } 
        }

        public ItemGenusDescription GenusDescription 
        { get
            {
                return FamilyDescription.Genera[GenusIndex];
            } 
        }

        public static string ExtractFamilyId(string itemCode)
        {
            return itemCode[..3];
        }

        public static int ExtractGenusIndex(string itemCode)
        {
            return Int32.Parse(itemCode[3..3]);
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
