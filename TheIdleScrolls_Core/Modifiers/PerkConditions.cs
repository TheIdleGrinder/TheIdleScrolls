using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Modifiers
{
    public record PerkLevelPerkCondition(string PerkId, int Level) : IPerkCondition
    {
        public string GetDescription()
        {
            if (Level == 1)
                return $"Requires '{PerkId.Localize()}' perk";
            else
                return $"Requires level {Level} '{PerkId.Localize()}' perk";
        }

        public bool IsSatisfied(Entity entity)
        {
            int level = entity.GetComponent<PerksComponent>()?.GetPerkLevel(PerkId) ?? 0;
            return level >= Level;
        }
    }

    public record AbilityLevelPerkCondition(string AbilityId, int Level) : IPerkCondition
    {
        public string GetDescription()
        {
            return $"Requires level {Level} '{AbilityId.Localize()}' ability";
        }
        public bool IsSatisfied(Entity entity)
        {
            int level = entity.GetComponent<AbilitiesComponent>()?.GetAbility(AbilityId)?.Level ?? 0;
            return level >= Level;
        }
    }
}
