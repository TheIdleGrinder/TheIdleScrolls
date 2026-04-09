using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Modifiers
{
    public record PerkLevelPerkCondition(string PerkId, string PerkName, int Level) : IPerkCondition
    {
        public string GetDescription()
        {
            if (Level == 1)
                return $"Requires '{PerkName}' perk";
            else
                return $"Requires level {Level} '{PerkName}' perk";
        }

        public bool IsSatisfied(Entity entity)
        {
            int level = entity.GetComponent<PerksComponent>()?.GetPerkLevel(PerkId) ?? 0;
            return level >= Level;
        }
    }

    public record AbilityLevelPerkCondition(string AbilityId, string AbilityName, int Level) : IPerkCondition
    {
        public string GetDescription()
        {
            return $"Requires level {Level} '{AbilityName}' ability";
        }
        public bool IsSatisfied(Entity entity)
        {
            int level = entity.GetComponent<AbilitiesComponent>()?.GetAbility(AbilityId)?.Level ?? 0;
            return level >= Level;
        }
    }
}
