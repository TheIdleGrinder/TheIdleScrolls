using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;

namespace TheIdleScrolls_Core.Resources
{
    public static class AbilityList
    {
        static Dictionary<string, AbilityDefinition> s_Abilities = [];

        private static void GenerateAbilities()
        {
            List<AbilityDefinition> abilities = [];
            static int regularXpCurve(int x) => 60 * (int)Math.Pow(x, 1.5);     // standard XP curve for abilities that increase over time
            static int slowXpCurve(int x)    => 60 * (int)Math.Pow(2 * x, 1.5); // slower but uses lower cap
            static int perUseXpCurve(int x)  =>  5 * (int)Math.Pow(x, 1.5);    // for abilities that gain XP for each use
            int regularMaxLevel              = 200;
            int slowMaxLevel                 = 100;

            Func<int, List<Modifier>> attackAbilityMods(string id)
            {
                return (x) => [
                    new($"{id}_dmg", ModifierType.More, Stats.AttackDamagePerAbilityLevel * x, [Tags.Damage, id], []),
                    new($"{id}_as", ModifierType.More, Stats.AttackSpeedPerAbilityLevel * x, [Tags.AttackSpeed, id], [])
                ];
            }

            // Offense
            abilities.Add(new AbilityDefinition(Abilities.Archery)
            {
                Name = Properties.LocalizedStrings.ARC,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve,
                ModifiersForLevel = attackAbilityMods(Abilities.Archery)
            });
            abilities.Add(new AbilityDefinition(Abilities.Axe)
            {
                Name = Properties.LocalizedStrings.AXE,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve,
                ModifiersForLevel = attackAbilityMods(Abilities.Axe)
            });
            abilities.Add(new AbilityDefinition(Abilities.Blunt) 
            { 
                Name = Properties.LocalizedStrings.BLN,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve,
                ModifiersForLevel = attackAbilityMods(Abilities.Blunt)
            });
            abilities.Add(new AbilityDefinition(Abilities.LongBlade) 
            { 
                Name = Properties.LocalizedStrings.LBL,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve,
                ModifiersForLevel = attackAbilityMods(Abilities.LongBlade)
            });
            abilities.Add(new AbilityDefinition(Abilities.Polearm) 
            { 
                Name = Properties.LocalizedStrings.POL,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve,
                ModifiersForLevel = attackAbilityMods(Abilities.Polearm)
            });
            abilities.Add(new AbilityDefinition(Abilities.ShortBlade) 
            { 
                Name = Properties.LocalizedStrings.SBL,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve,
                ModifiersForLevel = attackAbilityMods(Abilities.ShortBlade)
            });
            abilities.Add(new AbilityDefinition(Abilities.Unarmed) 
            { Name = Properties.LocalizedStrings.UNARMED,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve,
                ModifiersForLevel = attackAbilityMods(Abilities.Unarmed)
            });

            // Defense
            abilities.Add(new AbilityDefinition(Abilities.LightArmor) 
            { 
                Name = Properties.LocalizedStrings.LAR,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve,
                ModifiersForLevel = (x) => [
                    new($"{Abilities.LightArmor}_def", ModifierType.More, Stats.DefensePerAbilityLevel * x, [Tags.Defense, Abilities.LightArmor], [])
                ]
            });
            abilities.Add(new AbilityDefinition(Abilities.HeavyArmor) 
            { 
                Name = Properties.LocalizedStrings.HAR,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve,
                ModifiersForLevel = (x) => [
                    new($"{Abilities.HeavyArmor}_def", ModifierType.More, Stats.DefensePerAbilityLevel * x, [Tags.Defense, Abilities.HeavyArmor], [])
                ]
            });
            abilities.Add(new AbilityDefinition(Abilities.Unarmored) 
            { 
                Name = Properties.LocalizedStrings.UNARMORED,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = regularXpCurve,
                ModifiersForLevel = (x) => [
                    new($"{Abilities.Unarmored}_def", ModifierType.More, Stats.DefensePerAbilityLevel * x, [Tags.Defense, Abilities.Unarmored], [])
                ]
            });
            abilities.Add(new AbilityDefinition(Abilities.Blocking)
            {
                Name = Properties.LocalizedStrings.ABL_BLOCKING,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = perUseXpCurve,
                ModifiersForLevel = (x) => [
                    new($"{Abilities.Blocking}_blk", ModifierType.More, 0.01 * x, [Tags.BlockChance], [])
                ]
            });

            // Crafting
            abilities.Add(new AbilityDefinition(Abilities.Crafting)
            {
                Name = Properties.LocalizedStrings.ABL_CRAFT,
                MaxLevel = regularMaxLevel,
                RequiredXpForLevelUp = (int x) => 50 * x,
                ModifiersForLevel = (x) => [
                    new($"{Abilities.Crafting}_csw", ModifierType.AddBase, x, [Tags.CraftingStrength], [])
                ]
            });

            // Fighting Styles
            abilities.Add(new AbilityDefinition(Abilities.DualWield)
            {
                Name = Properties.LocalizedStrings.ABL_DUALWIELD,
                MaxLevel = slowMaxLevel,
                RequiredXpForLevelUp = slowXpCurve,
                ModifiersForLevel = (x) => [
                    new($"{Abilities.DualWield}_as", ModifierType.More, 0.005 * x, [Tags.AttackSpeed], [Tags.DualWield])
                ]
            });
            abilities.Add(new AbilityDefinition(Abilities.Shielded)
            {
                Name = Properties.LocalizedStrings.ABL_SHIELDED,
                MaxLevel = slowMaxLevel,
                RequiredXpForLevelUp = slowXpCurve,
                ModifiersForLevel = (x) => [
                    new($"{Abilities.Shielded}_def", ModifierType.More, 0.005 * x, [Tags.Defense], [Tags.Shielded])
                ]
            });
            abilities.Add(new AbilityDefinition(Abilities.SingleHanded)
            {
                Name = Properties.LocalizedStrings.ABL_SINGLEHANDED,
                MaxLevel = slowMaxLevel,
                RequiredXpForLevelUp = slowXpCurve,
                ModifiersForLevel = (x) => [
                    new($"{Abilities.SingleHanded}_hp", ModifierType.More, 0.005 * x, [Tags.HitPoints], [Tags.SingleHanded])
                ]
            });
            abilities.Add(new AbilityDefinition(Abilities.TwoHanded)
            {
                Name = Properties.LocalizedStrings.ABL_TWOHANDED,
                MaxLevel = slowMaxLevel,
                RequiredXpForLevelUp = slowXpCurve,
                ModifiersForLevel = (x) => [
                    new($"{Abilities.TwoHanded}_dmg", ModifierType.More, 0.005 * x, [Tags.Damage], [Tags.TwoHanded])
                ]
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

        public static bool Add(AbilityDefinition ability)
        {
            if (s_Abilities.Count == 0)
			{
				GenerateAbilities();
			}
            return s_Abilities.TryAdd(ability.Key, ability);
		}

        public static void Remove(string key)
        {
			if (s_Abilities.Count == 0)
			{
				GenerateAbilities();
			}
			s_Abilities.Remove(key);
		}

        public static void Reset()
        {
            s_Abilities = [];
			GenerateAbilities();
		}
	}

    public class AbilityDefinition(string key)
    {
        public string Key { get; set; } = key;
        public string Name { get; set; } = "??";
        public int MaxLevel { get; set; } = int.MaxValue;
        public Func<int, int> RequiredXpForLevelUp { get; set; } = (x) => 60 * x;
        public Func<int, List<Modifier>> ModifiersForLevel { get; set; } = (x) => [];

        public Ability GetAbility()
        {
            return new(Key) 
            { 
                Name = Name,
                MaxLevel = MaxLevel,
                Definition = this
            };
        }
    }

}