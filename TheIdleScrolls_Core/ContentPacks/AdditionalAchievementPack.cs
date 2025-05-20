using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Utility;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class AdditionalAchievementPack : IContentPack
	{
		public string Id => "CP_AdditionalAchievements";
		public string Name => "Miscellaneous";
		public string Description => "This pack contains miscellaneous achievements that don't fit into other categories";
		public List<IContentPiece> ContentPieces
        {
            get
            {
                List<IContentPiece> contentPieces = [];
                contentPieces.Add(new AchievementContent(new(
                    "Instructions Unclear",
                    "Instructions Unclear",
                    "Lose a fight before reaching level 12",
                    (e, w) => true,
                    ExpressionParser.ParseToFunction("Losses > 0 && Level < 12")
                )));

                if (DungeonList.GetDungeon(DungeonIds.Crypt) is not null)
                {
                    if (DungeonList.GetDungeon(DungeonIds.Lighthouse) is not null)
                    {
                        contentPieces.Add(new AchievementContent(new(
                            "NOCRYPT",
                            "Untainted",
                            "Complete the Beacon before the Crypt",
                            (e, w) => true,
                            ExpressionParser.ParseToFunction("dng:CRYPT <= 0 && dng:LIGHTHOUSE > 0")
                        )));
                    }
                    contentPieces.Add(new AchievementContent(new(
                        "FOUNDUBERCRYPT",
                        "Archaeologist",
                        $"Discover the {DungeonList.GetDungeon(DungeonIds.Crypt)!.Name}'s high level version",
                        Conditions.DungeonLevelAvailableCondition(DungeonIds.Crypt, DungeonList.LevelUberCrypt),
                        Conditions.DungeonLevelAvailableCondition(DungeonIds.Crypt, DungeonList.LevelUberCrypt)
                    )));
                }

                return contentPieces;
            }
        }
	}
}
