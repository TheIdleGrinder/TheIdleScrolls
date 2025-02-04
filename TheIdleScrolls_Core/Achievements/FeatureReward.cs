using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

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
            playerComp.AvailableFeatures.Add(Feature);
            return true;
        }
    }
}
