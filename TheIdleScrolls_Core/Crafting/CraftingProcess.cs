﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Crafting
{
	public enum CraftingType
	{
		Craft,
		Refine
	}

	public class CraftingProcess
	{
		static uint NextID = 1;
		public uint ID { get; } = 0;
		public Entity TargetItem { get; }
		public Cooldown Duration { get; }
		public CraftingType Type { get; } = CraftingType.Craft;
		public double Roll { get; set; } = 0.5;
		public int CoinsPaid { get; set; } = 0;

		public bool HasFinished => Duration.HasFinished;

		public CraftingProcess(CraftingType type, Entity targetItem, double duration, double strength, int cost)
		{
			ID = NextID++;
			TargetItem = targetItem;
			Duration = new Cooldown(duration);
			Type = type;
			Roll = strength;
			CoinsPaid = cost;

			Duration.SingleShot = true;
		}

		public void Update(double deltaTime)
		{
			Duration.Update(deltaTime);
		}
	}
}
