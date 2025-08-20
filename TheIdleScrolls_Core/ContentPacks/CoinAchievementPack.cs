using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Utility;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class CoinAchievementPack : IContentPack
	{
		public string Id => "CP_CoinAchievements";
		public string Name => "Coins";
		public string Description => "This pack contains achievements for earning and hoarding coins";
		public List<IContentPiece> ContentPieces
        {
            get
            {
                List<IContentPiece> contentPieces = [];
                
                (int Level, string Rank)[] ranks =
                [
                    (  1000, "Scavenger"),
                    (  5000, "Fence"),
                    ( 25000, "Merchant"),
                    (100000, "Wholesaler"),
                    (250000, "Patrician"),
                ];
                for (int i = 0; i < ranks.Length; i++)
                {
                    int coins = ranks[i].Level;
                    Achievement achievement = new(
                        $"TotalCoins{i}",
                        $"{ranks[i].Rank}",
                        $"Collect a total of {coins} coins with a single character",
                        (i > 0) ? ExpressionParser.ParseToFunction($"TotalCoins{i - 1}") : (e, w) => true,
                        ExpressionParser.ParseToFunction($"TotalCoins >= {coins}"))
                    {
                        Reward = AchievementList.GetPerkForLeveledAchievement("TotalCoins", i + 1)
                    };
                    contentPieces.Add(new AchievementContent(achievement));
                }
                ranks =
                [
                    (  1000, "Pocket Change"),
                    (  5000, "Nest Egg"),
                    ( 25000, "Money Bags"),
                    (100000, "Money Vault")
                ];
                for (int i = 0; i < ranks.Length; i++)
                {
                    int coins = ranks[i].Level;
                    Achievement achievement = new(
                        $"MaxCoins{i}",
                        $"{ranks[i].Rank}",
                        $"Stockpile {coins} coins",
                        (i > 0) ? ExpressionParser.ParseToFunction($"MaxCoins{i - 1}") : (e, w) => true,
                        ExpressionParser.ParseToFunction($"MaxCoins >= {coins}"))
                    {
                        Reward = AchievementList.GetPerkForLeveledAchievement("MaxCoins", i + 1)
                    };
                    contentPieces.Add(new AchievementContent(achievement));
                }

                return contentPieces;
            }
        }
	}
}
