using MiniECS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Modifiers;

namespace TheIdleScrolls_Core.Components
{
    public class PerksComponent : IComponent
    {
        List<Perk> Perks = new List<Perk>();

        public void AddPerk(Perk perk)
        {
            Perks.Add(perk);
        }

        public bool RemovePerk(Perk perk)
        {
            return Perks.Remove(perk);
        }

        public bool RemovePerk(string id)
        {
            Perk? target = Perks.FirstOrDefault(x => x.Id == id);
            if (target == null)
                return false;
            return RemovePerk(target);
        }

        public List<Perk> GetPerks()
        {
            return Perks;
        }

        public List<Modifier> GetModifiers()
        {
            return Perks.Select(p => p.Modifiers).Aggregate((agg, lm) => agg.Concat(lm).ToList());
        }
    }
}
