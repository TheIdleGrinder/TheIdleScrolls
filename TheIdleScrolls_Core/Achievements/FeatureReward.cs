using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrolls_Core.Achievements
{
    internal record FeatureReward(GameFeature Feature, string FeatureDescription) : IAchievementReward
    {
        public string Description => FeatureDescription;

        public bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback)
        {
            var playerComp = entity.GetComponent<PlayerComponent>();
            if (playerComp == null)
                return false;
            bool added = playerComp.AvailableFeatures.Add(Feature);
            if (added)
            {
                postMessageCallback(new FeatureStateMessage(Feature, true));
            }
            return true;
        }
    }
}
