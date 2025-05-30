using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Quests;

namespace TheIdleScrolls_Core.Resources
{
	public static class QuestList
	{
		readonly static List<AbstractQuest> s_Quests = [];

		public static List<AbstractQuest> GetAllQuests() => s_Quests;

		public static void Reset()
		{
			s_Quests.Clear();
		}
	}
}
