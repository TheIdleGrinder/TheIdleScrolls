using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Quests;

namespace TheIdleScrolls_Core.Systems
{

    public class QuestSystem : AbstractSystem
    {
        Entity? Player = null;

        readonly List<AbstractQuest> Quests;

        public QuestSystem()
        {
            Quests = [
                new GettingStartedQuest(),
                new StoryQuest(),
                new EndgameQuest()
            ];
        }

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            Player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();

            if (Player == null)
                return;

            foreach (AbstractQuest quest in Quests)
            {
                quest.UpdateEntity(Player, coordinator, world, dt,
                    (IMessage message) =>
                    {
                        coordinator.PostMessage(this, message as dynamic);
                    }
                );
            }
        }
    }

    /// <summary>
    /// Potentially placeholder. Used to notify other systems of the fact that a quest state has changed.
    /// </summary>
    public record QuestProgressMessage(QuestId Quest, int Progress, string? QuestMessage = null, string? MessageTitle = null) : IMessage
    {
        string IMessage.BuildMessage()
        {
            if (QuestMessage != null && QuestMessage != string.Empty)
            {
                string result = $"{Quest}|{Progress}: ";
                if (MessageTitle != null && MessageTitle != string.Empty)
                {
                    result += MessageTitle + " - ";
                }
                return result + QuestMessage;
            }
            else
            {
                return $"Quest progressed: {Quest}|{Progress}";
            }
        }

        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }

    public record FeatureStateMessage(GameFeature Feature, bool Enabled) : IMessage
    {
        string IMessage.BuildMessage() => $"Feature '{Feature}' has been {(Enabled ? "en" : "dis")}abled";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }

}
