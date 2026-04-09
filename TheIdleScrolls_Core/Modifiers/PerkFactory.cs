using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Properties;

namespace TheIdleScrolls_Core.Modifiers
{
    public static class PerkFactory
    {
        public static Perk MakeAbilityLevelBasedPerk(string id,
                                                     string name,
                                                     string description,
                                                     string ability,
                                                     ModifierType modType,
                                                     double valuePerLevel, 
                                                     IEnumerable<string> localTags,
                                                     IEnumerable<string> globalTags)
        {
            return MakeAbilityLevelBasedMultiModPerk(id, name, description, 
                new() { ability }, new() { modType }, new() { valuePerLevel }, new() { localTags }, new() { globalTags }
            );
        }

        public static Perk MakeAbilityLevelBasedMultiModPerk(string id,
                                                             string name,
                                                             string description,
                                                             List<string> abilities,
                                                             List<ModifierType> modTypes,
                                                             List<double> valuesPerLevel,
                                                             List<IEnumerable<string>> localTags,
                                                             List<IEnumerable<string>> globalTags,
                                                             bool alwaysActive = false)
        {
            return new(
                id,
                name,
                description,
                new() { UpdateTrigger.AbilityIncreased },
                delegate (int level, Entity entity, World world, Coordinator coordinator)
                {
                    List<Modifier> mods = new();
                    for (int i = 0; i < modTypes.Count; i++)
                    {
                        int abilityLevel = entity.GetComponent<AbilitiesComponent>()?.GetAbility(abilities[i])?.Level ?? 0;
                        if (abilityLevel == 0)
                            continue;
                        double bonus = abilityLevel * valuesPerLevel[i];
                        mods.Add(new($"{id}_{i}", modTypes[i], level * bonus, localTags[i].Append(abilities[i]).ToHashSet(), globalTags[i].ToHashSet()));
                    }
                    return mods;
                }
            )
            {
                Permanent = alwaysActive
            };
        }

        public static Perk MakeCharacterLevelBasedPerk(string id,
                                                       string name,
                                                       string description,
                                                       ModifierType modType,
                                                       double valuePerLevel,
                                                       IEnumerable<string> localTags,
                                                       IEnumerable<string> globalTags,
                                                       bool alwaysActive = false)
        {
            return MakeCharacterLevelBasedMultiModPerk(id, name, description, 
                new() { modType }, new() { valuePerLevel }, new() { localTags }, new() { globalTags }, alwaysActive
            );
        }

        public static Perk MakeCharacterLevelBasedMultiModPerk(string id,
                                                       string name,
                                                       string description,
                                                       List<ModifierType> modTypes,
                                                       List<double> valuesPerLevel,
                                                       List<IEnumerable<string>> localTags, 
                                                       List<IEnumerable<string>> globalTags, 
                                                       bool alwaysActive = false)
        {
            return new(
                id,
                name,
                description,
                new() { UpdateTrigger.LevelUp },
                delegate (int level, Entity entity, World world, Coordinator coordinator)
                {
                    int charLevel = entity.GetComponent<LevelComponent>()?.Level ?? 0;
                    List<Modifier> mods = new();
                    for (int i = 0; i < modTypes.Count; i++)
                    {
                        double bonus = charLevel * valuesPerLevel[i];
                        mods.Add(new($"{id}_{i}", modTypes[i], level * bonus, localTags[i].ToHashSet(), globalTags[i].ToHashSet()));
                    }
                    return mods;
                }
            )
            {
                Permanent = alwaysActive
            };
        }

        public static Perk MakeStaticPerk(string id,
                                          string name,
                                          string description,
                                          ModifierType modType,
                                          double value,
                                          IEnumerable<string> localTags,
                                          IEnumerable<string> globalTags,
                                          bool alwaysActive = false,
                                          int maxLevel = 1)
        {
            return new(
                id,
                name,
                description,
                new(),
                delegate (int level, Entity entity, World world, Coordinator coordinator)
                {
                    return new() { new(id, modType, level * value, localTags.ToHashSet(), globalTags.ToHashSet()) };
                }
            )
            {
                Permanent = alwaysActive,
                MaxLevel = maxLevel
            };
        }

