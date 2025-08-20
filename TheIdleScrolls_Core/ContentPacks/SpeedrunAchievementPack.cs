using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Quests;
using TheIdleScrolls_Core.Resources;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class SpeedrunAchievementPack : IContentPack
	{
		public string Id => "CP_SpeedrunAchievements";
		public string Name => "Speedruns";
		public string Description => "This pack contains achievements for completing several dungeons from the main story quickly";
		public List<IContentPiece> ContentPieces => [
			new AchievementContent(MakeSpeedrunAch(1, "Rat Racer", DungeonIds.DenOfRats, 4.0)),
			new AchievementContent(MakeSpeedrunAch(1, "Fast Castle", DungeonIds.CultistCastle, 15.0)),
			new AchievementContent(MakeSpeedrunAch(1, "Speedrun", DungeonIds.Threshold, 45.0)),
			new AchievementContent(MakeSpeedrunAch(2, "Speedier Run", DungeonIds.Threshold, 35.0)),
			new AchievementContent(MakeSpeedrunAch(3, "Gold Medal", DungeonIds.Threshold, 25.0)),
			new AchievementContent(MakeSpeedrunAch(4, "Author Medal", DungeonIds.Threshold, 20.0))
		];

		private Achievement MakeSpeedrunAch(int index, string name, string dungeonId, double target)
		{
			return new($"speed_{dungeonId}_{index}", name,
				$"Complete the {DungeonList.GetDungeon(dungeonId)?.Name ?? "???"} in under {target:0.#} minutes",
				index > 1
					? ExpressionParser.ParseToFunction($"speed_{dungeonId}_{index - 1}")
					: Conditions.DungeonAvailableCondition(dungeonId),
				ExpressionParser.ParseToFunction($"dng:{dungeonId} < {target * 60} && dng:{dungeonId} > 0"));
		}
	}
}
