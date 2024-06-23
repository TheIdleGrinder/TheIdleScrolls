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
        private static readonly string FullRegexString = @":([a-zA-Z]+-)([A-F0-9][A-F0-9])@([A-F0-9][A-F0-9])+([A-F0-9][A-F0-9])";
        public override string ToString()
        {
            return $":{FamilyId}{GenusIndex:X02}@{(short)MaterialId:X02}+{Rarity:X02}"; 
        }

        public static ItemBlueprint Parse(string code)
        {
            if (code.Length != 12 || code[0] != ':')
            {
                try
                {
                    var oldId = new ItemIdentifier(code);
                    return new ItemBlueprint(oldId.FamilyId, oldId.GenusIndex, oldId.MaterialId, oldId.RarityLevel);
                }
                catch (Exception)
                {
                    throw new ArgumentException("Invalid item code");
                }
            }

            string familyId = code.Substring(1, 3);
            int genusIndex = Convert.ToInt32(code.Substring(4, 2), 16);
            MaterialId materialId = (MaterialId)Convert.ToInt16(code.Substring(7, 2), 16);
            int rarity = Convert.ToInt32(code.Substring(10, 2), 16);

            return new ItemBlueprint(familyId, genusIndex, materialId, rarity);
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
