using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Quests
{
    public abstract class AbstractQuest
    {
        const int QuestNotFound = -99;

        public QuestId Id => GetId();

        public abstract QuestId GetId();

        public abstract void UpdateEntity(Entity entity, Coordinator coordinator, World world, double dt, Action<IMessage> postMessageCallback);
    }
}
