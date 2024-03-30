using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Crafting;

namespace TheIdleScrolls_Core.Components
{
	public class CraftingBenchComponent : IComponent
	{
		public int CraftingSlots { get; set; } = 1;
		public List<CraftingProcess> ActiveCrafts { get; } = new();

		public bool HasFreeSlot => ActiveCrafts.Count < CraftingSlots;

		public bool AddCraft(CraftingProcess craft)
		{
			if (HasFreeSlot)
			{
				ActiveCrafts.Add(craft);
				return true;
			}

			return false;
		}

		public CraftingProcess? RemoveCraft(uint id)
		{
			var craft = ActiveCrafts.FirstOrDefault(c => c.ID == id);
			if (craft != null)
			{
				ActiveCrafts.Remove(craft);
				return craft;
			}
			return null;
		}
	}
}
