using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Modifiers
{
    public static class PerkFactory
    {
        public static Perk MakeOffensiveAbilityBasedPerk(string id, string ability, double damagePerLevel, double speedPerLevel)
        {
            bool dmg = damagePerLevel != 0;
            bool aps = speedPerLevel != 0;
            return new(
                id, 
                $"Ability: {ability.Localize()}",
                $"For each ability level: {(dmg ? $"{damagePerLevel:0.#%} more damage " : "")}" + 
                    $"{(dmg && aps ? "and " : "")}{(aps ? $"{speedPerLevel:0.#%} more attack speed " : "")}" +
                    $"with {ability.Localize()} weapons",
                new() { UpdateTrigger.AbilityIncreased },
                delegate (int level, Entity entity, World world, Coordinator coordinator)
                {
                    int abilityLevel = entity.GetComponent<AbilitiesComponent>()?.GetAbility(ability)?.Level ?? 0;
                    List<Modifier> mods = new();
                    if (dmg)
                    {
                        double bonus = Math.Pow(1.0 + damagePerLevel, abilityLevel) - 1.0;
                        mods.Add(new(id + "_dmg", ModifierType.More, level * bonus, new() { ability, Definitions.Tags.Damage }, new()));
                    }
                    if (aps)
                    {
                        double bonus = Math.Pow(1.0 + speedPerLevel, abilityLevel) - 1.0;
                        mods.Add(new(id + "_aps", ModifierType.More, level * bonus, new() { ability, Definitions.Tags.AttackSpeed }, new()));
                    }
                    return mods;
                }
            );
        }

        public static Perk MakeDefensiveAbilityBasedPerk(string id, string ability, double defensePerLevel)
        {
            return new(
                id,
                $"Ability: {ability.Localize()}",
                    $"For each ability level: {defensePerLevel:0.#%} more armor and evasion rating with {ability.Localize()}",
                new() { UpdateTrigger.AbilityIncreased },
                delegate (int level, Entity entity, World world, Coordinator coordinator)
                {
                    int abilityLevel = entity.GetComponent<AbilitiesComponent>()?.GetAbility(ability)?.Level ?? 0;
                    List<Modifier> mods = new();
                    double bonus = Math.Pow(1.0 + defensePerLevel, abilityLevel) - 1.0;
                    mods.Add(new(id + "_def", ModifierType.More, level * bonus, new() { ability, Definitions.Tags.Defense }, new()));
                    return mods;
                }
            );
        }

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
    }
}
