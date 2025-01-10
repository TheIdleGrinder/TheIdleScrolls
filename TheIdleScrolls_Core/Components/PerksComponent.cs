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
        public int PerkPointLimit { get; set; } = -1;

        public readonly Dictionary<string, int> ActivePerkLevels = [];

        readonly List<Perk> Perks = [];

        readonly HashSet<string> ChangedPerks = []; // Holds list of perks that need to be processed by the PerksSystem

        public void AddPerk(Perk perk, int index = -1)
        {
            if (Perks.Any(p => p.Id == perk.Id))
                return;
            if (index >= 0 && index <= Perks.Count)
            {
                Perks.Insert(index, perk);
            }
            else
            {
                if (perk.Permanent)
                {
                    Perks.Insert(GetPermanentPerkCount(), perk);
                }
                else
                {
                    Perks.Add(perk);
                }
            }
            if (perk.Permanent)
            {
                ActivePerkLevels[perk.Id] = perk.MaxLevel;
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
            return Perks.Where(p => ActivePerkLevels.ContainsKey(p.Id)).ToList();
        }

        public int GetUsedPerkPoints()
        {
            return GetActivePerks().Where(p => !p.Permanent).Sum(p => ActivePerkLevels[p.Id]);
        }

        public int GetPermanentPerkCount()
        {
            return GetActivePerks().Where(p => p.Permanent).Count();
        }

        public int GetAvailablePerkPoints()
        {
            return PerkPointLimit - GetUsedPerkPoints();
        }

        public bool HasPerk(string id)
        {
            return Perks.Any(p => p.Id == id);
        }

        public int GetPerkLevel(string id)
        {
            return ActivePerkLevels.TryGetValue(id, out int value) ? value : 0;
        }

        public bool IsPerkActive(string id)
        {
            return HasPerk(id) && ActivePerkLevels.ContainsKey(id) && ActivePerkLevels[id] > 0;
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

        public bool SetPerkLevel(string id, int level)
        {
            if (!HasPerk(id))
                return false;
            if (level < 0)
                return false;
            if (level == 0)
            {
                ActivePerkLevels.Remove(id);
            }
            else
            {
                ActivePerkLevels[id] = level;
            }
            ChangedPerks.Add(id);
            return true;
        }
    }
}
