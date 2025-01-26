using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.Achievements
{
    internal record AbilityLevelReward(List<string> AbilityIds, int Level) : IAchievementReward
    {
        public string Description => $"Minimum level {Level} for {String.Join(", ", AbilityIds.Select(a => AbilityList.GetAbility(a)?.Name ?? "??"))}";

        public bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback)
        {
            var abilitiesComp = entity.GetComponent<AbilitiesComponent>();
            if (abilitiesComp == null)
                return false;
            bool result = true;
            foreach (var abilityId in AbilityIds)
            {
                var ability = abilitiesComp.GetAbility(abilityId);
                if (ability == null)
                {
                    result = false;
                }
                else if (ability.Level < Level)
                {
                    abilitiesComp.UpdateAbility(abilityId, Level, 0);
                }
            }
            return result;
        }
    }
}
