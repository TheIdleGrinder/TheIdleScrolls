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
    internal record AbilityReward(string AbilityId) : IAchievementReward
    {
        public string Description => $"'{AbilityList.GetAbility(AbilityId)?.Name ?? "??"}' Ability";

        public bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback)
        {
            var abiltiesComp = entity.GetComponent<AbilitiesComponent>();
            if (abiltiesComp == null)
                return false;
            if (abiltiesComp.GetAbility(AbilityId) == null)
            {
                abiltiesComp.AddAbility(AbilityId);
                postMessageCallback(new AbilityAddedMessage(entity, AbilityId));
            }            
            return true;
        }
    }
}
