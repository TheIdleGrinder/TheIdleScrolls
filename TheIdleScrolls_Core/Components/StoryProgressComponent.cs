using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public enum QuestId { GettingStarted, FinalFight }

    public class Quest
    {
        public QuestId Id { get; }
        public Dictionary<int, string> Messages { get; }

        public Quest(QuestId id, Dictionary<int, string> messages)
        {
            Id = id;
            Messages = messages;
        }
    }

    namespace QuestStates
    {
        public enum GettingStarted { Inventory, Armor, Abilities, Travel }
        public enum FinalFight { None = -1, NotStarted, Slowing, Pause, End, Finished }
    }

    public class QuestProgressComponent : IComponent
    {
        public FinalFight FinalFight = new();

        public Dictionary<QuestId, int> Quests = new();

        public int GetQuestProgress(QuestId quest)
        {
            return Quests.GetValueOrDefault(quest, -1);
        }

        public void SetQuestProgress(QuestId quest, int progress)
        {
            if (progress >= 0)
            {
                Quests[quest] = progress;
            }
            else
            {
                Quests.Remove(quest);
            }
        }

        public void SetQuestProgress<T>(QuestId quest, T progress) where T : Enum
        {
            SetQuestProgress(quest, Convert.ToInt32(progress));
        }
    }

    public struct FinalFight
    {
        public DateTime StartTime = DateTime.MinValue;

        public FinalFight() {}
    }
}
