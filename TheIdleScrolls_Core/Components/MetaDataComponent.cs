using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.Components
{
	public class MetaDataComponent : IComponent
	{
		public string Name { get; set; } = "??";
		public string NameWithSuffixTitle { get; set; } = "??";
		public string PrefixTitle { get; set; } = "";
		public int Level { get; set; } = 0;
		public string DisplayClass { get; set; } = "????";
		public string AdventureId { get; set; } = AdventureList.DefaultAdventureId;

		public void UpdateFromEntity(Entity entity)
		{
			var nameComponent = entity.GetComponent<NameComponent>();
			if (nameComponent is not null)
			{
				Name = nameComponent.Name;
			}
			var levelComponent = entity.GetComponent<LevelComponent>();
			if (levelComponent is not null)
			{
				Level = levelComponent.Level;
			}
			var titleComponent = entity.GetComponent<TitleBearerComponent>();
			if (titleComponent is not null)
			{
				NameWithSuffixTitle = entity.GetTitledName(false, true);
				PrefixTitle = titleComponent.GetPrefixTitle();
			}
			else
			{
				NameWithSuffixTitle = Name;
				PrefixTitle = "";
			}
			DisplayClass = PlayerFactory.GetCharacterClass(entity);
		}
	}
}
