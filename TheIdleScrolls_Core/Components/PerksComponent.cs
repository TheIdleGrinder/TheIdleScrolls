using MiniECS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.Components
{
    public class PerksComponent : IComponent
    {
        public int PerkPointLimit => BasePerkPoints + BonusPerkPointIds.Count;
        public int BasePerkPoints { get; set; } = 0;
        
        public readonly Dictionary<string, int> PerkLevels = [];

        readonly List<Perk> Perks = [];

        readonly HashSet<string> BonusPerkPointIds = [];
        public int NewBonusPoints = 0;

        readonly HashSet<string> ChangedPerks = []; // Holds list of perks that need to be processed by the PerksSystem
        static List<string> OrderedPerkIds { get; set; } = []; // Holds the order of perks, which is used when adding new perks
        Dictionary<string, (string TargetId, int Level)> Dependencies { get; } = [];

        public void AddPerk(Perk perk, int index = -1)
        {
            if (OrderedPerkIds.Count == 0)
                PrepareOrderList();
            if (Perks.Any(p => p.Id == perk.Id))
                return;
            if (index >= 0 && index <= Perks.Count)
            {
                Perks.Insert(index, perk);
            }
            else
            {
                int targetIndex = OrderedPerkIds.IndexOf(perk.Id);
                if (targetIndex == -1)
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
                else
                {
                    bool added = false;
                    foreach (var p in Perks)
                    {
                        int pIndex = OrderedPerkIds.IndexOf(p.Id);
                        if (pIndex > targetIndex) // Conveniently skips perks basic perks that are not in the list
                        {
                            Perks.Insert(Perks.IndexOf(p), perk);
                            added = true;
                            break;
                        }
                    }
                    if (!added)
                    {
                        Perks.Add(perk);
                    }
                }
                
            }
            if (!PerkLevels.ContainsKey(perk.Id))
            {
                PerkLevels[perk.Id] = 0;
            }
            if (perk.Permanent)
            {
                PerkLevels[perk.Id] = perk.MaxLevel;
            }
            ChangedPerks.Add(perk.Id);
        }

        public bool RemovePerk(Perk perk)
        {
            ChangedPerks.Add(perk.Id);
            PerkLevels.Remove(perk.Id);
            Dependencies.Remove(perk.Id);
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
            return Perks.Where(p => PerkLevels.ContainsKey(p.Id) && PerkLevels[p.Id] > 0).ToList();
        }

        public int GetUsedPerkPoints()
        {
            return GetActivePerks().Where(p => !p.Permanent).Sum(p => PerkLevels[p.Id]);
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

        public Perk? GetPerk(string Id)
        {
            return Perks.FirstOrDefault(p => p.Id == Id);
        }

        public int GetPerkLevel(string id)
        {
            return PerkLevels.TryGetValue(id, out int value) ? value : 0;
        }

        public bool IsPerkActive(string id)
        {
            return HasPerk(id) && PerkLevels.ContainsKey(id) && PerkLevels[id] > 0;
        }

        public List<Perk> GetChangedPerks() => Perks.Where(p => ChangedPerks.Contains(p.Id)).ToList();

        public List<Modifier> GetModifiers()
        {
            return Perks.Select(p => p.Modifiers).Aggregate((agg, lm) => agg.Concat(lm).ToList());
        }

        public void MarkPerkAsUpdated(Perk perk)
        {
            ChangedPerks.Remove(perk.Id);
            Dependencies.Remove(perk.Id);
            var condition = perk.ConditionFunc(perk.CurrentLevel) as PerkLevelPerkCondition;
            if (condition != null)
            {
                Dependencies[perk.Id] = (condition.PerkId, condition.Level);
            }
        }

        public bool AddBonusPerkPoint(string id)
        {
            NewBonusPoints++;
            return BonusPerkPointIds.Add(id);
        }

        public (bool works, string text) CanSetPerkLevel(Entity owner, string id, int level)
        {
            if (!HasPerk(id))
                return (false, "Perk not found");
            var perk = GetPerk(id);
            if (perk == null)
                return (false, "Perk not found");
            if (level < 0 || level > perk.MaxLevel)
                return (false, "Invalid perk level");
            int currentLevel = GetPerkLevel(id);

            if (level < currentLevel)
            {
                // Check if any other perk depends on the perk being at a level above the new level
                foreach (var dep in Dependencies)
                {
                    if (dep.Value.TargetId == id && dep.Value.Level > level)
                    {
                        Perk dependingPerk = GetPerk(dep.Key) ?? throw new InvalidOperationException("Dependency not found");
                        return (false, $"Perk '{dependingPerk.Name}' requires level {dep.Value.Level}");
                    }
                }
            }
            else if (level > currentLevel)
            {
                var condition = perk.ConditionFunc(level);
                if (condition is null)
                {
                    return (true, "");
                }

                if (condition.IsSatisfied(owner))
                {
                    return (true, condition.Description);
                }
                else
                {
                    return (false, "Dependency not satisfied");
                }
            }

            return (true, "");
        }

        public bool SetPerkLevel(string id, int level)
        {
            if (!HasPerk(id))
                return false;
            if (level < 0)
                return false;
            PerkLevels[id] = level;
            ChangedPerks.Add(id);
            return true;
        }

        private static void PrepareOrderList()
        {
            List<string> permanent = [];
            List<string> nonPermanent = [];
            foreach (var ach in AchievementList.GetAllAchievements())
            {
                if (ach.Reward != null && ach.Reward.GetType() == typeof(PerkReward))
                {
                    var perk = ((PerkReward)ach.Reward).Perk;
                    if (perk.Permanent)
                    {
                        permanent.Add(perk.Id);
                    }
                    else
                    {
                        nonPermanent.Add(perk.Id);
                    }
                }
            }
            OrderedPerkIds = permanent.Concat(nonPermanent).ToList();
        }
    }
}
