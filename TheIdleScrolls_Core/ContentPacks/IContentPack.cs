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
		
		public List<IContentPiece> ContentPieces { get; }

		public static void Activate(IContentPack pack)
		{
			foreach (var content in pack.ContentPieces)
			{
				if (!content.CanActivate())
					throw new Exception($"Failed to activate content pack '{pack.Name}' (Can't activeate '{content.Id}')");
			}

			foreach (var content in pack.ContentPieces)
			{
				content.Activate();
			}
		}


	}
}
