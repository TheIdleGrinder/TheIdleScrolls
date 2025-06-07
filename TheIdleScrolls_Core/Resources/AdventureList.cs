using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.ContentPacks;

namespace TheIdleScrolls_Core.Resources
{
	public class AdventureList
	{
		public static readonly string DefaultAdventureId = "warrior_adventure";

		private static List<Adventure> s_Adventures = [
			WarriorAdventure
		];

		public static Adventure WarriorAdventure = new("warrior_adventure", "The Warrior", [
			new FightingStylesContentPack(),
			new CraftingContentPack(),
			new WarriorGettingStartedContentPack(),
			new WarriorCampaignContentPack(),
			new WarriorEndgameContentPack(),
			new CoinAchievementPack(),
			new AdditionalAchievementPack(),
			new EquipmentAchievementPack(),
			new UnderequippedAchievementPack(),
			new SpeedrunAchievementPack()
		]);

		public static Adventure? GetAdventure(string id)
		{
			return s_Adventures.FirstOrDefault(a => a.Id == id);
		}

		public static List<Adventure> GetAllAdventures()
		{
			return s_Adventures;
		}
	}
}
