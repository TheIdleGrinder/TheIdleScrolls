using MiniECS;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
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
            static bool tautology(Entity e, World w) => true;
            var achievements = new List<Achievement>();

            // Level achievements
            int[] levels = [ 10, 20, 30, 40, 50, 75, 100, 150 ];
            for (int i = 0; i < levels.Length; i++)
            {
                int level = levels[i];
                var achievement = new Achievement
                {
                    Id = $"LVL{level}",
                    Title = $"Level {level}",
                    Description = $"Reach level {level}",
                    Condition = ExpressionParser.ParseToFunction($"Level >= {level}"),
                    Hidden = false,
                    Prerequisite = (i > 0) ? ExpressionParser.ParseToFunction($"LVL{levels[i - 1]}") : tautology,
                    Reward = GetPerkForLeveledAchievement("LVL", level)
                };
                achievements.Add(achievement);
            }

            // Wilderness progress achievements
            (int Level, string Name)[] wildernessLevels =
            [
                ( 15, "Stroll Taker"),
                ( 25, "To the Lighthouse"),
                ( 50, "Wilderness Traveller"),
                ( 75, "Wilderness Explorer"),
                (100, "Wilderness Conqueror"),
                (150, "Wilderness Wanderer")
            ];
            for (int i = 0; i < wildernessLevels.Length; i++)
            {
                int level = wildernessLevels[i].Level;
                var achievement = new Achievement
                {
                    Id = $"WILD{level}",
                    Title = wildernessLevels[i].Name,
                    Description = $"Reach wilderness level {level}",
                    Condition = ExpressionParser.ParseToFunction($"Wilderness >= {level}"),
                    Hidden = false,
                    Prerequisite = (i > 0) ? ExpressionParser.ParseToFunction($"WILD{wildernessLevels[i - 1].Level}") : tautology,
                    Reward = GetPerkForLeveledAchievement("WILD", level)
                };
                achievements.Add(achievement);
            }

            // Kill achievements
            (int Count, string Name)[] killCounts =
            [
                (  50, "Brawler"),
                ( 200, "Warrior"),
                ( 500, "Slayer"),
                (1000, "Exterminator"),
                (4800, "Legion Slayer")
            ];
            for (int i = 0; i < killCounts.Length; i++)
            {
                int count = killCounts[i].Count;
                var achievement = new Achievement
                {
                    Id = $"KILL{count}",
                    Title = killCounts[i].Name,
                    Description = $"Defeat {count} enemies with a single character",
                    Condition = ExpressionParser.ParseToFunction($"Kills >= {count}"),
                    Hidden = false,
                    Prerequisite = (i > 0) ? ExpressionParser.ParseToFunction($"KILL{killCounts[i - 1].Count}") : tautology,
                    Reward = GetPerkForLeveledAchievement("KILL", count)
                };
                achievements.Add(achievement);
            }

            // Dungeon achievements
            (string Id, string Name, string Title, int PerkPoints)[] dungeons =
            [
                (DungeonIds.Crypt, Properties.Places.Dungeon_Crypt, "Cryptkeeper", 0),
                (DungeonIds.MercenaryCamp, Properties.Places.Dungeon_MercenaryCamp, "Sellsword Slayer", 0),
                (DungeonIds.Lighthouse, Properties.Places.Dungeon_Lighthouse, "I Shall Be Light", 1),
                (DungeonIds.Temple, Properties.Places.Dungeon_Temple, "I Shall Keep Faith", 1),
                (DungeonIds.CultistCastle, Properties.Places.Dungeon_CultistCastle, "I Shall Hone My Craft", 1),
                (DungeonIds.Labyrinth, Properties.Places.Dungeon_Labyrinth, "I Shall Have No Mercy", 1),
                (DungeonIds.ReturnToLighthouse, Properties.Places.Dungeon_ReturnToLighthouse, "I Shall Have No Fear", 1),
                (DungeonIds.Threshold, Properties.Places.Dungeon_Threshold, "I Shall Have No Remorse", 2),
            ];
            foreach (var (id, name, title, points) in dungeons)
            {
                var achievement = new Achievement()
                {
                    Id = $"DNG:{id}",
                    Title = title,
                    Description = $"Complete the {name}",
                    Prerequisite = ExpressionParser.ParseToFunction($"dng_open:{id}"),
                    Condition = ExpressionParser.ParseToFunction($"dng:{id} > 0"),
                    Hidden = false
                };
                if (id == DungeonIds.Threshold)
                {
                    achievement.Description = $"Hold off the demonic invasion at {name}";
                }
                if (points > 0)
                {
                    HashSet<string> pointIds = [];
                    for (int i = 0; i < points; i++)
                    {
                        pointIds.Add($"{achievement.Id}.{i}");
                    }
                    achievement.Reward = new PerkPointReward(pointIds);
                }
                achievements.Add(achievement);
            }
            // Void dungeon achievements
            achievements.Add(new("DNG:VOID",
                "Void Scout",
                $"Complete your first excursion to {Properties.Places.Dungeon_Void}",
                Conditions.DungeonAvailableCondition(DungeonIds.Void),
                Conditions.DungeonCompletedCondition(DungeonIds.Void)));
            achievements.Add(new("DNG:VOID@100",
                "Void Explorer",
                $"Complete {Properties.Places.Dungeon_Void} at area level 100",
                Conditions.AchievementUnlockedCondition("DNG:VOID"),
                Conditions.DungeonLevelCompletedCondition(DungeonIds.Void, 100)));
            achievements.Add(new("DNG:VOID@125",
                "Void Conqueror",
                $"Complete {Properties.Places.Dungeon_Void} at area level 125",
                Conditions.AchievementUnlockedCondition("DNG:VOID@100"),
                Conditions.DungeonLevelCompletedCondition(DungeonIds.Void, 125)));
            achievements.Add(new("VOIDBOSSES",
                "Void Duelist",
                $"Defeat all different bosses in {Properties.Places.Dungeon_Void}",
                Conditions.AchievementUnlockedCondition("DNG:VOID"),
                Conditions.MobsDefeatedCondition(DungeonList.VoidBosses)));

            // Hardcore achievements
            int hcWildLevel = 75;
            achievements.Add(new("HC:WILDERNESS",
                "Attentive Grinder",
                $"Defeat a level {hcWildLevel} enemy in the wilderness without ever losing a fight",
                ExpressionParser.ParseToFunction("WILD50"),
                ExpressionParser.ParseToFunction($"Wilderness >= {hcWildLevel} && Losses == 0")));

            achievements.Add(new($"HC:{DungeonIds.Crypt}",
                "Attentive Dungeoneer",
                $"Complete the Crypt without ever losing a fight",
                ExpressionParser.ParseToFunction($"DNG:{DungeonIds.Crypt}"),
                ExpressionParser.ParseToFunction($"dng:{DungeonIds.Crypt} > 0 && Losses == 0")));
            achievements.Add(new($"HC:{DungeonIds.Lighthouse}",
                "So that All May Find You",
                "Complete the Beacon without ever losing a fight",
                ExpressionParser.ParseToFunction($"DNG:{DungeonIds.Lighthouse}"),
                ExpressionParser.ParseToFunction($"dng:{DungeonIds.Lighthouse} > 0 && Losses == 0")));
            achievements.Add(new($"HC:{DungeonIds.ReturnToLighthouse}",
                "Hardcore",
                "Complete the Lighthouse without ever losing a fight",
                ExpressionParser.ParseToFunction($"DNG:{DungeonIds.ReturnToLighthouse}"),
                ExpressionParser.ParseToFunction($"dng:{DungeonIds.ReturnToLighthouse} > 0 && Losses == 0")));

            // Ability achievements
            (int Level, string Rank)[] ranks =
            [
                ( 25, "Apprentice"),
                ( 50, "Adept"),
                ( 75, "Expert"),
                (100, "Master"),
                (150, "Grandmaster")
            ];
            foreach (string weapFamily in Abilities.Weapons)
            {
                for (int i = 0; i < ranks.Length; i++)
                {
                    int level = ranks[i].Level;
                    Achievement achievement = new(
                        $"{weapFamily}{level}",
                        $"{weapFamily.Localize()} {ranks[i].Rank}",
                        $"Reach ability level {level} for {weapFamily.Localize()} weapons",
                        (i > 0) ? ExpressionParser.ParseToFunction($"{weapFamily}{ranks[i - 1].Level}") : tautology,
                        ExpressionParser.ParseToFunction($"abl:{weapFamily} >= {level}"))
                    {
                        Reward = GetPerkForLeveledAchievement(weapFamily, level)
                    };
                    achievements.Add(achievement);
                }
            }
            foreach (string armorFamily in Abilities.Armors)
            {
                for (int i = 0; i < ranks.Length; i++)
                {
                    int level = ranks[i].Level;
                    Achievement achievement = new(
                        $"{armorFamily}{level}",
                        $"{armorFamily.Localize()} {ranks[i].Rank}",
                        $"Reach ability level {level} for {armorFamily.Localize()}",
                        (i > 0) ? ExpressionParser.ParseToFunction($"{armorFamily}{ranks[i - 1].Level}") : tautology,
                        ExpressionParser.ParseToFunction($"abl:{armorFamily} >= {level}"))
                    {
                        Reward = GetPerkForLeveledAchievement(armorFamily, level)
                    };
                    achievements.Add(achievement);
                }
            }
            int mobsForStyles = 200;
            achievements.Add(new(Abilities.DualWield, "Dual Wielder", 
                $"Defeat {mobsForStyles} enemies while using two weapons",
                tautology,
                Conditions.MobsDefeatedConditionallyCondition(Tags.DualWield, mobsForStyles))
                { 
                    Reward = new AbilityReward(Abilities.DualWield)
                });
            achievements.Add(new(Abilities.Shielded, "Shieldbearer",
                $"Defeat {mobsForStyles} enemies while carrying a shield",
                tautology,
                Conditions.MobsDefeatedConditionallyCondition(Tags.Shielded, mobsForStyles))
            {
                Reward = new AbilityReward(Abilities.Shielded)
            });
            achievements.Add(new(Abilities.SingleHanded, "Fencer",
                $"Defeat {mobsForStyles} enemies while wielding only one one-handed weapon",
                tautology,
                Conditions.MobsDefeatedConditionallyCondition(Tags.SingleHanded, mobsForStyles))
            {
                Reward = new AbilityReward(Abilities.SingleHanded)
            });
            achievements.Add(new(Abilities.TwoHanded, "Wide Swings",
                $"Defeat {mobsForStyles} enemies while wielding a two-handed weapon",
                tautology,
                Conditions.MobsDefeatedConditionallyCondition(Tags.TwoHanded, mobsForStyles))
            {
                Reward = new AbilityReward(Abilities.TwoHanded)
            });

            // Fighting style level achievements
            achievements.Add(new(Abilities.DualWield + "25", "Ambidextrous",
                $"Reach ability level {25} for the {AbilityList.GetAbility(Abilities.DualWield)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.DualWield),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.DualWield], 25))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.DualWield, 25)
            });
            achievements.Add(new(Abilities.DualWield + "50", $"Dual Weapon Adept",
                $"Reach ability level {50} for the {AbilityList.GetAbility(Abilities.DualWield)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.DualWield + "25"),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.DualWield], 50))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.DualWield, 50)
            });
            achievements.Add(new(Abilities.DualWield + "75", "Dual Weapon Expert",
                $"Reach ability level {75} for the {AbilityList.GetAbility(Abilities.DualWield)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.DualWield + "50"),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.DualWield], 75))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.DualWield, 75)
            });
            achievements.Add(new(Abilities.DualWield + "100", "Dual Weapon Master",
                $"Reach ability level {100} for the {AbilityList.GetAbility(Abilities.DualWield)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.DualWield + "75"),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.DualWield], 100))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.DualWield, 100)
            });
            achievements.Add(new(Abilities.Shielded + "50", "Shield Adept",
                $"Reach ability level {50} for the {AbilityList.GetAbility(Abilities.Shielded)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.Shielded),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.Shielded], 50))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.Shielded, 50)
            });
            achievements.Add(new(Abilities.Shielded + "75", "Shield Expert",
                $"Reach ability level {75} for the {AbilityList.GetAbility(Abilities.Shielded)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.Shielded + "50"),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.Shielded], 75))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.Shielded, 75)
            });
            achievements.Add(new(Abilities.Shielded + "100", "Shield Master",
                $"Reach ability level {100} for the {AbilityList.GetAbility(Abilities.Shielded)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.Shielded + "75"),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.Shielded], 100))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.Shielded, 100)
            });
            achievements.Add(new(Abilities.SingleHanded + "25", "Fire Dancer",
                $"Reach ability level {25} for the {AbilityList.GetAbility(Abilities.SingleHanded)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.SingleHanded),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.SingleHanded], 25))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.SingleHanded, 25)
            });
            achievements.Add(new(Abilities.SingleHanded + "50", "Single Weapon Adept",
                $"Reach ability level {50} for the {AbilityList.GetAbility(Abilities.SingleHanded)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.SingleHanded + "25"),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.SingleHanded], 50))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.SingleHanded, 50)
            });
            achievements.Add(new(Abilities.SingleHanded + "75", "Single Weapon Expert",
                $"Reach ability level {75} for the {AbilityList.GetAbility(Abilities.SingleHanded)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.SingleHanded + "50"),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.SingleHanded], 75))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.SingleHanded, 75)
            });
            achievements.Add(new(Abilities.SingleHanded + "100", "Single Weapon Master",
                $"Reach ability level {100} for the {AbilityList.GetAbility(Abilities.SingleHanded)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.SingleHanded + "75"),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.SingleHanded], 100))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.SingleHanded, 100)
            });
            achievements.Add(new(Abilities.TwoHanded + "50", "Two-Handed Adept",
                $"Reach ability level {50} for the {AbilityList.GetAbility(Abilities.TwoHanded)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.TwoHanded),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.TwoHanded], 50))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.TwoHanded, 50)
            });
            achievements.Add(new(Abilities.TwoHanded + "75", "Two-Handed Expert",
                $"Reach ability level {75} for the {AbilityList.GetAbility(Abilities.TwoHanded)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.TwoHanded + "50"),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.TwoHanded], 75))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.TwoHanded, 75)
            });
            achievements.Add(new(Abilities.TwoHanded + "100", "Two-Handed Master",
                $"Reach ability level {100} for the {AbilityList.GetAbility(Abilities.TwoHanded)!.Name} style",
                Conditions.AchievementUnlockedCondition(Abilities.TwoHanded + "75"),
                Conditions.HasLevelledAllAbilitiesCondition([Abilities.TwoHanded], 100))
            {
                Reward = GetPerkForLeveledAchievement(Abilities.TwoHanded, 100)
            });

            // 'of all trades' achievements
            (int Level, string Rank)[] oAllRanks =
            [
                ( 25, "Jack"),
                ( 50, "Queen"),
                ( 75, "King"),
                (100, "Ace")
            ];
            for (int i = 0; i < oAllRanks.Length; i++)
            {
                int level = oAllRanks[i].Level;
                Achievement achievement = new(
                $"{oAllRanks[i].Rank[..1]}oALL",
                $"{oAllRanks[i].Rank} of All Trades",
                $"Reach ability level {level} with all fighting styles",
                (i > 0) ? ExpressionParser.ParseToFunction($"{oAllRanks[i - 1].Rank[..1]}oALL") : tautology,
                Conditions.HasLevelledAllAbilitiesCondition(Abilities.Styles, level))
                {
                    Reward = GetPerkForLeveledAchievement("oALL", level)
                };
                achievements.Add(achievement);
            }

            // Crafting achievements
            for (int i = 0; i < ranks.Length; i++)
            {
                int level = ranks[i].Level;
                achievements.Add(new(
                    $"CRAFTING{level}",
                    $"{ranks[i].Rank} Blacksmith",
                    $"Train Crafting ability to level {level}",
                    (i > 0) ? ExpressionParser.ParseToFunction($"CRAFTING{ranks[i - 1].Level}") : tautology,
                    ExpressionParser.ParseToFunction($"abl:ABL_CRAFT >= {level}"))
                {
                    Reward = GetPerkForLeveledAchievement("ABL_CRAFT", level)
                });
            }
            string[] craftNames = [ "Transmuted", "Augmented", "Regal", "Exalted", "Divine", "Awakened", "Eternal" ];
            for (int i = 0; i < craftNames.Length; i++)
            {
                achievements.Add(new(
                    $"BestRefine+{i + 1}",
                    $"{craftNames[i]} Craft",
                    $"Refine an item to quality +{i + 1}",
                    (i > 0) ? ExpressionParser.ParseToFunction($"BestRefine+{i}") : tautology,
                    ExpressionParser.ParseToFunction($"BestRefine >= {i + 1}")));
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
            achievements.Add(tier0Refine);

            // Coins achievements
            ranks =
            [
                (  1000, "Scavenger"),
                (  5000, "Fence"),
                ( 25000, "Merchant"),
                (100000, "Wholesaler"),
                (500000, "Patritian"),
            ];
            for (int i = 0; i < ranks.Length; i++)
            {
                int coins = ranks[i].Level;
                Achievement achievement = new(
                    $"TotalCoins{i}",
                    $"{ranks[i].Rank}",
                    $"Collect a total of {coins} coins with a single character",
                    (i > 0) ? ExpressionParser.ParseToFunction($"TotalCoins{i - 1}") : tautology,
                    ExpressionParser.ParseToFunction($"TotalCoins >= {coins}"))
                {
                    Reward = GetPerkForLeveledAchievement("TotalCoins", coins)
                };
                achievements.Add(achievement);
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
                    (i > 0) ? ExpressionParser.ParseToFunction($"MaxCoins{i - 1}") : tautology,
                    ExpressionParser.ParseToFunction($"MaxCoins >= {coins}"))
                {
                    Reward = GetPerkForLeveledAchievement("MaxCoins", coins)
                };
                achievements.Add(achievement);
            }

            // Miscellaneous achievements
            achievements.Add(new(
                "Instructions Unclear",
                "Instructions Unclear",
                "Lose a fight before reaching level 12",
                tautology,
                ExpressionParser.ParseToFunction("Losses > 0 && Level < 12")));
            achievements.Add(new(
                "NOCRYPT",
                "Untainted",
                "Complete the Beacon before the Crypt",
                tautology,
                ExpressionParser.ParseToFunction("dng:CRYPT <= 0 && dng:LIGHTHOUSE > 0")));


            // Unarmored/Unarmed achievements
            double noArmorBaseEvasion = 10;
            double noArmorBaseEvasion1 = 0.5;
            double noArmorBaseEvasion2 = 1.0;
            achievements.Add(new(
                "NOARMOR1",
                "Plain Clothes",
                $"Complete the {Properties.Places.Dungeon_RatDen} without ever raising an armor ability",
                tautology,
                ExpressionParser.ParseToFunction($"dng:{DungeonIds.DenOfRats} > 0 && abl:LAR <= 1 && abl:HAR <= 1"))
                {
                    Reward = new PerkReward(PerkFactory.MakeStaticPerk("NOARMOR1",
                        "Agile",
                        $"Gain +{noArmorBaseEvasion:0.#} base evasion rating while unarmored",
                        ModifierType.AddBase,
                        noArmorBaseEvasion,
                        [Tags.EvasionRating],
                        [Tags.Unarmored]
                    ))
                }
            );
            achievements.Add(new(
                "NOARMOR2",
                "Wollt Ihr Ewig Leben?!",
                $"Complete the {Properties.Places.Dungeon_Lighthouse} without ever raising an armor ability",
                ExpressionParser.ParseToFunction("NOARMOR1"),
                ExpressionParser.ParseToFunction($"dng:{DungeonIds.Lighthouse} > 0 && abl:LAR <= 1 && abl:HAR <= 1"))
                {
                    Reward = new AbilityReward(Abilities.Unarmored)
                }
            );
            achievements.Add(new(
                "HC:NOARMOR",
                "Not Today",
                "Complete the Beacon without ever raising an armor ability or losing a fight",
                ExpressionParser.ParseToFunction("NOARMOR2"),
                ExpressionParser.ParseToFunction($"dng:{DungeonIds.Lighthouse} > 0 && abl:LAR <= 1 && abl:HAR <= 1 && Losses == 0"))
                {
                    Reward = new PerkReward(PerkFactory.MakeCharacterLevelBasedPerk("HC:NOARMOR",
                        "Elusive",
                        $"Gain +{noArmorBaseEvasion1:0.#} evasion rating for each level",
                        ModifierType.AddBase,
                        noArmorBaseEvasion1,
                        [Tags.EvasionRating],
                        [Tags.Unarmored]
                    ))
                }
            );
            achievements.Add(new(
                "HC:NOARMOR_50",
                "Armored in Faith",
                "Reach level 50 without ever raising an armor ability or losing a fight",
                ExpressionParser.ParseToFunction("HC:NOARMOR"),
                ExpressionParser.ParseToFunction("Level >= 50 && abl:LAR <= 1 && abl:HAR <= 1 && Losses == 0"))
                {
                    Reward = new PerkReward(PerkFactory.MakeCharacterLevelBasedPerk("HC:NOARMOR_50",
                        "Ethereal Form",
                        $"Gain +{noArmorBaseEvasion2:0.#} base evasion rating for each level",
                        ModifierType.AddBase,
                        noArmorBaseEvasion2,
                        [Tags.EvasionRating],
                        [Tags.Unarmored]
                    ))
                }
            );

            int noWeaponLevel = 5;
            double noWeaponBaseDamageBonus = 3.0;
            double noWeaponLevelDamageBonus = 0.1;
            double noWeaponMaxLevel = 50;
            achievements.Add(new(
                "NOWEAPON",
                "Boxer",
                $"Reach level {noWeaponLevel} without ever raising a weapon ability",
                tautology,
                ExpressionParser.ParseToFunction($"Level >= {noWeaponLevel} && abl:AXE <= 1 && abl:BLN <= 1 " +
                    $"&& abl:LBL <= 1 && abl:POL <= 1 && abl:SBL <= 1"))
                {
                    Reward = new PerkReward(PerkFactory.MakeStaticPerk("NOWEAPON",
                        "Boxer",
                        $"Increased base damage with unarmed attacks",
                        ModifierType.AddBase,
                        noWeaponBaseDamageBonus,
                        [Tags.Damage],
                        [Tags.Unarmed]
                    ))
                }
            );
            achievements.Add(new(
                "NOWEAPON_DMG",
                "Prize Fighter",
                $"Complete the {Properties.Places.Dungeon_RatDen} without ever raising a weapon ability",
                ExpressionParser.ParseToFunction("NOWEAPON"),
                ExpressionParser.ParseToFunction($"dng:{DungeonIds.DenOfRats} > 0 && abl:AXE <= 1 " +
                    $"&& abl:BLN <= 1 && abl:LBL <= 1 && abl:POL <= 1 && abl:SBL <= 1"))
                {
                    Reward = new AbilityReward(Abilities.Unarmed)
                }
            );
            achievements.Add(new(
                "NOWEAPON_DMG2",
                "I Am The Greatest!",
                $"Complete the {Properties.Places.Dungeon_Lighthouse} without ever raising a weapon ability",
                ExpressionParser.ParseToFunction("NOWEAPON_DMG"),
                ExpressionParser.ParseToFunction($"dng:{DungeonIds.Lighthouse} > 0 && abl:AXE <= 1 " +
                    $"&& abl:BLN <= 1 && abl:LBL <= 1 && abl:POL <= 1 && abl:SBL <= 1"))
                {
                    Reward = new PerkReward(PerkFactory.MakeCharacterLevelBasedPerk("NOWEAPON_DMG2",
                        "Iron Fists",
                        $"Gain +{noWeaponLevelDamageBonus:0.#} unarmed damage for each level",
                        ModifierType.AddBase,
                        noWeaponLevelDamageBonus,
                        [Tags.Damage],
                        [Tags.Unarmed]
                    ))
                }
            );
            achievements.Add(new(
                $"HC:NOWEAPON_{noWeaponMaxLevel}",
                "Path of the Monk",
                $"Reach level {noWeaponMaxLevel} without ever raising a weapon ability or losing a fight",
                ExpressionParser.ParseToFunction("NOWEAPON_DMG2"),
                ExpressionParser.ParseToFunction($"Level >= {noWeaponMaxLevel} && abl:AXE <= 1 && abl:BLN <= 1 && abl:LBL <= 1 " +
                    "&& abl:POL <= 1 && abl:SBL <= 1 && Losses == 0"))
                {
                    Reward = new PerkReward(PerkFactory.MakeCharacterLevelBasedPerk($"HC:NOWEAPON_{noWeaponMaxLevel}",
                        "Force of Spirit",
                        $"Gain {Stats.AttackBonusPerLevel:0.#%} increased unarmed damage per character level",
                        ModifierType.Increase,
                        Stats.AttackBonusPerLevel,
                        [Tags.Damage],
                        [Tags.Unarmed]
                    ))
                }
            );

            achievements.Add(new(
                "HC:NOARMOR+NOWEAPON",
                "One with Nothing",
                "Defeat a level 75 enemy in the wilderness without raising any ability or losing a fight",
                ExpressionParser.ParseToFunction($"HC:NOWEAPON_{noWeaponMaxLevel}"),
                ExpressionParser.ParseToFunction("Wilderness >= 75 && abl:AXE <= 10 && abl:BLN <= 10 && abl:LBL <= 10 && abl:POL <= 10 " +
                    "&& abl:SBL <= 10 && abl:LAR <= 10 && abl:HAR <= 10 && Losses == 0"))
                {
                    Reward = new PerkReward(PerkFactory.MakeStaticMultiModPerk("HC:NOARMOR+NOWEAPON",
                        "Luminous Being",
                        "Gain a significant bonus to damage and defense while unarmed and unarmored",
                        [ModifierType.More, ModifierType.More],
                        [0.5, 0.5],
                        [[Tags.Damage], [Tags.Defense]],
                        [[Tags.Unarmed, Tags.Unarmored], [Tags.Unarmed, Tags.Unarmored]]
                    ))
                }
            );


            return achievements;
        }

        public static IAchievementReward? GetPerkForLeveledAchievement(string id, int level)
        {
            var perk = (id, level) switch
            {
                ("LVL", 150) => PerkFactory.MakeStaticPerk($"{id}{level}", "Experienced",
                                $"Gain {Stats.SavantXpMultiplier:0.#%} increased experience",
                                ModifierType.Increase,
                                Stats.SavantXpMultiplier,
                                [Tags.CharacterXpGain],
                                [],
                                true),
                ("KILL", 1000) => PerkFactory.MakeStaticPerk($"{id}{level}", "Battle Tested",
                                $"",
                                ModifierType.Increase,
                                2 * Stats.BigPerkFactor * Stats.BasicDamageIncrease,
                                [Tags.Damage],
                                []),
                ("AXE", 25) => PerkFactory.MakeStaticPerk($"{id}{level}", "Furious Swings",
                                $"Gain {0.2:0.#} extra attacks per second with {id.Localize()}s",
                                ModifierType.AddFlat,
                                0.2,
                                [Tags.AttackSpeed, Abilities.Axe],
                                []),
                ("BLN", 25) => new($"{id}{level}", "Stunning Blow",
                                $"Gain {5} to base armor value for each defensive item while using {id.Localize()} after first strike",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (_, e, w, c) =>
                                {
                                    bool firstStrike = (e.GetComponent<BattlerComponent>()?.FirstStrike ?? false);
                                    bool usingBlunt = e.GetComponent<EquipmentComponent>()?.GetItems()
                                        ?.Any(i => i.GetComponent<ItemComponent>()!
                                        .Blueprint.GetRelatedAbilityId() == Abilities.Blunt) ?? false;
                                    return [ new($"{id}{level}", ModifierType.AddBase,
                                        (!firstStrike && usingBlunt) ? 5 : 0,
                                        [Tags.ArmorRating],
                                        []
                                        )
                                    ];
                                }),
                ("LBL", 25) => PerkFactory.MakeStaticPerk($"{id}{level}", "Quick Slash",
                                $"{1.0:0.#%} more attack speed during first attack with {id.Localize()}s",
                                ModifierType.More,
                                1.0,
                                [Tags.AttackSpeed, Abilities.LongBlade],
                                [Tags.FirstStrike]),
                ("POL", 25) => PerkFactory.MakeStaticPerk($"{id}{level}", "Range Advantage",
                                $"{1.0:0.#%} more defenses during first attack with {id.Localize()}s",
                                ModifierType.More,
                                1.0,
                                [Tags.Defense],
                                [Tags.FirstStrike, Abilities.Polearm]),
                ("SBL", 25) => PerkFactory.MakeStaticPerk($"{id}{level}", "Sneak Attack",
                                $"Deal double damage with short blades on your first attack every battle",
                                ModifierType.More,
                                1.0,
                                [Tags.Damage, Abilities.ShortBlade],
                                [Tags.FirstStrike]),
                ("AXE", 75) => new($"{id}{level}", "Frenzy",
                                $"Gain {0.01:0.#%}/{0.02:0.#%}/{0.03:0.#%} increased attack speed with {id.Localize()}s " +
                                    $"after every attack (up to {0.1:0.#%}/{0.2:0.#%}/{0.3:0.#%})",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (l, e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    return [ new($"{id}{level}", ModifierType.Increase, Math.Min(attacks * l * 0.01, 0.3),
                                        [ Tags.AttackSpeed, Abilities.Axe ],
                                        [])
                                    ];
                                })
                                { MaxLevel = 3 },
                ("BLN", 75) => new($"{id}{level}", "Armor Breaker",
                                $"Gain {0.02:0.#%}/{0.04:0.#%}/{0.06:0.#%} increased damage with {id.Localize()}s " +
                                    $"after every attack (up to {0.2:0.#%}/{0.4:0.#%}/{0.6:0.#%})",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (l, e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    return [ new($"{id}{level}", ModifierType.Increase, Math.Min(attacks * l * 0.02, 0.6),
                                        [ Tags.Damage, Abilities.Blunt ],
                                        [])
                                    ];
                                })
                                { MaxLevel = 3 },
                ("LBL", 75) => new($"{id}{level}", "Fluent Technique",
                                $"Gain {0.1:0.#%}/{0.2:0.#%}/{0.3:0.#%} increased damage or attack speed with {id.Localize()}s " +
                                    $"(changes after each attack)",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (l, e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    bool damage = (attacks % 2) == 0;
                                    return
                                    [
                                        new($"{id}{level}_dmg", ModifierType.Increase, damage ? 0.1 * l : 0.0,
                                            [ Tags.Damage, Abilities.LongBlade ], []
                                        ),
                                        new($"{id}{level}_spd", ModifierType.Increase, damage ? 0.0 : 0.1 * l,
                                            [ Tags.AttackSpeed, Abilities.LongBlade ], []
                                        )
                                    ];
                                })
                                { MaxLevel = 3},
                ("POL", 75) => new($"{id}{level}", "Skewer",
                                $"Gain {0.02:0.#%}/{0.04:0.#%}/{0.06:0.#%} increased damage with {id.Localize()}s " +
                                    $"after every attack (up to {0.2:0.#%}/{0.4:0.#%}/{0.6:0.#%})",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (l, e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    return [ new($"{id}{level}", 
                                             ModifierType.Increase, 
                                             Math.Min(attacks * 0.05, 0.5),
                                             [ Tags.Damage, Abilities.Polearm ],
                                             [])
                                    ];
                                })
                                { MaxLevel = 3 },
                ("SBL", 75) => new($"{id}{level}", "Critical Strikes",
                                $"Deal {1.5:0.#%} increased damage with {id.Localize()}s once every {5}/{4}/{3} attacks",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (l, e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    bool bonus = (attacks % (6 - l)) == (5 - l);
                                    return [ new($"{id}{level}", 
                                             ModifierType.Increase, 
                                             bonus ? 1.5 : 0.0,
                                            [ Tags.Damage, Abilities.ShortBlade ],
                                            [])
                                    ];
                                })
                                { MaxLevel = 3 },
                ("AXE" or "BLN" or "LBL" or "POL" or "SBL", 50)
                            => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Adept",
                                $"Gain {0.12:0.#%} increased damage with {id.Localize()}s",
                                ModifierType.Increase,
                                Stats.BigPerkFactor * Stats.BasicDamageIncrease,
                                [Tags.Damage, id],
                                [],
                                maxLevel: 3),
                ("AXE" or "BLN" or "LBL" or "POL" or "SBL", 100)
                            => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Master",
                                $"Gain a {Stats.MasterPerkMultiplier:0.#%} damage multiplier",
                                ModifierType.More,
                                Stats.MasterPerkMultiplier,
                                [Tags.Damage],
                                []),
                ("LAR", 50) => new($"{id}{level}", "Elegant Parry", $"Light shields also grant evasion rating equal to {.3:0.#%} of their armor",
                                [UpdateTrigger.EquipmentChanged],
                                (_, e, w, c) =>
                                {
                                    double shieldArmor = e.GetComponent<EquipmentComponent>()?.GetItems()
                                        ?.Where(i => i.IsShield())?.Sum(i => i.GetComponent<ArmorComponent>()?.Armor ?? 0.0) ?? 0.0;
                                    return [ new("ShieldEvasion", ModifierType.AddBase, 0.3 * shieldArmor,
                                        [ Tags.Shield,
                                          Tags.EvasionRating,
                                          Abilities.LightArmor ],
                                        []
                                        )
                                    ];
                                }),
                ("HAR", 50) => new($"{id}{level}", "Bulwark", $"{0.5:0.#%} multiplier to defenses from equipped heavy shield",
                                [UpdateTrigger.EquipmentChanged],
                                (_, e, w, c) =>
                                {
                                    double shieldArmor = e.GetComponent<EquipmentComponent>()?.GetItems()
                                        ?.Where(i => i.IsShield())?.Sum(i => i.GetComponent<ArmorComponent>()?.Armor ?? 0.0) ?? 0.0;
                                    return [ new("ShieldArmor", ModifierType.More, 0.5,
                                        [ Tags.Shield,
                                          Tags.Defense,
                                          Abilities.HeavyArmor ],
                                        []
                                        )
                                    ];
                                }),
                ("LAR" or "HAR", 25)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Apprentice",
                                    $"Gain increased defense with {id.Localize()}",
                                    ModifierType.Increase,
                                    Stats.BigPerkFactor * Stats.BasicDefenseIncrease,
                                    [Tags.Defense, id],
                                    [],
                                    maxLevel: 5),
                ("LAR" or "HAR", 75)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", $"Comfortable in {id.Localize()}",
                                    $"Gain increased attack speed while wearing {id.Localize()}",
                                    ModifierType.Increase,
                                    Stats.BigPerkFactor * Stats.BasicAttackSpeedIncrease,
                                    [Tags.AttackSpeed],
                                    [id]),
                ("LAR" or "HAR", 100)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Master",
                                    $"Gain a {Stats.MasterPerkMultiplier:0.#%} defense multiplier",
                                    ModifierType.More,
                                    Stats.MasterPerkMultiplier,
                                    [Tags.Defense],
                                    []),
                ("ABL_CRAFT", 25)
                                => new($"{id}{level}", "Crafting Apprentice",
                                    $"Gain an additional slot in the crafting queue plus another one for every 25 levels of the Crafting ability",
                                    [UpdateTrigger.AbilityIncreased],
                                    (_, e, w, c) =>
                                    {
                                        int craftLevel = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return [ new($"{id}{level}", ModifierType.AddFlat, (craftLevel / 25) + 1,
                                            [ Tags.CraftingSlots ],
                                            []
                                            )
                                        ];
                                    })
                                    {
                                        Permanent = true
                                    },
                ("ABL_CRAFT", 50)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", "Crafting Journeyman",
                                    $"Gain a discount for all crafts",
                                    ModifierType.Increase,
                                    -0.2,
                                    [Tags.CraftingCost],
                                    [], true),
                ("ABL_CRAFT", 75)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", "Crafting Expert",
                                    $"Gain increased crafting speed",
                                    ModifierType.Increase,
                                    0.2,
                                    [Tags.CraftingSpeed],
                                    [], true),
                ("ABL_CRAFT", 100)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", "Crafting Master",
                                    $"Gain one more active crafting slot",
                                    ModifierType.AddFlat,
                                    1.0,
                                    [Tags.ActiveCrafts],
                                    [], true),
                ("AXE" or "BLN" or "LBL" or "POL" or "SBL" or "LAR" or "HAR" or "ABL_CRAFT", 150)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Savant",
                                    $"{Stats.SavantXpMultiplier:0.#%} increased experience gain for {id.Localize()} ability",
                                    ModifierType.Increase,
                                    Stats.SavantXpMultiplier,
                                    [Tags.AbilityXpGain, id],
                                    [], true),
                (Abilities.DualWield, 25) => PerkFactory.MakeStaticPerk($"{id}{level}", $"Ambidextrous",
                                    $"{Stats.DualWieldAttackSpeedMulti:0.#%} more attack speed while dual wielding",
                                    ModifierType.More,
                                    Stats.DualWieldAttackSpeedMulti,
                                    [Tags.AttackSpeed],
                                    [Tags.DualWield]),
                (Abilities.DualWield, 50) => new($"{id}{level}", "Reckless Assault",
                                    $"Sacrifice some defense to gain an exponentially increasing multiplier to attack speed",
                                    [],
                                    (l, e, w, c) => 
                                    [
                                        new($"{id}{level}_as", ModifierType.More, Math.Pow(1.09, l) - 1.0, [ Tags.AttackSpeed ], []),
                                        new($"{id}{level}_def", ModifierType.More, Math.Pow(0.95, l) - 1.0, [ Tags.Defense ], [])
                                    ])
                                    {
                                        MaxLevel = 5
                                    },
                (Abilities.DualWield, 75) => new($"{id}{level}", "Assassin",
                                    $"Gain {0.1} base damage per level of the {Properties.LocalizedStrings.ABL_DualWield} ability",
                                    [UpdateTrigger.AbilityIncreased],
                                    (_, e, w, c) =>
                                    {
                                        int lvl = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_dmg", ModifierType.AddBase, 0.1 * lvl,
                                                [ Tags.Damage ],
                                                []
                                            )
                                        ];
                                    }),
                (Abilities.DualWield, 100) => PerkFactory.MakeStaticPerk($"{id}{level}", 
                                    $"{Properties.LocalizedStrings.ABL_DualWield} Master",
                                    $"Gain a {Stats.MasterPerkMultiplier:0.#%} attack speed multiplier",
                                    ModifierType.More,
                                    Stats.MasterPerkMultiplier,
                                    [Tags.AttackSpeed],
                                    []),
                (Abilities.Shielded, 50) => new($"{id}{level}", "Methodical",
                                    $"Sacrifice some damage to gain an exponentially increasing multiplier to defense",
                                    [],
                                    (l, e, w, c) =>
                                    [
                                        new($"{id}{level}_def", ModifierType.More, Math.Pow(1.09, l) - 1.0, [ Tags.Defense ], []),
                                        new($"{id}{level}_dmg", ModifierType.More, Math.Pow(0.95, l) - 1.0, [ Tags.Damage ], [])
                                    ])
                                    {
                                        MaxLevel = 5
                                    },
                (Abilities.Shielded, 75) => new($"{id}{level}", "Juggernaut",
                                    $"Gain {0.001:0.###%} increased damage per {1000} points of armor rating per level of " +
                                    $"the {Properties.LocalizedStrings.ABL_Shielded} ability",
                                    [UpdateTrigger.AbilityIncreased, UpdateTrigger.EquipmentChanged, 
                                        UpdateTrigger.BattleStarted, UpdateTrigger.AttackPerformed],
                                    (_, e, w, c) =>
                                    {
                                        double armor = e.GetComponent<DefenseComponent>()?.Armor ?? 0;
                                        int lvl = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_dmg", ModifierType.Increase, 0.001 * (lvl * armor / 1000.0),
                                                [ Tags.Damage ],
                                                []
                                            )
                                        ];
                                    }),
                (Abilities.Shielded, 100) => PerkFactory.MakeStaticPerk($"{id}{level}",
                                    $"{Properties.LocalizedStrings.ABL_Shielded} Master",
                                    $"Gain a {Stats.MasterPerkMultiplier:0.#%} defense multiplier",
                                    ModifierType.More,
                                    Stats.MasterPerkMultiplier,
                                    [Tags.Defense],
                                    []),
                (Abilities.SingleHanded, 25) => PerkFactory.MakeStaticPerk($"{id}{level}", $"Fire Dancing",
                                    $"{0.2:0.#%} more damage in single-handed style",
                                    ModifierType.More,
                                    0.2,
                                    [Tags.Damage],
                                    [Tags.SingleHanded]),
                (Abilities.SingleHanded, 50) => new($"{id}{level}", "Fleet-footed",
                                    $"Gain base evasion rating per level of the {Properties.LocalizedStrings.ABL_SingleHanded} " +
                                        $"ability while fighting single-handed",
                                    [UpdateTrigger.AbilityIncreased],
                                    (l, e, w, c) =>
                                    {
                                        int lvl = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_eva", ModifierType.AddBase, 0.5 * l * lvl,
                                                [Tags.EvasionRating, Tags.Global],
                                                [Tags.SingleHanded]
                                            )
                                        ];
                                    })
                                    { MaxLevel = 5 },
                (Abilities.SingleHanded, 75) => new($"{id}{level}", "Duelist",
                                    $"Gain {0.02:0.#%} increased damage per level of the " +
                                        $"{Properties.LocalizedStrings.ABL_SingleHanded} ability while evading",
                                    [UpdateTrigger.AbilityIncreased],
                                    (_, e, w, c) =>
                                    {
                                        int lvl = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_dmg", ModifierType.Increase, 0.02 * lvl,
                                                [Tags.Damage],
                                                [Tags.Evading]
                                            )
                                        ];
                                    }),
                (Abilities.SingleHanded, 100) => PerkFactory.MakeStaticPerk($"{id}{level}",
                                    $"{Properties.LocalizedStrings.ABL_SingleHanded} Master",
                                    $"Gain a {Stats.MasterPerkMultiplier:0.#%} time limit multiplier",
                                    ModifierType.More,
                                    Stats.MasterPerkMultiplier,
                                    [Tags.TimeShield],
                                    []),
                (Abilities.TwoHanded, 50) => new($"{id}{level}", "Precise Attacks",
                                    $"Sacrifice some attack speed to gain an exponentially increasing multiplier to damage",
                                    [],
                                    (l, e, w, c) =>
                                    [
                                        new($"{id}{level}_dmg", ModifierType.More, Math.Pow(1.09, l) - 1.0, [ Tags.Damage ], []),
                                        new($"{id}{level}_as", ModifierType.More, Math.Pow(0.95, l) - 1.0, [ Tags.AttackSpeed ], [])
                                    ])
                                    {
                                        MaxLevel = 5
                                    },
                (Abilities.TwoHanded, 75) => new($"{id}{level}", "Executioner",
                                    $"Gain {0.01:0.#%} increased damage per second of attack time per level of " +
                                    $"{Properties.LocalizedStrings.ABL_TwoHanded} ability",
                                    [UpdateTrigger.AbilityIncreased, UpdateTrigger.EquipmentChanged,
                                        UpdateTrigger.BattleStarted, UpdateTrigger.AttackPerformed],
                                    (_, e, w, c) =>
                                    {
                                        double cooldown = e.GetComponent<AttackComponent>()?.Cooldown.Duration ?? 0.0;
                                        int lvl = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_dmg", ModifierType.Increase, cooldown * lvl / 100,
                                                [ Tags.Damage ],
                                                []
                                            )
                                        ];
                                    }),
                (Abilities.TwoHanded, 100) => PerkFactory.MakeStaticPerk($"{id}{level}",
                                    $"{Properties.LocalizedStrings.ABL_TwoHanded} Master",
                                    $"Gain a {Stats.MasterPerkMultiplier:0.#%} damage multiplier",
                                    ModifierType.More,
                                    Stats.MasterPerkMultiplier,
                                    [Tags.Damage],
                                    []),
                _ => null
            };
            return perk != null ? new PerkReward(perk) as dynamic : null;
        }
    }
}
