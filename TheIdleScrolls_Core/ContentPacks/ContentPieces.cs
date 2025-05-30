using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Quests;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class AchievementContent(Achievement Achievement) : IContentPiece
	{
		public string Id => "Achievement_" + Achievement.Id;
		public string Name => "Achievement: " + Achievement.Title;
		public bool CanActivate() => !AchievementList.GetAllAchievements().Any(a => a.Id == Achievement.Id);
		public bool Activate()
		{
			if (!CanActivate())
				return false;
			AchievementList.GetAllAchievements().Add(Achievement);
			return true;
		}
		public bool Deactivate()
		{
			AchievementList.GetAllAchievements().RemoveAll(i => i.Id == Achievement.Id);
			return true;
		}
	}

	internal class DungeonContent(DungeonDescription Dungeon) : IContentPiece
	{
		public string Id => "Dungeon_" + Dungeon.Id;
		public string Name => "Dungeon: " + Dungeon.Name;
		public bool CanActivate() => !DungeonList.GetAllDungeons().Any(d => d.Id == Dungeon.Id);
		public bool Activate()
		{
			if (!CanActivate())
				return false;
			DungeonList.AddDungeon(Dungeon);
			return true;
		}
		public bool Deactivate()
		{
			DungeonList.GetAllDungeons().RemoveAll(d => d.Id == Dungeon.Id);
			return true;
		}
	}

	internal class ItemFamilyContent(ItemFamilyDescription Items) : IContentPiece
	{
		public string Id => "Items_" + Items.Id;
		public string Name => "Item Family: " + Items.Name;
		public bool CanActivate() => !ItemList.ItemFamilies.Any(f => f.Id == Items.Id); 
		public bool Activate()
		{
			if (!CanActivate())
				return false;
			ItemList.ItemFamilies.Add(Items);
			return true;
		}
		public bool Deactivate()
		{
			ItemList.ItemFamilies.RemoveAll(i => i.Id == Items.Id);
			return true;
		}
	}

	internal class QuestContent(AbstractQuest Quest) : IContentPiece
	{
		public string Id => "Quest_" + Quest.Id;
		public string Name => "Quest: " + Quest.Id;
		public bool CanActivate() => !QuestList.GetAllQuests().Any(q => q.Id == Quest.Id);
		public bool Activate()
		{
			if (!CanActivate())
				return false;
			QuestList.GetAllQuests().Add(Quest);
			return true;
		}
		public bool Deactivate()
		{
			QuestList.GetAllQuests().RemoveAll(q => q.Id == Quest.Id);
			return true;
		}
	}

	internal class AbilityContent(AbilityDefinition Ability) : IContentPiece
	{
		public string Id => "Ability_" + Ability.Key;
		public string Name => "Ability: " + Ability.Name;
		public bool CanActivate() => AbilityList.GetAbility(Ability.Key) is null;
		public bool Activate()
		{
			if (!CanActivate())
				return false;
			return AbilityList.Add(Ability);
		}
		public bool Deactivate()
		{
			AbilityList.Remove(Ability.Key);
			return true;
		}
	}
}
