using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public enum QuestId { GettingStarted, Story }

    public class QuestProgressComponent : IComponent
    {
        public Dictionary<QuestId, int> Quests = new();

        public Dictionary<string, dynamic> TemporaryData = new();

        public int GetQuestProgress(QuestId quest, int defaultValue = -1)
        {
            return Quests.GetValueOrDefault(quest, defaultValue);
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

        public void StoreTemporaryData(string key, dynamic value)
        {
            TemporaryData[key] = value;
        }

        public T? RetrieveTemporaryData<T>(string key)
        {
            return TemporaryData.ContainsKey(key)
                ? (T)TemporaryData[key]
                : default;
        }
    }
}
