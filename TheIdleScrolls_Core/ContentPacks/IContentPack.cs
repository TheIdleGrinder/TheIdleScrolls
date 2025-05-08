using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Quests;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.ContentPacks
{
	public interface IContentPack
	{
		public string Id { get; }
		public string Name { get; }
		public string Description { get; }
		
		public List<AbstractQuest> Quests { get; }
		public List<ItemFamilyDescription> ItemFamilies { get; }
		public List<Achievement> Achievements { get; }

		public static void Activate(IContentPack pack)
		{
			foreach (ItemFamilyDescription family in pack.ItemFamilies)
			{
				if (ItemList.ItemFamilies.Any(f => f.Id == family.Id))
					throw new Exception($"Duplicate item family: {family.Id}");
			}

			foreach (Achievement achievement in pack.Achievements)
			{
				if (AchievementList.GetAllAchievements().Any(a => a.Id == achievement.Id))
					throw new Exception($"Duplicate achievement: {achievement.Id}");
			}

			ItemList.ItemFamilies.AddRange(pack.ItemFamilies);
			AchievementList.GetAllAchievements().AddRange(pack.Achievements);
		}


	}
}
