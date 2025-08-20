using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Utility;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class CraftingContentPack : IContentPack
	{
		public string Id => "CP_Crafting";
		public string Name => "Crafting";
		public string Description => "This pack contains content that is related to crafting and refining items";
		public List<IContentPiece> ContentPieces
        {
            get
            {
                List<IContentPiece> contentPieces = [];
                
                (int Level, string Rank)[] ranks =
                [
                    ( 25, "Apprentice"),
                    ( 50, "Adept"),
                    ( 75, "Expert"),
                    (100, "Master"),
                    (150, "Grandmaster"),
                ];
                for (int i = 0; i < ranks.Length; i++)
                {
                    int level = ranks[i].Level;
                    contentPieces.Add(new AchievementContent(new(
                        $"{Abilities.Crafting}{level}",
                        $"{ranks[i].Rank} Blacksmith",
                        $"Train Crafting ability to level {level}",
                        (i > 0) ? ExpressionParser.ParseToFunction($"{Abilities.Crafting}{ranks[i - 1].Level}") : (e, w) => true,
                        ExpressionParser.ParseToFunction($"abl:ABL_CRAFT >= {level}"))
                        {
                            Reward = AchievementList.GetRewardForLeveledAchievement(Abilities.Crafting, level)
                        }
                    ));
                }
                string[] craftNames = [ "Transmuted", "Augmented", "Regal", "Exalted", "Divine", "Awakened", "Eternal" ];
                for (int i = 0; i < craftNames.Length; i++)
                {
                    contentPieces.Add(new AchievementContent(new(
                        $"BestRefine+{i + 1}",
                        $"{craftNames[i]} Craft",
                        $"Refine an item to quality +{i + 1}",
                        (i > 0) ? ExpressionParser.ParseToFunction($"BestRefine+{i}") : (e, w) => true,
                        ExpressionParser.ParseToFunction($"BestRefine >= {i + 1}")
                    )));
                }
                Achievement tier0Refine = new(
                    "G0Refine+3",
                    "Still not Viable",
                    "Refine a training item to quality +3 or better",
                    ExpressionParser.ParseToFunction("BestRefine+3"),
                    ExpressionParser.ParseToFunction("BestG0Craft >= 3")
                    )
                    {
                        Hidden = true
                    };
                    contentPieces.Add(new AchievementContent(tier0Refine));

                return contentPieces;
            }
        }
	}
}
