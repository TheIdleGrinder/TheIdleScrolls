using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Resources;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrolls_Core.Components
{
    public class AbilitiesComponent : IComponent
    {
        readonly Dictionary<string, Ability> m_abilities = new();

        public AbilitiesComponent() {}

        public void AddAbility(string abilityId, int level = 1)
        {
            var abilityDef = AbilityList.GetAbility(abilityId) ?? throw new ArgumentException("Invalid ability ID");
            var ability = abilityDef.GetAbility();
            ability.Level = level;
            ability.TargetXP = abilityDef.RequiredXpForLevelUp(ability.Level);
            m_abilities[ability.Key] = ability;
        }

        public Ability? GetAbility(string key)
        {
            return m_abilities.GetValueOrDefault(key);
        }

        public List<Ability> GetAbilities()
        {
            return m_abilities.Values.ToList();
        }

        public bool UpdateAbility(string key, int level, int xp)
        {
            if (!m_abilities.ContainsKey(key))
                return false;
            var ability = m_abilities[key];
            ability.Level = level;
            ability.XP = xp;
            ability.TargetXP = AbilityList.GetAbility(ability.Key)!.RequiredXpForLevelUp(level);
            return true;
        }

        public enum AddXPResult { NotFound, InvalidAmount, Added, LevelIncreased, AlreadyMax }

        public AddXPResult AddXP(string key, int amount)
        {
            if (amount < 0)
                return AddXPResult.InvalidAmount;
            
            var ability = GetAbility(key);
            if (ability == null)
                return AddXPResult.NotFound;
            if (ability.Level == ability.MaxLevel) // == so that levels above max are pruned later
                return AddXPResult.AlreadyMax;
            if (ability.Level >= ability.MaxLevel)
            {
                ability.Level = ability.MaxLevel;
                ability.XP = 0;
                ability.TargetXP = 0;
                return AddXPResult.AlreadyMax;
            }
            bool lvlUp = ability.AddXP(amount);
            if (!lvlUp)
                return AddXPResult.Added;
            while (ability.XP >= ability.TargetXP)
            {
                ability.Level++;
                ability.XP -= ability.TargetXP;
                ability.TargetXP = AbilityList.GetAbility(ability.Key)!.RequiredXpForLevelUp(ability.Level);
            }
            return AddXPResult.LevelIncreased;
        }
    }

    public class Ability(string key)
    {
        public string Key { get; set; } = key;
        public string Name { get; set; } = key;
        public int Level { get; set; } = 1;
        public int XP { get; set; } = 0;
        public int TargetXP { get; set; } = 100;
        public int MaxLevel { get; init; } = int.MaxValue;

        public bool AddXP(int amount)
        {
            XP += amount;
            return XP >= TargetXP;
        }
    }

    public record AbilityAddedMessage(Entity Owner, string AbilityId) : IMessage
    {
        public string BuildMessage() => $"{Owner.GetName()} gained ability '{AbilityList.GetAbility(AbilityId)?.Name ?? "??"}'";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.High;
    }
}
