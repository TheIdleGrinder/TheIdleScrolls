using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;

namespace TheIdleScrolls_Core.Items
{
    public record ItemBlueprint(string FamilyId, int GenusIndex, Definitions.MaterialId MaterialId, int Rarity = 0)
    {
        //private static readonly string FullRegexString = @":([a-zA-Z]+-)([A-F0-9][A-F0-9])@([A-F0-9][A-F0-9])+([A-F0-9][A-F0-9])";
        public override string ToString()
        {
            return $":{FamilyId}{GenusIndex:X02}@{(short)MaterialId:X02}+{Rarity:X02}"; 
        }

        public static ItemBlueprint Parse(string code)
        {
            if (code.Length != 12 || code[0] != ':')
            {
                return ParseFromOldCode(code);
            }

            string familyId = code.Substring(1, 3);
            int genusIndex = Convert.ToInt32(code.Substring(4, 2), 16);
            MaterialId materialId = (MaterialId)Convert.ToInt16(code.Substring(7, 2), 16);
            int rarity = Convert.ToInt32(code.Substring(10, 2), 16);

            return new ItemBlueprint(familyId, genusIndex, materialId, rarity);
        }

        private static ItemBlueprint ParseFromOldCode(string code)
        {
            try
            {
                var oldId = new ItemIdentifier(code);
                string familyId= oldId.FamilyId;
                int genusIndex = oldId.GenusIndex;
                if (familyId == ItemFamilies.LightArmor || familyId == ItemFamilies.HeavyArmor)
                {
                    string familySuffix = genusIndex switch
                    {
                        0 or 1 or 6 => "CH",
                        2 or 7 => "HE",
                        3 or 8 => "GL",
                        4 or 9 => "BT",
                        5 or 10 => "SH",
                        _ => throw new ArgumentException("Invalid item code")
                    };
                    familyId = familyId[0] + familySuffix;
                    genusIndex = genusIndex switch
                    {
                        0 or 2 or 3 or 4 or 5 => 0,
                        1 or 7 or 8 or 9 or 10 => 1,
                        6 => 2,
                        _ => throw new ArgumentException("Invalid item code")
                    };
                }
                return new ItemBlueprint(familyId, genusIndex, oldId.MaterialId, oldId.RarityLevel);
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid item code");
            }
        }

        public static ItemBlueprint WithLocalMaterialIndex(string familyId, int genusIndex, int materialIndex, int rarity = 0)
        {
            var genus = ItemKingdom.GetGenusDescriptionByIdAndIndex(familyId, genusIndex) 
                ?? throw new Exception($"Invalid item genus: {familyId}{genusIndex}");
            if (materialIndex < 0 || materialIndex >= genus.ValidMaterials.Count)
                throw new Exception($"Invalid material index for {familyId}{genusIndex}: {materialIndex}");
            return new ItemBlueprint(familyId, genusIndex, genus.ValidMaterials[materialIndex], rarity);
        }

        public ItemFamilyDescription GetFamilyDescription()
        {
            return ItemKingdom.GetFamilyDescription(this) ?? throw new Exception($"No item family for [{ToString()}]");
        }

        public ItemGenusDescription GetGenusDescription()
        {
            return ItemKingdom.GetGenusDescription(this) ?? throw new Exception($"No item genus for [{ToString()}]");
        }

        public ItemMaterial GetMaterial()
        {
            return ItemKingdom.GetMaterial(MaterialId) ?? throw new Exception($"No item material for [{ToString()}]");
        }
    }
}
