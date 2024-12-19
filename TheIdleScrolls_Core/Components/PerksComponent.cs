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
        readonly List<Perk> Perks = new();

        readonly HashSet<string> ChangedPerks = new(); // Holds list of perks that need to be processed by the PerksSystem

        public void AddPerk(Perk perk)
        {
            Perks.Add(perk);
            ChangedPerks.Add(perk.Id);
        }

        public bool RemovePerk(Perk perk)
        {
            ChangedPerks.Add(perk.Id);
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

        public bool HasPerk(string id)
        {
            return Perks.Any(p => p.Id == id);
        }

        public List<Perk> GetChangedPerks() => Perks.Where(p => ChangedPerks.Contains(p.Id)).ToList();

        public List<Modifier> GetModifiers()
        {
            return Perks.Select(p => p.Modifiers).Aggregate((agg, lm) => agg.Concat(lm).ToList());
        }

        public void MarkPerkAsUpdated(Perk perk)
        {
            ChangedPerks.Remove(perk.Id);
        }
    }
}
