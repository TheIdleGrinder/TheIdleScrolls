using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;

namespace TheIdleScrolls_Core.Resources
{
    public static class AbilityList
    {
        static Dictionary<string, AbilityDefinition> s_Abilities = [];

        private static void GenerateAbilities()
        {
            List<AbilityDefinition> abilities = [];
            abilities.Add(new AbilityDefinition(Abilities.Axe) { Name = Properties.LocalizedStrings.AXE });
            abilities.Add(new AbilityDefinition(Abilities.Blunt) { Name = Properties.LocalizedStrings.BLN });
            abilities.Add(new AbilityDefinition(Abilities.LongBlade) { Name = Properties.LocalizedStrings.LBL });
            abilities.Add(new AbilityDefinition(Abilities.Polearm) { Name = Properties.LocalizedStrings.POL });
            abilities.Add(new AbilityDefinition(Abilities.ShortBlade) { Name = Properties.LocalizedStrings.SBL });

            abilities.Add(new AbilityDefinition(Abilities.LightArmor) { Name = Properties.LocalizedStrings.LAR });
            abilities.Add(new AbilityDefinition(Abilities.HeavyArmor) { Name = Properties.LocalizedStrings.HAR });

            abilities.Add(new AbilityDefinition(Abilities.Crafting) { Name = Properties.LocalizedStrings.ABL_CRAFT });

            foreach (var ability in abilities)
            {
                s_Abilities.Add(ability.Key, ability);
            }
        }

        public static List<AbilityDefinition> GetAbilities()
        {
            if (s_Abilities.Count == 0)
            {
                GenerateAbilities();
            }
            return [.. s_Abilities.Values];
        }

        public static AbilityDefinition? GetAbility(string key)
        {
            if (s_Abilities.Count == 0)
            {
                GenerateAbilities();
            }
            return s_Abilities.GetValueOrDefault(key);
        }
    }

    public class AbilityDefinition(string key)
    {
        public string Key { get; set; } = key;
        public string Name { get; set; } = key;
        public int MaxLevel { get; set; } = 9999;
        public Func<int, int> RequiredXpForLevelUp { get; set; } = (x) => 60 * x;

        public Ability GetAbility()
        {
            return new(key) { Name = Name };
        }
    }

}