using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Items
{
    public record ItemBlueprint(string FamilyId, int GenusIndex, Definitions.MaterialId MaterialId, int Rarity = 0)
    {
        public override string ToString()
        {
            return $":{FamilyId}{GenusIndex}@{(short)MaterialId}+{Rarity}"; 
        }
    }
}
