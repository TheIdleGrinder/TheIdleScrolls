using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Resources
{
    public class AchievementList
    {
        public static Achievement? GetAchievement(string id)
        {
            return s_achievements.Where(a => a.Id == id).FirstOrDefault();
        }

        static readonly List<Achievement> s_achievements = GenerateAchievements();

        public static List<Achievement> GetAllAchievements() => s_achievements;

        static List<Achievement> GenerateAchievements()
        {
            var achievements = new List<Achievement>();

            // Level achievements
            int[] levels = { 10, 20, 30, 40, 50, 75, 100 };
            for (int i = 0; i < levels.Length; i++)
            {
                int level = levels[i];
                var achievement = new Achievement()
                {
                    Id = $"LVL{level}",
                    Title = $"Level {level}",
                    Description = $"Reach level {level}",
                    Condition = ExpressionParser.Parse($"Level >= {level}"),
                    Hidden = false
                };
                if (i > 0)
                {
                    achievement.Prerequisite = ExpressionParser.Parse($"LVL{levels[i - 1]}");
                }
                achievements.Add(achievement);
            }

            // Wilderness progress achievements
            (int Level, string Name)[] wildernessLevels = new (int, string)[]
            {
                ( 15, "Stroll Taker"),
                ( 25, "To the Lighthouse"),
                ( 50, "Wilderness Traveller"),
                ( 75, "Wilderness Explorer"),
                (100, "Wilderness Conqueror"),
                (150, "Wilderness Wanderer")
            };
            for (int i = 0; i < wildernessLevels.Length; i++)
            {
                int level = wildernessLevels[i].Level;
                var achievement = new Achievement()
                {
                    Id = $"WILD{level}",
                    Title = wildernessLevels[i].Name,
                    Description = $"Reach wilderness level {level}",
                    Condition = ExpressionParser.Parse($"WildernessLevel >= {level}"),
                    Hidden = false
                };
                if (i > 0)
                {
                    achievement.Prerequisite = ExpressionParser.Parse($"WILD{wildernessLevels[i - 1].Level}");
                }
                achievements.Add(achievement);
            }

            // Kill achievements
            (int Count, string Name)[] killCounts = new (int, string)[]
            {
                (  50, "Brawler"),
                ( 200, "Warrior"),
                ( 500, "Slayer"),
                (1000, "Exterminator"),
                (4800, "Legion Slayer")
            };
            for (int i = 0; i < killCounts.Length; i++)
            {
                int count = killCounts[i].Count;
                var achievement = new Achievement()
                {
                    Id = $"KILL{count}",
                    Title = killCounts[i].Name,
                    Description = $"Defeat {count} enemies with a single character",
                    Condition = ExpressionParser.Parse($"Kills >= {count}"),
                    Hidden = false
                };
                if (i > 0)
                {
                    achievement.Prerequisite = ExpressionParser.Parse($"KILL{killCounts[i - 1].Count}");
                }
                achievements.Add(achievement);
            }

            // Dungeon achievements
            (string Id, string Name, string Title)[] dungeons = new (string, string, string)[]
            {
                (Definitions.DungeonIds.Crypt, Properties.Places.Dungeon_Crypt, "Cryptkeeper"),
                (Definitions.DungeonIds.MercenaryCamp, Properties.Places.Dungeon_MercenaryCamp, "Sellsword Slayer"),
                (Definitions.DungeonIds.Lighthouse, Properties.Places.Dungeon_Lighthouse, "I Shall Be Light"),
                (Definitions.DungeonIds.Temple, Properties.Places.Dungeon_Temple, "I Shall Keep Faith"),
                (Definitions.DungeonIds.CultistCastle, Properties.Places.Dungeon_CultistCastle, "I Shall Hone My Craft"),
                (Definitions.DungeonIds.Labyrinth, Properties.Places.Dungeon_Labyrinth, "I Shall Have No Mercy"),
                (Definitions.DungeonIds.ReturnToLighthouse, Properties.Places.Dungeon_ReturnToLighthouse, "I Shall Have No Fear"),
                (Definitions.DungeonIds.Threshold, Properties.Places.Dungeon_Threshold, "I Shall Have No Remorse"),
            };
            foreach (var (id, name, title) in dungeons)
            {
                var achievement = new Achievement()
                {
                    Id = $"DNG:{id}",
                    Title = title,
                    Description = $"Complete the {name}",
                    Prerequisite = ExpressionParser.Parse($"dng_open:{id}"),
                    Condition = ExpressionParser.Parse($"dng:{id} > 0"),
                    Hidden = false
                };
                if (id == Definitions.DungeonIds.Threshold)
                {
                    achievement.Description = $"Hold off the demonic invasion at {name}";
                }
                achievements.Add(achievement);
            }

            // Hardcore achievements
            int hcWildLevel = 75;
            achievements.Add(new("HC:WILDERNESS",
                "Attentive Grinder",
                $"Defeat a level {hcWildLevel} enemy in the wilderness without ever losing a fight",
                ExpressionParser.Parse("WILD50"),
                ExpressionParser.Parse($"Wilderness >= {hcWildLevel} && Losses == 0")));

            achievements.Add(new($"HC:{Definitions.DungeonIds.Crypt}",
                "Attentive Dungeoneer",
                $"Complete the Crypt without ever losing a fight",
                ExpressionParser.Parse($"DNG:{Definitions.DungeonIds.Crypt}"),
                ExpressionParser.Parse($"dng:{Definitions.DungeonIds.Crypt} > 0 && Losses == 0")));
            achievements.Add(new($"HC:{Definitions.DungeonIds.Lighthouse}",
                "So that All May Find You",
                "Complete the Beacon without ever losing a fight",
                ExpressionParser.Parse($"DNG:{Definitions.DungeonIds.Lighthouse}"),
                ExpressionParser.Parse($"dng:{Definitions.DungeonIds.Lighthouse} > 0 && Losses == 0")));
            achievements.Add(new($"HC:{Definitions.DungeonIds.ReturnToLighthouse}",
                "Hardcore",
                "Complete the Lighthouse without ever losing a fight",
                ExpressionParser.Parse($"DNG:{Definitions.DungeonIds.ReturnToLighthouse}"),
                ExpressionParser.Parse($"dng:{Definitions.DungeonIds.ReturnToLighthouse} > 0 && Losses == 0")));

            // Ability achievements
            (int Level, string Rank)[] ranks = new (int, string)[]
            {
                ( 25, "Apprentice"),
                ( 50, "Adept"),
                ( 75, "Expert"),
                (100, "Master")
            };
            for (int i = 0; i < ranks.Length; i++)
            {
                int level = ranks[i].Level;
                achievements.Add(new(
                    $"WEAP{level}",
                    $"Weapon {ranks[i].Rank}",
                    $"Reach ability level {level} for any weapon type",
                    (i > 0) ? ExpressionParser.Parse($"WEAP{ranks[i - 1].Level}") : new NumericNode(1.0),
                    ExpressionParser.Parse($"abl:AXE >= {level} || abl:BLN >= {level} || abl:LBL >= {level} || abl:POL >= {level} || abl:SBL >= {level}")));
            }
            for (int i = 0; i < ranks.Length; i++)
            {
                int level = ranks[i].Level;
                achievements.Add(new(
                $"ARMOR{level}",
                $"Armor {ranks[i].Rank}",
                $"Reach ability level {level} for any armor type",
                (i > 0) ? ExpressionParser.Parse($"ARMOR{ranks[i - 1].Level}") : new NumericNode(1.0),
                ExpressionParser.Parse($"abl:LAR >= {level} || abl:HAR >= {level}")));
            }

            // 'of all trades' achievements
            (int Level, string Rank)[] oAllRanks = new (int, string)[]
            {
                ( 25, "Jack"),
                ( 50, "Queen"),
                ( 75, "King"),
                (100, "Ace")
            };
            for (int i = 0; i < oAllRanks.Length; i++)
            {
                int level = oAllRanks[i].Level;
                achievements.Add(new(
                $"{oAllRanks[i].Rank[..1]}oALL",
                $"{oAllRanks[i].Rank} of All Trades",
                $"Reach ability level {level} for all weapon and armor types with a single character",
                (i > 0) ? ExpressionParser.Parse($"{oAllRanks[i - 1].Rank[..1]}oALL") : new NumericNode(1.0),
                ExpressionParser.Parse($"abl:AXE >= {level} && abl:BLN >= {level} && abl:LBL >= {level} && abl:POL >= {level} && abl:SBL >= {level} && abl:LAR >= {level} && abl:HAR >= {level}")));
            }

            // Crafting achievements
            for (int i = 0; i < ranks.Length; i++)
            {
                int level = ranks[i].Level;
                achievements.Add(new(
                    $"CRAFTING{level}",
                    $"{ranks[i].Rank} Blacksmith",
                    $"Train Crafting ability to level {level}",
                    (i > 0) ? ExpressionParser.Parse($"CRAFTING{ranks[i - 1].Level}") : new NumericNode(1.0),
                    ExpressionParser.Parse($"abl:ABL_CRAFT >= {level}")));
            }
            string[] craftNames = { "Transmuted", "Augmented", "Regal", "Exalted", "Divine" };
            for (int i = 0; i < craftNames.Length; i++)
            {
                achievements.Add(new(
                    $"BestReforge+{i + 1}",
                    $"{craftNames[i]} Craft",
                    $"Reforge an item to +{i + 1} rarity",
                    (i > 0) ? ExpressionParser.Parse($"BestReforge+{i}") : new NumericNode(1.0),
                    ExpressionParser.Parse($"BestReforge >= {i + 1}")));
            }
            Achievement tier0Reforge = new(
                "G0Reforge+3",
                "Still not Viable",
                "Reforge a training item to +3 rarity or higher",
                ExpressionParser.Parse("BestReforge+3"),
                ExpressionParser.Parse("BestG0Craft >= 3")
                )
            {
                Hidden = true
            };
            achievements.Add(tier0Reforge);

            // Coins achievements
            ranks = new (int, string)[]
            {
                (  1000, "Scavenger"),
                (  5000, "Fence"),
                ( 25000, "Merchant"),
                (100000, "Wholesaler"),
                (500000, "Patritian"),
            };
            for (int i = 0; i < ranks.Length; i++)
            {
                int coins = ranks[i].Level;
                achievements.Add(new(
                    $"TotalCoins{i}",
                    $"{ranks[i].Rank}",
                    $"Collect a total of {coins} coins with a single character",
                    (i > 0) ? ExpressionParser.Parse($"TotalCoins{i - 1}") : new NumericNode(1.0),
                    ExpressionParser.Parse($"TotalCoins >= {coins}")));
            }
            ranks = new (int, string)[]
            {
                (  1000, "Pocket Change"),
                (  5000, "Nest Egg"),
                ( 25000, "Money Bags"),
                (100000, "Money Vault")
            };
            for (int i = 0; i < ranks.Length; i++)
            {
                int coins = ranks[i].Level;
                achievements.Add(new(
                    $"MaxCoins{i}",
                    $"{ranks[i].Rank}",
                    $"Stockpile {coins} coins",
                    (i > 0) ? ExpressionParser.Parse($"MaxCoins{i - 1}") : new NumericNode(1.0),
                    ExpressionParser.Parse($"MaxCoins >= {coins}")));
            }

            // Miscellaneous achievements
            achievements.Add(new(
                "Instructions Unclear",
                "Instructions Unclear",
                "Lose a fight before reaching level 12",
                new NumericNode(1.0),
                ExpressionParser.Parse("Losses > 0 && Level < 12")));
            achievements.Add(new(
                "NOCRYPT",
                "Untainted",
                "Complete the Beacon before the Crypt",
                new NumericNode(1.0),
                ExpressionParser.Parse("dng:CRYPT <= 0 && dng:LIGHTHOUSE > 0")));
            achievements.Add(new(
                "EndgameBeforeStory",
                "Did I forget something?",
                "Complete the endgame dungeons before finishing the story",
                ExpressionParser.Parse("dng:SUNKENCITY > 0 || dng:ACADEMY > 0 || dng:PAGODA > 0"),
                ExpressionParser.Parse("dng:SUNKENCITY > 0 && dng:ACADEMY > 0 && dng:PAGODA > 0 && dng:THRESHOLD <= 0")));


            // Unarmored/Unarmed achievements
            achievements.Add(new(
                "NOARMOR",
                "Wollt Ihr Ewig Leben?!",
                $"Complete the {Properties.Places.Dungeon_Lighthouse} without ever raising an armor ability",
                new NumericNode(1.0),
                ExpressionParser.Parse($"dng:{Definitions.DungeonIds.Lighthouse} > 0 && abl:LAR <= 10 && abl:HAR <= 10"))
                {
                    Perk = PerkFactory.MakeCharacterLevelBasedPerk("NOARMOR",
                        "Unarmored I",
                        "Gain evasion rating for each level",
                        ModifierType.AddBase,
                        0.5,
                        new List<string>() { Definitions.Tags.EvasionRating, Definitions.Tags.Unarmored })
                });
            achievements.Add(new(
                "HC:NOARMOR",
                "Not Today",
                "Complete the Beacon without ever raising an armor ability or losing a fight",
                ExpressionParser.Parse("NOARMOR"),
                ExpressionParser.Parse($"dng:{Definitions.DungeonIds.Lighthouse} > 0 && abl:LAR <= 10 && abl:HAR <= 10 && Losses == 0"))
                {
                    Perk = PerkFactory.MakeCharacterLevelBasedPerk("HC:NOARMOR",
                        "Unarmored II",
                        "Gain evasion rating for each level",
                        ModifierType.AddBase,
                        0.5,
                        new List<string>() { Definitions.Tags.EvasionRating, Definitions.Tags.Unarmored })
                });
            achievements.Add(new(
                "HC:NOARMOR_50",
                "Kensai",
                "Reach level 50 without ever raising an armor ability or losing a fight",
                ExpressionParser.Parse("HC:NOARMOR"),
                ExpressionParser.Parse("Level >= 50 && abl:LAR <= 10 && abl:HAR <= 10 && Losses == 0"))
                {
                    Perk = PerkFactory.MakeCharacterLevelBasedPerk("HC:NOARMOR_50",
                        "Unarmored III",
                        "Gain evasion rating for each level",
                        ModifierType.AddBase,
                        0.5,
                        new List<string>() { Definitions.Tags.EvasionRating, Definitions.Tags.Unarmored })
                });

            achievements.Add(new(
                "NOWEAPON",
                "Boxer",
                "Reach level 15 without ever raising a weapon ability",
                new NumericNode(1.0),
                ExpressionParser.Parse("Level >= 15 && abl:AXE <= 10 && abl:BLN <= 10 && abl:LBL <= 10 && abl:POL <= 10 && abl:SBL <= 10"))
                {
                    Perk = PerkFactory.MakeCharacterLevelBasedPerk("NOWEAPON",
                        "Unarmed I",
                        "Gain unarmed damage for each level",
                        ModifierType.AddBase,
                        0.05,
                        new List<string>() { Definitions.Tags.Damage, Definitions.Tags.Unarmed })
                });
            achievements.Add(new(
                "HC:NOWEAPON",
                "I Am The Greatest!",
                $"Complete the {Properties.Places.Dungeon_Lighthouse} without ever raising a weapon ability or losing a fight",
                ExpressionParser.Parse("NOWEAPON"),
                ExpressionParser.Parse($"dng:{Definitions.DungeonIds.Lighthouse} > 0 && abl:AXE <= 10 " +
                    $"&& abl:BLN <= 10 && abl:LBL <= 10 && abl:POL <= 10 && abl:SBL <= 10 && Losses == 0"))
                {
                    Perk = PerkFactory.MakeCharacterLevelBasedPerk("HC:NOWEAPON",
                        "Unarmed II",
                        "Gain unarmed damage for each level",
                        ModifierType.AddBase,
                        0.05,
                        new List<string>() { Definitions.Tags.Damage, Definitions.Tags.Unarmed })
                });
            achievements.Add(new(
                "HC:NOWEAPON_50",
                "Path of the Monk",
                "Reach level 50 without ever raising a weapon ability or losing a fight",
                ExpressionParser.Parse("HC:NOWEAPON"),
                ExpressionParser.Parse("Level >= 50 && abl:AXE <= 10 && abl:BLN <= 10 && abl:LBL <= 10 " +
                    "&& abl:POL <= 10 && abl:SBL <= 10 && Losses == 0"))
                {
                    Perk = PerkFactory.MakeCharacterLevelBasedPerk("HC:NOWEAPON_50",
                        "Unarmed III",
                        "Gain unarmed damage for each level",
                        ModifierType.AddBase,
                        0.05,
                        new List<string>() { Definitions.Tags.Damage, Definitions.Tags.Unarmed })
                });

            achievements.Add(new(
                "HC:NOARMOR+NOWEAPON",
                "The Paths Converge",
                "Defeat a level 75 enemy in the wilderness without raising any ability or losing a fight",
                ExpressionParser.Parse("HC:NOWEAPON_50"),
                ExpressionParser.Parse("Wilderness >= 75 && abl:AXE <= 10 && abl:BLN <= 10 && abl:LBL <= 10 && abl:POL <= 10 " +
                    "&& abl:SBL <= 10 && abl:LAR <= 10 && abl:HAR <= 10 && Losses == 0")));


            return achievements;
        }
    }
}
