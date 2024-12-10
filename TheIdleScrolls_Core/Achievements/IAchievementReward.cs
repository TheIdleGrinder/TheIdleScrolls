using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Achievements
{
    public interface IAchievementReward
    {
        /// <summary>
        /// Attempts to give the reward to the entity or activate it in the world.
        /// </summary>
        /// <param name="entity">Target entity</param>
        /// <param name="world">Target world</param>
        /// <param name="postMessageCallback">Used to post appropriate messages (e.g. AbilityAdded)</param>
        /// <returns>true if the reward was successfully given, false is returned for example if player is missing a required component</returns>
        bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback);

        string Description { get; }
    }
}
