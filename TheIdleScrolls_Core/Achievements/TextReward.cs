using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Achievements
{
    internal record TextReward(string Text) : IAchievementReward
    {
        public string Description => Text;
        public bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback) => true;
    }
}
