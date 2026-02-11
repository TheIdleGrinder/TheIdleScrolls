using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Achievements.Rewards
{
    public class MultiReward(List<IAchievementReward> rewards, string customDescription = "") : IAchievementReward
    {
        public List<(IAchievementReward, bool)> Rewards { get; } = [.. rewards.Select(r => (r, false))];

        public string Description => (customDescription.Length > 0) 
            ? customDescription 
            : String.Join('\n', Rewards.Select(r => r.Item1.Description));

        public bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback)
        {
            foreach (var (reward, given) in Rewards)
            {
                if (!given && reward.GiveReward(entity, world, postMessageCallback))
                {
                    Rewards[Rewards.IndexOf((reward, given))] = (reward, true);
                }
            }
            return Rewards.All(r => r.Item2);
        }
    }
}
