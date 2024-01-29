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
                $"{ability.Localize()}: {id.Localize()}",
                $"{(dmg ? $"{damagePerLevel:0.#%} more damage " : "")}" + 
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
    }
}
