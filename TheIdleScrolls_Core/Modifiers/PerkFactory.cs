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
                delegate (Entity entity, World world, Coordinator coordinator)
                {
                    int level = entity.GetComponent<AbilitiesComponent>()?.GetAbility(ability)?.Level ?? 0;
                    List<Modifier> mods = new();
                    if (dmg)
                    {
                        double bonus = Math.Pow(1.0 + damagePerLevel, level) - 1.0;
                        mods.Add(new(id + "_dmg", ModifierType.More, bonus, new() { ability, Definitions.Tags.Damage }));
                    }
                    if (aps)
                    {
                        double bonus = Math.Pow(1.0 + speedPerLevel, level) - 1.0;
                        mods.Add(new(id + "_aps", ModifierType.More, bonus, new() { ability, Definitions.Tags.AttackSpeed }));
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
                delegate (Entity entity, World world, Coordinator coordinator)
                {
                    int level = entity.GetComponent<AbilitiesComponent>()?.GetAbility(ability)?.Level ?? 0;
                    List<Modifier> mods = new();
                    double bonus = Math.Pow(1.0 + defensePerLevel, level) - 1.0;
                    mods.Add(new(id + "_def", ModifierType.More, bonus, new() { ability, Definitions.Tags.Defense }));
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
                                                     IEnumerable<string> tags,
                                                     bool exponentialMore = false)
        {
            return MakeAbilityLevelBasedMultiModPerk(id, name, description, 
                new() { ability }, new() { modType }, new() { valuePerLevel }, new() { tags }, exponentialMore
            );
        }

        public static Perk MakeAbilityLevelBasedMultiModPerk(string id,
                                                             string name,
                                                             string description,
                                                             List<string> abilities,
                                                             List<ModifierType> modTypes,
                                                             List<double> valuesPerLevel,
                                                             List<IEnumerable<string>> tags,
                                                             bool exponentialMore = false)
        {
            return new(
                id,
                name,
                description,
                new() { UpdateTrigger.AbilityIncreased },
                delegate (Entity entity, World world, Coordinator coordinator)
                {
                    List<Modifier> mods = new();
                    for (int i = 0; i < modTypes.Count; i++)
                    {
                        int level = entity.GetComponent<AbilitiesComponent>()?.GetAbility(abilities[i])?.Level ?? 0;
                        double bonus = level * valuesPerLevel[i];
                        if (modTypes[i] == ModifierType.More && exponentialMore)
                            bonus = Math.Pow(1.0 + valuesPerLevel[i], level) - 1.0;
                        mods.Add(new($"{id}_{i}", modTypes[i], bonus, tags[i].Append(abilities[i]).ToHashSet()));
                    }
                    return mods;
                }
            );
        }

        public static Perk MakeCharacterLevelBasedPerk(string id,
                                                       string name,
                                                       string description,
                                                       ModifierType modType,
                                                       double valuePerLevel,
                                                       IEnumerable<string> tags,
                                                       bool exponentialMore = false)
        {
            return MakeCharacterLevelBasedMultiModPerk(id, name, description, 
                new() { modType }, new() { valuePerLevel }, new() { tags }, exponentialMore
            );
        }

        public static Perk MakeCharacterLevelBasedMultiModPerk(string id,
                                                       string name,
                                                       string description,
                                                       List<ModifierType> modTypes,
                                                       List<double> valuesPerLevel,
                                                       List<IEnumerable<string>> tags, 
                                                       bool exponentialMore = false)
        {
            return new(
                id,
                name,
                description,
                new() { UpdateTrigger.LevelUp },
                delegate (Entity entity, World world, Coordinator coordinator)
                {
                    int level = entity.GetComponent<LevelComponent>()?.Level ?? 0;
                    List<Modifier> mods = new();
                    for (int i = 0; i < modTypes.Count; i++)
                    {
                        double bonus = level * valuesPerLevel[i];
                        if (modTypes[i] == ModifierType.More && exponentialMore)
                            bonus = Math.Pow(1.0 + valuesPerLevel[i], level) - 1.0;
                        mods.Add(new($"{id}_{i}", modTypes[i], bonus, tags[i].ToHashSet()));
                    }
                    return mods;
                }
            );
        }

        public static Perk MakeStaticPerk(string id,
                                          string name,
                                          string description,
                                          ModifierType modType,
                                          double value,
                                          IEnumerable<string> tags)
        {
            return new(
                id,
                name,
                description,
                new(),
                delegate (Entity entity, World world, Coordinator coordinator)
                {
                    return new() { new(id, modType, value, tags.ToHashSet()) };
                }
            );
        }
    }
}
