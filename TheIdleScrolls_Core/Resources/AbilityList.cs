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
            static int regularXpCurve(int x) => 60 * (int)Math.Pow(x, 2);
            static int slowXpCurve(int x)    => 120 * (int)Math.Pow(x, 2);
            int regularMaxLevel              = 200;
            int slowMaxLevel                 = 100;


            List<AbilityDefinition> abilities = [];
            abilities.Add(new AbilityDefinition(Abilities.Axe)
            {
                Name = Properties.LocalizedStrings.AXE,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve
            });
            abilities.Add(new AbilityDefinition(Abilities.Blunt) 
            { 
                Name = Properties.LocalizedStrings.BLN,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve
            });
            abilities.Add(new AbilityDefinition(Abilities.LongBlade) 
            { 
                Name = Properties.LocalizedStrings.LBL,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve
            });
            abilities.Add(new AbilityDefinition(Abilities.Polearm) 
            { 
                Name = Properties.LocalizedStrings.POL,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve
            });
            abilities.Add(new AbilityDefinition(Abilities.ShortBlade) 
            { 
                Name = Properties.LocalizedStrings.SBL,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve
            });
            abilities.Add(new AbilityDefinition(Abilities.Unarmed) 
            { Name = Properties.LocalizedStrings.UNARMED,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve
            });

            abilities.Add(new AbilityDefinition(Abilities.LightArmor) 
            { 
                Name = Properties.LocalizedStrings.LAR,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve
            });
            abilities.Add(new AbilityDefinition(Abilities.HeavyArmor) 
            { 
                Name = Properties.LocalizedStrings.HAR,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve
            });
            abilities.Add(new AbilityDefinition(Abilities.Unarmored) 
            { 
                Name = Properties.LocalizedStrings.UNARMORED,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve
            });

            abilities.Add(new AbilityDefinition(Abilities.Crafting)
            {
                Name = Properties.LocalizedStrings.ABL_CRAFT,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = (int x) => 50 * x
            });

            abilities.Add(new AbilityDefinition(Abilities.DualWield)
            {
                Name = Properties.LocalizedStrings.ABL_DualWield,
                MaxLevel = slowMaxLevel,
                RequiredXpForLevelUp = slowXpCurve
            });
            abilities.Add(new AbilityDefinition(Abilities.Shielded)
            {
                Name = Properties.LocalizedStrings.ABL_Shielded,
                MaxLevel = slowMaxLevel,
                RequiredXpForLevelUp = slowXpCurve
            });
            abilities.Add(new AbilityDefinition(Abilities.SingleHanded)
            {
                Name = Properties.LocalizedStrings.ABL_SingleHanded,
                MaxLevel = slowMaxLevel,
                RequiredXpForLevelUp = slowXpCurve
            });
            abilities.Add(new AbilityDefinition(Abilities.TwoHanded)
            {
                Name = Properties.LocalizedStrings.ABL_TwoHanded,
                MaxLevel = slowMaxLevel,
                RequiredXpForLevelUp = slowXpCurve
            });

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
        public string Name { get; set; } = "??";
        public int MaxLevel { get; set; } = 9999;
        public Func<int, int> RequiredXpForLevelUp { get; set; } = (x) => 60 * x;

        public Ability GetAbility()
        {
            return new(key) { Name = Name };
        }
    }

}