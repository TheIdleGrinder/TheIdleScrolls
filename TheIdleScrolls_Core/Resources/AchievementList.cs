﻿using MiniECS;
using System.Reflection.Emit;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Systems;
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
                "Void Traveller",
                $"Complete {Properties.Places.Dungeon_Void} at area level 100",
                Conditions.AchievementUnlockedCondition("DNG:VOID"),
                Conditions.DungeonLevelCompletedCondition(DungeonIds.Void, 100)));
            achievements.Add(new("DNG:VOID@125",
                "Void Explorer",
                $"Complete {Properties.Places.Dungeon_Void} at area level 125",
                Conditions.AchievementUnlockedCondition("DNG:VOID@100"),
                Conditions.DungeonLevelCompletedCondition(DungeonIds.Void, 125)));
            achievements.Add(new("DNG:ENDGAME",
                "Void Conqueror",
                $"Complete an endgame dungeon",
                Conditions.AchievementUnlockedCondition("DNG:VOID@125"),
                (e, w) => Conditions.HasCompletedDungeon(e, DungeonIds.EndgameAges) 
                            || Conditions.HasCompletedDungeon(e, DungeonIds.EndgameMagic)
                            || Conditions.HasCompletedDungeon(e, DungeonIds.EndgamePyramid)));
            achievements.Add(new("DNG:UBERENDGAME",
                "Void Emperor",
                $"Complete an endgame dungeon at area level {DungeonLevels.LevelUberEndgame}",
                Conditions.DungeonLevelAvailableCondition(DungeonIds.EndgameAges, DungeonLevels.LevelUberEndgame),
                (e, w) => Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgameAges, DungeonLevels.LevelUberEndgame)
                            || Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgameMagic, DungeonLevels.LevelUberEndgame)
                            || Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgamePyramid, DungeonLevels.LevelUberEndgame)));

            achievements.Add(new("DUNGEONGRIND12",
                "Dungeon Delver's Dozen",
                $"Complete dungeons 12 times",
                Conditions.DungeonAvailableCondition(DungeonIds.DenOfRats),
                Conditions.DungeonClearCountCondition(12)
            )
            {
                Reward = new FeatureReward(GameFeature.DungeonGrinding, "Auto-grind dungeons")
            });
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
            achievements.Add(new($"HC:Endgame",
                "Exalted Conqueror",
                "Complete the endgame dungeons without ever losing a fight",
                Conditions.AchievementUnlockedCondition($"HC:{DungeonIds.ReturnToLighthouse}"),
                ExpressionParser.ParseToFunction($"dng:{DungeonIds.EndgameMagic} > 0 && dng:{DungeonIds.EndgamePyramid} " +
                    $"&& dng:{DungeonIds.EndgameAges} && Losses == 0")));
            achievements.Add(new($"HC:UberEndgame",
                "Exalted Emperor",
                "Complete the endgame dungeons at area level 200 without ever losing a fight",
                Conditions.AchievementUnlockedCondition($"HC:Endgame"),
                (e, w) => Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgameAges, DungeonLevels.LevelUberEndgame)
                            && Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgameMagic, DungeonLevels.LevelUberEndgame)
                            && Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgamePyramid, DungeonLevels.LevelUberEndgame)
                            && !Conditions.HasLostFights(e)));

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
                        Reward = GetRewardForLeveledAchievement(weapFamily, level)
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
                        Reward = GetRewardForLeveledAchievement(armorFamily, level)
                    };
                    achievements.Add(achievement);
                }
            }
            int mobsForStyles = 200;
            int mobsForAdvStyles = 1000;
            achievements.Add(new(Abilities.DualWield, "Dual Wielder", 
                $"Defeat {mobsForStyles} enemies while using two weapons",
                tautology,
                Conditions.MobsDefeatedConditionallyCondition(Tags.DualWield, mobsForStyles))
                { 
                    Reward = new AbilityReward(Abilities.DualWield)
                });
            achievements.Add(new("Adv" + Abilities.DualWield, "Advanced Dual Wielder",
                $"Defeat {mobsForAdvStyles} enemies while using two weapons",
                Conditions.AchievementUnlockedCondition(Abilities.DualWield),
                Conditions.MobsDefeatedConditionallyCondition(Tags.DualWield, mobsForAdvStyles))
            {
                Reward = new PerkReward(new("Adv" + Abilities.DualWield, "Weapon Block", 
                    $"While using two weapons, gain {3}/{6}/{10} global base evasion rating per level of the Two Weapons ability", 
                    [UpdateTrigger.AbilityIncreased], 
                    (l, e, w, c) => {
                        int level = e.GetComponent<AbilitiesComponent>()?
                                        .GetAbility(Abilities.DualWield)?.Level ?? 0;
                        int bonus = l switch
                        {
                            1 => 3,
                            2 => 6,
                            3 => 10,
                            _ => 0
                        };
                        return [
                            new("AdvDW_ev", ModifierType.AddBase, level * bonus, [Tags.EvasionRating, Tags.Global], [Tags.DualWield])
                        ];
                    })
                {
                    MaxLevel = 3
                })
            });
            achievements.Add(new(Abilities.Shielded, "Shieldbearer",
                $"Defeat {mobsForStyles} enemies while using a shield",
                tautology,
                Conditions.MobsDefeatedConditionallyCondition(Tags.Shielded, mobsForStyles))
            {
                Reward = new AbilityReward(Abilities.Shielded)
            });
            achievements.Add(new("Adv" + Abilities.Shielded, "Advanced Shieldbearer",
                $"Defeat {mobsForAdvStyles} enemies while using a shield",
                Conditions.AchievementUnlockedCondition(Abilities.Shielded),
                Conditions.MobsDefeatedConditionallyCondition(Tags.Shielded, mobsForAdvStyles))
            {
                Reward = new PerkReward(new("Adv" + Abilities.Shielded, "Shield Bash", 
                    $"Every third attack, gain {4}/{7}/{10} base damage + {1}/{2}/{4} per level of quality on your shield", 
                    [UpdateTrigger.EquipmentChanged, UpdateTrigger.AttackPerformed],
                    (l, e, w, c) => {
                        int quality = e.GetComponent<EquipmentComponent>()
                                        ?.GetItems()?.FirstOrDefault(i => i.IsShield())
                                        ?.GetComponent<ItemQualityComponent>()?.Quality ?? 0;
                        int bonus = l switch
                        {
                            1 => 2 * quality + 4,
                            2 => 3 * quality + 7,
                            3 => 4 * quality + 10,
                            _ => 0
                        };
                        bool active = (e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0) % 3 == 0;
                        return [
                            new("AdvSh_dmg", ModifierType.AddBase, active ? bonus : 0, [Tags.Damage], [Tags.Shielded])
                        ];
                    })
                { 
                    MaxLevel = 3 
                })
            });
            achievements.Add(new(Abilities.SingleHanded, "Fencer",
                $"Defeat {mobsForStyles} enemies while wielding only one one-handed weapon",
                tautology,
                Conditions.MobsDefeatedConditionallyCondition(Tags.SingleHanded, mobsForStyles))
            {
                Reward = new AbilityReward(Abilities.SingleHanded)
            });
            achievements.Add(new("Adv" + Abilities.SingleHanded, "Advanced Fencer",
                $"Defeat {mobsForAdvStyles} enemies while wielding only one one-handed weapon",
                Conditions.AchievementUnlockedCondition(Abilities.SingleHanded),
                Conditions.MobsDefeatedConditionallyCondition(Tags.SingleHanded, mobsForAdvStyles))
            {
                Reward = new PerkReward(new("Adv" + Abilities.SingleHanded, "Seize the Moment",
                    $"While fighting single-handedly, gain {0.07:0.#%}/{0.15:0.#%}/{0.25:0.#%} increased attack speed while evading", 
                    [],
                    (l, e, w, c) => [
                        new("AdvSH_as", ModifierType.Increase,
                        l switch { 1 => 0.07, 2 => 0.15, 3 => 0.25, _ => 0.0 },
                        [Tags.AttackSpeed], [Tags.SingleHanded, Tags.Evading])    
                    ])
                {
                    MaxLevel = 3
                })
            });
            achievements.Add(new(Abilities.TwoHanded, "Wide Swings",
                $"Defeat {mobsForStyles} enemies while wielding a two-handed weapon",
                tautology,
                Conditions.MobsDefeatedConditionallyCondition(Tags.TwoHanded, mobsForStyles))
            {
                Reward = new AbilityReward(Abilities.TwoHanded)
            });
            achievements.Add(new("Adv" + Abilities.TwoHanded, "Advanced Wide Swings",
                $"Defeat {mobsForAdvStyles} enemies while wielding a two-handed weapon",
                Conditions.AchievementUnlockedCondition(Abilities.TwoHanded),
                Conditions.MobsDefeatedConditionallyCondition(Tags.TwoHanded, mobsForAdvStyles))
            {
                Reward = new PerkReward(new("Adv" + Abilities.TwoHanded, "Distance Control",
                    $"Gain {0.3:0.#%}/{0.6:0.#%}/{1.0:0.#%} of your two-handed weapon's damage as global base armor rating", 
                    [UpdateTrigger.EquipmentChanged],
                    (l, e, w, c) => { 
                        double dmg = e.GetComponent<EquipmentComponent>()
                                    ?.GetItemInSlot(EquipmentSlot.Hand)
                                    ?.GetComponent<WeaponComponent>()?.Damage ?? 0;
                        double bonus = l switch
                        {
                            1 => 0.3,
                            2 => 0.6,
                            3 => 1.0,
                            _ => 0.0
                        };
                        return [
                            new("AdvTH_ar", ModifierType.AddBase, bonus * dmg, [Tags.ArmorRating, Tags.Global], [Tags.TwoHanded])
                        ];
                    })
                {
                    MaxLevel = 3
                })
            });

            // Fighting style level achievements
            achievements.Add(new(Abilities.DualWield + "25", "Dual Weapon Apprentice",
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
                    Reward = GetRewardForLeveledAchievement("oALL", level)
                };
                achievements.Add(achievement);
            }

            // Crafting achievements
            for (int i = 0; i < ranks.Length; i++)
            {
                int level = ranks[i].Level;
                achievements.Add(new(
                    $"{Abilities.Crafting}{level}",
                    $"{ranks[i].Rank} Blacksmith",
                    $"Train Crafting ability to level {level}",
                    (i > 0) ? ExpressionParser.ParseToFunction($"{Abilities.Crafting}{ranks[i - 1].Level}") : tautology,
                    ExpressionParser.ParseToFunction($"abl:ABL_CRAFT >= {level}"))
                {
                    Reward = GetRewardForLeveledAchievement(Abilities.Crafting, level)
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
                (250000, "Patrician"),
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
                    Reward = GetPerkForLeveledAchievement("TotalCoins", i + 1)
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
                    Reward = GetPerkForLeveledAchievement("MaxCoins", i + 1)
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
			achievements.Add(new(
				"FOUNDUBERCRYPT",
				"Archaeologist",
				$"Discover the {DungeonList.GetDungeon(DungeonIds.Crypt)!.Name}'s high level version",
				Conditions.DungeonLevelAvailableCondition(DungeonIds.Crypt, DungeonList.LevelUberCrypt),
				Conditions.DungeonLevelAvailableCondition(DungeonIds.Crypt, DungeonList.LevelUberCrypt)));
			achievements.Add(new(
                "DifferentQualities",
                "Happy Pride",
                "Wear items with six different quality levels above 0 at the same time",
                tautology,
                (e, w) => (e.GetComponent<EquipmentComponent>()
                            ?.GetItems()
                            ?.Select(i => i.GetBlueprint()?.Quality ?? 0)
                            ?.Distinct()
                            ?.Count(i => i > 0) ?? 0) >= 6)
                {
                    Reward = new PerkReward(new Perk("WellDressed", "Well Dressed", 
                        $"Gain {0.005:0.#%} increased damage and defense per level of quality on your gear", 
                        [UpdateTrigger.EquipmentChanged],
                        (_, e, w, c) =>
                        {
                            int total = e.GetComponent<EquipmentComponent>()?.GetItems()
                                ?.Sum(i => i.GetBlueprint()?.Quality ?? 0)
                                ?? 0;
                            return [ 
                                new($"WellDressed_dmg", ModifierType.Increase, 0.005 * total, [Tags.Damage],  []),
                                new($"WellDressed_def", ModifierType.Increase, 0.005 * total, [Tags.Defense], [])
                            ];
                        }))
                });


            // Unarmored/Unarmed achievements
            double noArmorBaseEvasion  = 25;
            double noArmorBaseEvasion1 = 1.0;
            double noArmorBaseEvasion2 = 2.0;
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
                        [Tags.EvasionRating, Tags.Global],
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
                    Reward = new PerkReward(PerkFactory.MakeAbilityLevelBasedPerk("HC:NOARMOR",
                        "Elusive",
                        $"Gain +{noArmorBaseEvasion1:0.#} evasion rating per level of the {Properties.LocalizedStrings.UNARMORED} ability",
                        Abilities.Unarmored,
                        ModifierType.AddBase,
                        noArmorBaseEvasion1,
                        [Tags.EvasionRating, Tags.Global],
                        []
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
                    Reward = new PerkReward(PerkFactory.MakeAbilityLevelBasedPerk("HC:NOARMOR_50",
                        "Ethereal Form",
                        $"Gain +{noArmorBaseEvasion2:0.#} base evasion rating per level of the {Properties.LocalizedStrings.UNARMORED} ability",
                        Abilities.Unarmored,
                        ModifierType.AddBase,
                        noArmorBaseEvasion2,
                        [Tags.EvasionRating, Tags.Global],
                        []
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
                    Reward = new PerkReward(PerkFactory.MakeAbilityLevelBasedPerk("NOWEAPON_DMG2",
                        "Iron Fists",
                        $"Gain +{noWeaponLevelDamageBonus:0.#} unarmed damage per level of the {Properties.LocalizedStrings.UNARMED} ability",
                        Abilities.Unarmed,
                        ModifierType.AddBase,
                        noWeaponLevelDamageBonus,
                        [Tags.Damage],
                        []
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
                    Reward = new PerkReward(PerkFactory.MakeAbilityLevelBasedPerk($"HC:NOWEAPON_{noWeaponMaxLevel}",
                        "Force of Spirit",
                        $"Gain {Stats.AttackBonusPerLevel:0.#%} increased unarmed damage per level of the {Properties.LocalizedStrings.UNARMED} ability",
                        Abilities.Unarmed,
                        ModifierType.Increase,
                        Stats.AttackBonusPerLevel,
                        [Tags.Damage],
                        []
                    ))
                }
            );

            achievements.Add(new(
                "HC:NOARMOR+NOWEAPON",
                "One with Nothing",
                "Defeat a level 75 enemy in the wilderness without raising a weapon or armor ability or losing a fight",
                ExpressionParser.ParseToFunction($"HC:NOWEAPON_{noWeaponMaxLevel}"),
                ExpressionParser.ParseToFunction("Wilderness >= 75 && abl:AXE <= 1 && abl:BLN <= 1 && abl:LBL <= 1 && abl:POL <= 1 " +
                    "&& abl:SBL <= 1 && abl:LAR <= 1 && abl:HAR <= 1 && Losses == 0"))
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

            // Speedrunning achievements
            Achievement MakeSpeedrunAchievement(int index, string name, string dungeonId, double target)
            {
                return new($"speed_{dungeonId}_{index}", name,
                    $"Complete the {DungeonList.GetDungeon(dungeonId)?.Name ?? "???"} in under {target:0.#} minutes",
                    index > 1
                        ? ExpressionParser.ParseToFunction($"speed_{dungeonId}_{index - 1}")
                        : Conditions.DungeonAvailableCondition(dungeonId),
                    ExpressionParser.ParseToFunction($"dng:{dungeonId} < {target * 60} && dng:{dungeonId} > 0"));
            }
            achievements.Add(MakeSpeedrunAchievement(1, "Rat Racer", DungeonIds.DenOfRats, 4.0));
            achievements.Add(MakeSpeedrunAchievement(1, "Fast Castle", DungeonIds.CultistCastle, 15.0));
            achievements.Add(MakeSpeedrunAchievement(1, "Speedrun", DungeonIds.Threshold, 45.0));
            achievements.Add(MakeSpeedrunAchievement(2, "Speedier Run", DungeonIds.Threshold, 35.0));
            achievements.Add(MakeSpeedrunAchievement(3, "Gold Medal", DungeonIds.Threshold, 25.0));
            achievements.Add(MakeSpeedrunAchievement(4, "Author Medal", DungeonIds.Threshold, 20.0));

            return achievements;
        }

        public static IAchievementReward? GetPerkForLeveledAchievement(string id, int level)
        {
            const double DualWieldKeystone = 0.2;
            const double ShieldedKeystone = 0.001;
            const double SingleHandedKeystone = 0.02;
            const double TwoHandedKeystone = 0.01;

            var perk = (id, level) switch
            {
                ("LVL", 150) => PerkFactory.MakeStaticPerk($"{id}{level}", "Quick Learner",
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
                                $"Gain {0.1:0.#} extra attacks per second with {id.Localize()}s",
                                ModifierType.AddFlat,
                                0.1,
                                [Tags.AttackSpeed, Abilities.Axe],
                                []),
                ("BLN", 25) => new($"{id}{level}", "Stunning Blow",
                                $"Gain +{1} global armor per level of the {Properties.LocalizedStrings.BLN} ability " +
                                $"while using {id.Localize()} after first strike",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (_, e, w, c) =>
                                {
                                    bool firstStrike = (e.GetComponent<BattlerComponent>()?.FirstStrike ?? false);
                                    bool usingBlunt = e.GetComponent<EquipmentComponent>()?.GetItems()
                                        ?.Any(i => i.GetComponent<ItemComponent>()!
                                        .Blueprint.GetRelatedAbilityId() == Abilities.Blunt) ?? false;
                                    int level = e.GetComponent<AbilitiesComponent>()?.GetAbility(Abilities.Blunt)?.Level ?? 0;
                                    return [ new($"{id}{level}", ModifierType.AddBase,
                                        (!firstStrike && usingBlunt) ? 1 * level : 0,
                                        [Tags.ArmorRating, Tags.Global],
                                        []
                                        )
                                    ];
                                }),
                ("LBL", 25) => PerkFactory.MakeStaticMultiModPerk($"{id}{level}", "Quick Slash",
                                $"{1.0:0.#%} more damage and attack speed during first attack with {id.Localize()}s",
                                [ModifierType.More, ModifierType.More],
                                [1.0, 1.0],
                                [[Tags.Damage, Abilities.LongBlade], [Tags.AttackSpeed, Abilities.LongBlade]],
                                [[Tags.FirstStrike], [Tags.FirstStrike]]),
                ("POL", 25) => PerkFactory.MakeStaticPerk($"{id}{level}", "Range Advantage",
                                $"{1.0:0.#%} more defenses during first attack with {id.Localize()}s",
                                ModifierType.More,
                                1.0,
                                [Tags.Defense],
                                [Tags.FirstStrike, Abilities.Polearm]),
                ("SBL", 25) => new($"{id}{level}", "Sneak Attack",
                                $"Deal 100% more damage per 25 levels of the {Properties.LocalizedStrings.SBL} ability with short blades on " +
                                $"your first attack every battle",
                                [UpdateTrigger.AbilityIncreased],
                                (_, e, w, c) =>
                                {
                                    int level = e.GetComponent<AbilitiesComponent>()?.GetAbility(Abilities.ShortBlade)?.Level ?? 0;
                                    return [ new($"{id}{level}", ModifierType.More,
                                        Math.Max(level / 25, 1),
                                        [Tags.Damage, Abilities.ShortBlade],
                                        [Tags.FirstStrike]
                                        )
                                    ];
                                }),
                ("AXE", 75) => new($"{id}{level}", "Frenzy",
                                $"Gain {0.02:0.#%}/{0.04:0.#%}/{0.06:0.#%} increased attack speed with {id.Localize()}s " +
                                    $"after every attack (up to {0.2:0.#%}/{0.4:0.#%}/{0.6:0.#%})",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (l, e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    return [ new($"{id}{level}", ModifierType.Increase, Math.Min(attacks * l * 0.02, 0.2 * l),
                                        [ Tags.AttackSpeed, Abilities.Axe ],
                                        [])
                                    ];
                                })
                { MaxLevel = 3 },
                ("BLN", 75) => new($"{id}{level}", "Armor Breaker",
                                $"Gain {0.05:0.#%}/{0.1:0.#%}/{0.15:0.#%} increased damage with {id.Localize()}s " +
                                    $"after every attack (up to {0.25:0.#%}/{0.5:0.#%}/{0.75:0.#%})",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (l, e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    return [ new($"{id}{level}", ModifierType.Increase, Math.Min(attacks * l * 0.05, 0.25 * l),
                                        [ Tags.Damage, Abilities.Blunt ],
                                        [])
                                    ];
                                })
                { MaxLevel = 3 },
                ("LBL", 75) => new($"{id}{level}", "Fluent Technique",
                                $"Gain {0.15:0.#%}/{0.3:0.#%}/{0.45:0.#%} increased damage or attack speed with {id.Localize()}s " +
                                    $"(changes after each attack)",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (l, e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    bool damage = (attacks % 2) == 0;
                                    return
                                    [
                                        new($"{id}{level}_dmg", ModifierType.Increase, damage ? 0.15 * l : 0.0,
                                            [ Tags.Damage, Abilities.LongBlade ], []
                                        ),
                                        new($"{id}{level}_spd", ModifierType.Increase, damage ? 0.0 : 0.15 * l,
                                            [ Tags.AttackSpeed, Abilities.LongBlade ], []
                                        )
                                    ];
                                })
                { MaxLevel = 3 },
                ("POL", 75) => new($"{id}{level}", "Skewering Thrusts",
                                $"Gain {0.2:0.#%}/{0.4:0.#%}/{0.6:0.#%} increased damage with {id.Localize()}s. Gets reduced " +
                                    $"by {0.02:0.#%}/{0.04:0.#%}/{0.06:0.#%} after every attack",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (l, e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    return [ new($"{id}{level}",
                                             ModifierType.Increase,
                                             0.02 * l * Math.Max(10 - attacks, 1),
                                             [ Tags.Damage, Abilities.Polearm ],
                                             [])
                                    ];
                                })
                { MaxLevel = 3 },
                ("SBL", 75) => new($"{id}{level}", "Critical Strikes",
                                $"Deal {1.0:0.#%}/{1.5:0.#%}/{2.0:0.#%} increased damage with {id.Localize()}s once every {5}/{4}/{3} attacks",
                                [UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted],
                                (l, e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    bool bonus = (attacks % (6 - l)) == (5 - l);
                                    return [ new($"{id}{level}",
                                             ModifierType.Increase,
                                             bonus ? 0.5 * (l + 1) : 0.0,
                                            [ Tags.Damage, Abilities.ShortBlade ],
                                            [])
                                    ];
                                })
                { MaxLevel = 3 },
                ("AXE" or "BLN" or "LBL" or "POL" or "SBL", 50)
                            => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Adept",
                                $"Gain {Stats.BigPerkFactor * Stats.BasicDamageIncrease:0.#%} increased damage with {id.Localize()}s",
                                ModifierType.Increase,
                                Stats.BigPerkFactor * Stats.BasicDamageIncrease,
                                [Tags.Damage, id],
                                [],
                                maxLevel: 3),
                ("AXE" or "BLN" or "LBL" or "POL" or "SBL", 100)
                            => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Master",
                                $"Gain a {Stats.MasterPerkMultiplier:0.#%} multiplier to ALL damage",
                                ModifierType.More,
                                Stats.MasterPerkMultiplier,
                                [Tags.Damage],
                                []),
                ("LAR", 50) => new($"{id}{level}", "Elegant Parry", $"Light shields also grant evasion rating equal to {.4:0.#%} of their armor",
                                [UpdateTrigger.EquipmentChanged],
                                (_, e, w, c) =>
                                {
                                    double shieldArmor = e.GetComponent<EquipmentComponent>()?.GetItems()
                                        ?.Where(i => i.IsShield())?.Sum(i => i.GetComponent<ArmorComponent>()?.Armor ?? 0.0) ?? 0.0;
                                    return [ new("ShieldEvasion", ModifierType.AddBase, 0.4 * shieldArmor,
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
                                    2 * Stats.BasicAttackSpeedIncrease,
                                    [Tags.AttackSpeed],
                                    [id]),
                ("LAR" or "HAR", 100)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Master",
                                    $"Gain a {Stats.MasterPerkMultiplier:0.#%} multiplier to ALL defenses",
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
                                    0.25,
                                    [Tags.CraftingCostEfficiency],
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
                (Abilities.DualWield, 25) => PerkFactory.MakeStaticMultiModPerk($"{id}{level}", $"Versatile",
                                    $"Gain {0.05:0.#%} more damage, attack speed and defense while using two weapons",
                                    Enumerable.Repeat(ModifierType.More, 3).ToList(),
                                    Enumerable.Repeat(0.05, 3).ToList(),
                                    [[Tags.Damage], [Tags.AttackSpeed], [Tags.Defense]],
                                    Enumerable.Repeat<IEnumerable<string>>([Tags.DualWield], 3).ToList()),
                (Abilities.DualWield, 50) => new($"{id}{level}", "Reckless Assault",
                                    $"Sacrifice some defense to gain an exponentially increasing multiplier to attack speed",
                                    [],
                                    (l, e, w, c) =>
                                    [
                                        new($"{id}{level}_as", ModifierType.More, Math.Pow(Stats.TradeoffPerkBonus, l) - 1.0, [ Tags.AttackSpeed ], []),
                                        new($"{id}{level}_def", ModifierType.More, Math.Pow(Stats.TradeoffPerkMalus, l) - 1.0, [ Tags.Defense ], [])
                                    ])
                {
                    MaxLevel = 5
                },
                (Abilities.DualWield, 75) => new($"{id}{level}", "Assassin",
                                    $"Gain {DualWieldKeystone} base damage per level of the {Properties.LocalizedStrings.ABL_DUALWIELD} ability",
                                    [UpdateTrigger.AbilityIncreased],
                                    (_, e, w, c) =>
                                    {
                                        int lvl = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_dmg", ModifierType.AddBase, DualWieldKeystone * lvl,
                                                [ Tags.Damage ],
                                                []
                                            )
                                        ];
                                    }),
                (Abilities.DualWield, 100) => PerkFactory.MakeStaticPerk($"{id}{level}",
                                    $"{Properties.LocalizedStrings.ABL_DUALWIELD} Master",
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
                                        new($"{id}{level}_def", ModifierType.More, Math.Pow(Stats.TradeoffPerkBonus, l) - 1.0, [ Tags.Defense ], []),
                                        new($"{id}{level}_dmg", ModifierType.More, Math.Pow(Stats.TradeoffPerkMalus, l) - 1.0, [ Tags.Damage ], [])
                                    ])
                {
                    MaxLevel = 5
                },
                (Abilities.Shielded, 75) => new($"{id}{level}", "Juggernaut",
                                    $"Gain {ShieldedKeystone:0.###%} increased damage per {1000} points of armor rating per level of " +
                                    $"the {Properties.LocalizedStrings.ABL_SHIELDED} ability",
                                    [UpdateTrigger.AbilityIncreased, UpdateTrigger.EquipmentChanged,
                                        UpdateTrigger.BattleStarted, UpdateTrigger.AttackPerformed],
                                    (_, e, w, c) =>
                                    {
                                        double armor = e.GetComponent<DefenseComponent>()?.Armor ?? 0;
                                        int lvl = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_dmg", ModifierType.Increase, ShieldedKeystone * (lvl * armor / 1000.0),
                                                [ Tags.Damage ],
                                                []
                                            )
                                        ];
                                    }),
                (Abilities.Shielded, 100) => PerkFactory.MakeStaticPerk($"{id}{level}",
                                    $"{Properties.LocalizedStrings.ABL_SHIELDED} Master",
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
                                    $"Gain base evasion rating per level of the {Properties.LocalizedStrings.ABL_SINGLEHANDED} " +
                                        $"ability while fighting single-handed",
                                    [UpdateTrigger.AbilityIncreased],
                                    (l, e, w, c) =>
                                    {
                                        int lvl = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_eva", ModifierType.AddBase, 1.0 * l * lvl,
                                                [Tags.EvasionRating, Tags.Global],
                                                [Tags.SingleHanded]
                                            )
                                        ];
                                    })
                { MaxLevel = 5 },
                (Abilities.SingleHanded, 75) => new($"{id}{level}", "Duelist",
                                    $"Gain {SingleHandedKeystone:0.#%} increased damage per level of the " +
                                        $"{Properties.LocalizedStrings.ABL_SINGLEHANDED} ability while evading",
                                    [UpdateTrigger.AbilityIncreased],
                                    (_, e, w, c) =>
                                    {
                                        int lvl = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_dmg", ModifierType.Increase, SingleHandedKeystone * lvl,
                                                [Tags.Damage],
                                                [Tags.Evading]
                                            )
                                        ];
                                    }),
                (Abilities.SingleHanded, 100) => PerkFactory.MakeStaticPerk($"{id}{level}",
                                    $"{Properties.LocalizedStrings.ABL_SINGLEHANDED} Master",
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
                                        new($"{id}{level}_dmg", ModifierType.More, Math.Pow(Stats.TradeoffPerkBonus, l) - 1.0, [ Tags.Damage ], []),
                                        new($"{id}{level}_as", ModifierType.More, Math.Pow(Stats.TradeoffPerkMalus, l) - 1.0, [ Tags.AttackSpeed ], [])
                                    ])
                {
                    MaxLevel = 5
                },
                (Abilities.TwoHanded, 75) => new($"{id}{level}", "Executioner",
                                    $"Gain {TwoHandedKeystone:0.#%} increased damage per second of attack time per level of " +
                                    $"{Properties.LocalizedStrings.ABL_TWOHANDED} ability",
                                    [UpdateTrigger.AbilityIncreased, UpdateTrigger.EquipmentChanged,
                                        UpdateTrigger.BattleStarted, UpdateTrigger.AttackPerformed],
                                    (_, e, w, c) =>
                                    {
                                        double cooldown = e.GetComponent<AttackComponent>()?.Cooldown.Duration ?? 0.0;
                                        int lvl = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_dmg", ModifierType.Increase, TwoHandedKeystone * cooldown * lvl,
                                                [ Tags.Damage ],
                                                []
                                            )
                                        ];
                                    }),
                (Abilities.TwoHanded, 100) => PerkFactory.MakeStaticPerk($"{id}{level}",
                                    $"{Properties.LocalizedStrings.ABL_TWOHANDED} Master",
                                    $"Gain a {Stats.MasterPerkMultiplier:0.#%} damage multiplier",
                                    ModifierType.More,
                                    Stats.MasterPerkMultiplier,
                                    [Tags.Damage],
                                    []),
                ("oALL", 25) => PerkFactory.MakeStaticMultiModPerk($"{id}{level}", "Fighting Affinity",
                                    $"{Stats.SavantXpMultiplier:0.#%} increased experience gain for fighting style abilities",
                                    Enumerable.Repeat(ModifierType.Increase, 4).ToList(),
                                    Enumerable.Repeat(Stats.SavantXpMultiplier, 4).ToList(),
                                    [
                                        [Tags.AbilityXpGain, Abilities.DualWield], 
                                        [Tags.AbilityXpGain, Abilities.Shielded], 
                                        [Tags.AbilityXpGain, Abilities.SingleHanded], 
                                        [Tags.AbilityXpGain, Abilities.TwoHanded]
                                    ],
                                    [[], [], [], []],
                                    alwaysActive: true),
                ("oALL", 50) => PerkFactory.MakeStaticMultiModPerk($"{id}{level}", "Allrounder", 
                                    "",
                                    Enumerable.Repeat(ModifierType.Increase, 4).ToList(),
                                    [Stats.BasicDamageIncrease, Stats.BasicAttackSpeedIncrease, Stats.BasicDefenseIncrease, Stats.BasicTimeIncrease],
                                    [[Tags.Damage], [Tags.AttackSpeed], [Tags.Defense], [Tags.TimeShield]],
                                    [[], [], [], []],
                                    maxLevel: 3),
                ("oALL", 100) => new($"{id}{level}", "Ascendant",
                                    $"Gain half the bonuses of the Assassin, Juggernaut, Duelist and Executioner perks",
                                    [UpdateTrigger.AbilityIncreased, UpdateTrigger.EquipmentChanged,
                                        UpdateTrigger.BattleStarted, UpdateTrigger.AttackPerformed],
                                    (_, e, w, c) =>
                                    {
                                        double cooldown = e.GetComponent<AttackComponent>()?.Cooldown.Duration ?? 0.0;
                                        double armor = e.GetComponent<DefenseComponent>()?.Armor ?? 0.0;
                                        var abilitiesComp = e.GetComponent<AbilitiesComponent>();
                                        int lvlDW = abilitiesComp?.GetAbility(Abilities.DualWield)?.Level ?? 0;
                                        int lvlSh = abilitiesComp?.GetAbility(Abilities.Shielded)?.Level ?? 0;
                                        int lvlSi = abilitiesComp?.GetAbility(Abilities.SingleHanded)?.Level ?? 0;
                                        int lvlTH = abilitiesComp?.GetAbility(Abilities.TwoHanded)?.Level ?? 0;
                                        return
                                        [
                                            new($"{id}{level}_DW", ModifierType.AddBase, DualWieldKeystone / 2 * lvlDW, [Tags.Damage], []),
                                            new($"{id}{level}_Sh", ModifierType.Increase, ShieldedKeystone / 2 * (lvlSh * armor / 1000.0), [Tags.Damage], []),
                                            new($"{id}{level}_Si", ModifierType.Increase, SingleHandedKeystone / 2 * lvlSi, [Tags.Damage], [Tags.Evading]),
                                            new($"{id}{level}_TH", ModifierType.Increase, TwoHandedKeystone / 2 * cooldown * lvlTH, [Tags.Damage], [])
                                        ];
                                    }),
                _ => null
            };
            return perk != null ? new PerkReward(perk) as dynamic : null;
        }

        public static IAchievementReward? GetRewardForLeveledAchievement(string id, int level)
        {
            switch (id, level)
            {
                case (Abilities.Axe 
                    or Abilities.Blunt 
                    or Abilities.LongBlade 
                    or Abilities.Polearm 
                    or Abilities.ShortBlade 
                    or Abilities.LightArmor 
                    or Abilities.HeavyArmor 
                    or Abilities.Crafting, 150): return new TextReward($"Natural Affinity for '{id.Localize()}' ability");
                case ("oALL", 25): return new TextReward("Natural Affinity for fighting styles");
                case ("oALL", 75): return new AbilityLevelReward(Abilities.Styles, 25);
                default: 
                    break;
            }
            return GetPerkForLeveledAchievement(id, level);
        }
    }
}
