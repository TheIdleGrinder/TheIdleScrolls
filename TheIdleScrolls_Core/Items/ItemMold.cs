using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Items
{
    public class ItemMold
    {
        public string FamilyId { get; set; }
        public int GenusIndex { get; set; }
        public Definitions.MaterialId MaterialId { get; set; }
        public int Rarity { get; set; }

        public ItemMold(string familyId, int genusIndex, Definitions.MaterialId material, int rarity)
        {
            FamilyId = familyId;
            GenusIndex = genusIndex;
            MaterialId = material;
            Rarity = rarity;
        }

    }
}
