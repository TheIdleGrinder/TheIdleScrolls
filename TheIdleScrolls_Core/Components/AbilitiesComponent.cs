using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Components
{
    public class AbilitiesComponent : IComponent
    {
        Dictionary<string, Ability> m_abilities = new();
        static Func<int, int> s_xpFunction = (x) => 60 * x;

        public AbilitiesComponent() // CornerCut: Abilities should be assigned somewhere else
        {
            foreach (string key in ItemFactory.GetAllItemFamilyIds())
            {
                string? className = ItemFactory.GetItemFamilyName(key);
                if (className == null)
                {
                    Debug.WriteLine($"Failed to retrieve class name for {key}");
                    continue;
                }
                Ability ability = new Ability(key, className);
                ability.Level = 10;
                AddAbility(ability);
            }
        }

        public void AddAbility(Ability ability)
        {
            ability.TargetXP = s_xpFunction(ability.Level);
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
            ability.TargetXP = s_xpFunction(level);
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
                ability.TargetXP = s_xpFunction(ability.Level);
            }
            return AddXPResult.LevelIncreased;
        }
    }

    public class Ability
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public int Level { get; set; } = 1;
        public int XP { get; set; } = 0;
        public int TargetXP { get; set; } = 100;

        public Ability(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public bool AddXP(int amount)
        {
            XP += amount;
            return XP >= TargetXP;
        }
    }
}
