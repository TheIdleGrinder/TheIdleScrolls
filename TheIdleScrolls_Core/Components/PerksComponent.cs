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
        public int ActivePerkLimit { get; set; } = 0;

        readonly HashSet<string> ActivePerks = [];

        readonly List<Perk> Perks = [];

        readonly HashSet<string> ChangedPerks = []; // Holds list of perks that need to be processed by the PerksSystem

        public void AddPerk(Perk perk)
        {
            if (Perks.Any(p => p.Id == perk.Id))
                return;
            if (perk.AlwaysActive)
            {
                Perks.Insert(0, perk);
                ActivePerks.Add(perk.Id);
            }
            else
            {
                Perks.Add(perk);
            }
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

        public List<Perk> GetActivePerks()
        {
            return Perks.Where(p => ActivePerks.Contains(p.Id)).ToList();
        }

        public int GetUsedPerkPoints()
        {
            return GetActivePerks().Where(p => !p.AlwaysActive).Count();
        }

        public bool HasPerk(string id)
        {
            return Perks.Any(p => p.Id == id);
        }

        public bool IsPerkActive(string id)
        {
            return HasPerk(id) && ActivePerks.Contains(id);
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

        public bool SetPerkActive(string id, bool active)
        {
            if (active)
            {
                if (!HasPerk(id))
                    return false;
                ActivePerks.Add(id);
            }
            else
            {
                ActivePerks.Remove(id);
            }
            return true;
        }
    }
}
