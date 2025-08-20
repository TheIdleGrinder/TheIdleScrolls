using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.ContentPacks
{
	public class Adventure(string id, string name, List<IContentPack> content)
	{
		public string Id { get; init; } = id;
		public string Name { get; init; } = name;
		public List<IContentPack> ContentPacks { get; init; } = content;

		// For now, we keep track of active content packs globally.
		readonly static List<string> ActiveContentPacks = [];

		public bool Activate()
		{
			// TODO: Add addional content packs that were unlocked earlier
			ResetContent();
			foreach (var pack in ContentPacks)
			{
				IContentPack.Activate(pack);
			}
			return true;
		}

		public static void ResetContent()
		{
			ActiveContentPacks.Clear();
			AbilityList.Reset();
			AchievementList.Reset();
			DungeonList.Reset();
			QuestList.Reset();
		}
	}
}