        public static Perk MakeStaticMultiModPerk(string id,
                                                  string name,
                                                  string description,
                                                  List<ModifierType> modTypes,
                                                  List<double> values,
                                                  List<IEnumerable<string>> localTags,
                                                  List<IEnumerable<string>> globalTags,
                                                  bool alwaysActive = false,
                                                  int maxLevel = 1)
        {
            return new(
                id,
                name,
                description,
                new(),
                delegate (int level, Entity entity, World world, Coordinator coordinator)
                {
                    List<Modifier> mods = new();
                    for (int i = 0; i < modTypes.Count; i++)
                    {
                        mods.Add(new($"{id}_{i}", modTypes[i], level * values[i], localTags[i].ToHashSet(), globalTags[i].ToHashSet()));
                    }
                    return mods;
                }
            )
            {
                Permanent = alwaysActive,
                MaxLevel = maxLevel
            };
        }

        public static Perk MakePercentLifeRegPerks(string id, string name, double percentage)
        {
            return new(
                id,
                name,
                $"Regenerate {percentage:0.##%} of max life per second",
                [],
                delegate (int level, Entity entity, World world, Coordinator coordinator)
                {
                    double lifePool = entity.GetComponent<LifePoolComponent>()?.Maximum ?? 1.0;
                    double regenAmount = percentage * lifePool;
                    return [ new($"{id}_regen", ModifierType.AddBase, regenAmount, [Tags.LifeRegeneration], []) ];
                }
            )
            {
                Permanent = true
            };
        }

        public static Perk MakeFlatDamageReductionPerk(string id, string name, double percentage)
        {
            return new(
                id,
                name,
                $"Incoming damage is reduced by {percentage:0.##%} of maximum life",
                [],
                delegate (int level, Entity entity, World world, Coordinator coordinator)
                {
                    double lifePool = entity.GetComponent<LifePoolComponent>()?.Maximum ?? 1.0;
                    double prevention = percentage * lifePool;
                    return [new($"{id}_prevention", ModifierType.AddBase, prevention, [Tags.DamageReduction], [])];
                }
            )
            {
                Permanent = true
            };
        }

        public static Perk MakeDefenseLayerPerk(string id, string name, double layers)
        {
            return new(
                id,
                name,
                $"Can lose at most {1 / (layers + 1):0.##%} of maximum life per incoming hit",
                [],
                delegate (int level, Entity entity, World world, Coordinator coordinator)
                {
                    return [new($"{id}_layers", ModifierType.AddBase, layers, [Tags.DefensiveLayers], [])];
                }
            )
            {
                Permanent = true
            };
        }
    }

    public static class PerkExtensions
    {
        public static Perk WithCategories(this Perk perk, params string[] categories)
        {
            perk.Categories.AddRange(categories.Where(c => !string.IsNullOrEmpty(c)));
            return perk;
        }

        public static Perk WithConditionFunc(this Perk perk, Func<int, IPerkCondition?> conditionFunc)
        {
            perk.ConditionFunc = conditionFunc;
            return perk;
        }

        static string[] CategoryLookup = 
        [
            LocalizedStrings.BasicPerks,
            LocalizedStrings.Weapons,
            LocalizedStrings.Armours,
            LocalizedStrings.FightingStyles,
            LocalizedStrings.UNARMED,
            LocalizedStrings.UNARMORED,
            "Monk"
        ];

        public static int GetCategoryOrderPrefix(this Perk perk)
        {
            if (perk.Categories.Count == 0)
                return 0;

            string category = perk.Categories[0];
            int index = Array.IndexOf(CategoryLookup, category);
            if (index == -1)
                return 0;
            return index + 1;
        }
    }
}
