using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Achievements.Rewards
{
    public class MultiReward : IAchievementReward
    {
        public List<IAchievementReward> Rewards { get; }
        bool[] Given = [];
        string CustomDescription;

        public MultiReward(List<IAchievementReward> rewards, string customDescription = "")
        {
            Rewards = rewards;
            Given = new bool[rewards.Count];
            CustomDescription = customDescription;            
        }

        public string Description => (CustomDescription.Length > 0) 
            ? CustomDescription
            : String.Join('\n', Rewards.Select(r => r.Description));

        public bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback)
        {
            for (int i = 0; i < Rewards.Count; i++)
            {
                if (!Given[i])
                {
                    Given[i] = (Rewards[i] as dynamic).GiveReward(entity, world, postMessageCallback);
                }
            }
            return Given.All(b => b);
        }
    }
}
