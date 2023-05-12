using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Quests;
using QuestStates = TheIdleScrolls_Core.Components.QuestStates;

namespace TheIdleScrolls_Core.Systems
{

    public class QuestSystem : AbstractSystem
    {
        Entity? m_player = null;

        readonly List<AbstractQuest> m_quests;

        public QuestSystem()
        {
            m_quests = new List<AbstractQuest>() 
            { 
                new GettingStartedQuest(),
                new StoryQuest()
            };
        }

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();

            if (m_player == null)
                return;

            //HandleGettingStarted(world, coordinator);
            GettingStartedQuest gettingStartedQuest = new();
            gettingStartedQuest.UpdateEntity(m_player, coordinator, world, dt, 
                (IMessage message) => 
                {
                    coordinator.PostMessage(this, message as dynamic);
                }
            );

            foreach (AbstractQuest quest in m_quests)
            {
                quest.UpdateEntity(m_player, coordinator, world, dt,
                    (IMessage message) =>
                    {
                        coordinator.PostMessage(this, message as dynamic);
                    }
                );
            }
        }
    }

    public class StoryMessage : IMessage
    {
        readonly string content = "";

        public StoryMessage(string content)
        {
            this.content = content;
        }

        string IMessage.BuildMessage()
        {
            return content;
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.VeryHigh;
        }
    }

    /// <summary>
    /// Potentially placeholder. Used to notify other systems of the fact that a quest state has changed.
    /// </summary>
    public class QuestProgressMessage : IMessage
    {
        public QuestId Quest { get; }
        public int Progress { get; }
        public string? QuestMessage { get; }

        string IMessage.BuildMessage()
        {
            if (QuestMessage != null)
            {
                return $"{Quest}|{Progress}: {QuestMessage}";
            }
            else
            {
                return $"Quest progressed: {Quest}|{Progress}";
            }
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return (QuestMessage != null) 
                ? IMessage.PriorityLevel.VeryHigh 
                : IMessage.PriorityLevel.Debug;
        }

        public QuestProgressMessage(QuestId quest, int progress, string? message = null)
        {
            Quest = quest;
            Progress = progress;
            QuestMessage = message;
        }
    }

    public class FeatureStateMessage : IMessage
    {
        public GameFeature Feature { get; }
        public bool Enabled { get; }

        string IMessage.BuildMessage()
        {
            return $"Feature '{Feature}' has been {(Enabled ? "en" : "dis")}abled";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }

        public FeatureStateMessage(GameFeature feature, bool enabled)
        {
            Feature = feature;
            Enabled = enabled;
        }
    }

}
