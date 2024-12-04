using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.Components
{
    public class AbilitiesComponent : IComponent
    {
        readonly Dictionary<string, Ability> m_abilities = new();

        public AbilitiesComponent()
        {
            //var fightingAbilities = Definitions.Abilities.Weapons.Concat(Definitions.Abilities.Armors);
            //// Weapons and armor
            //foreach (string key in fightingAbilities)
            //{
            //    Ability ability = new(key)
            //    {
            //        Level = 10
            //    };
            //    AddAbility(ability);
            //}
            //// Crafting
            //Ability crafting = new(Definitions.Abilities.Crafting) { Level = 1 };
            //AddAbility(crafting);
        }

        public void AddAbility(string abilityId)
        {
            var abilityDef = AbilityList.GetAbility(abilityId) ?? throw new ArgumentException("Invalid ability ID");
            var ability = abilityDef.GetAbility();
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

        public enum AddXPResult { NotFound, InvalidAmount, Added, LevelIncreased }

        public AddXPResult AddXP(string key, int amount)
        {
            if (amount < 0)
                return AddXPResult.InvalidAmount;
            
            var ability = GetAbility(key);
            if (ability == null)
                return AddXPResult.NotFound;
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

        public bool AddXP(int amount)
        {
            XP += amount;
            return XP >= TargetXP;
        }
    }
}
