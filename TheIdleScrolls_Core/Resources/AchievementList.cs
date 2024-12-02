using MiniECS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
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
                    Perk = GetPerkForLeveledAchievement("LVL", level)
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
                    Perk = GetPerkForLeveledAchievement("WILD", level)
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
                    Perk = GetPerkForLeveledAchievement("KILL", count)
                };
                achievements.Add(achievement);
            }

            // Dungeon achievements
            (string Id, string Name, string Title)[] dungeons =
            [
                (Definitions.DungeonIds.Crypt, Properties.Places.Dungeon_Crypt, "Cryptkeeper"),
                (Definitions.DungeonIds.MercenaryCamp, Properties.Places.Dungeon_MercenaryCamp, "Sellsword Slayer"),
                (Definitions.DungeonIds.Lighthouse, Properties.Places.Dungeon_Lighthouse, "I Shall Be Light"),
                (Definitions.DungeonIds.Temple, Properties.Places.Dungeon_Temple, "I Shall Keep Faith"),
                (Definitions.DungeonIds.CultistCastle, Properties.Places.Dungeon_CultistCastle, "I Shall Hone My Craft"),
                (Definitions.DungeonIds.Labyrinth, Properties.Places.Dungeon_Labyrinth, "I Shall Have No Mercy"),
                (Definitions.DungeonIds.ReturnToLighthouse, Properties.Places.Dungeon_ReturnToLighthouse, "I Shall Have No Fear"),
                (Definitions.DungeonIds.Threshold, Properties.Places.Dungeon_Threshold, "I Shall Have No Remorse"),
            ];
            foreach (var (id, name, title) in dungeons)
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
                if (id == Definitions.DungeonIds.Threshold)
                {
                    achievement.Description = $"Hold off the demonic invasion at {name}";
                }
                achievements.Add(achievement);
            }
            // Void dungeon achievements
            achievements.Add(new("DNG:VOID",
                "Void Scout",
                $"Complete your first excursion to {Properties.Places.Dungeon_Void}",
                Conditions.DungeonAvailableCondition(Definitions.DungeonIds.Void),
                Conditions.DungeonCompletedCondition(Definitions.DungeonIds.Void)));
            achievements.Add(new("DNG:VOID@100",
                "Void Explorer",
                $"Complete {Properties.Places.Dungeon_Void} at area level 100",
                Conditions.AchievementUnlockedCondition("DNG:VOID"),
                Conditions.DungeonLevelCompletedCondition(Definitions.DungeonIds.Void, 100)));
            achievements.Add(new("DNG:VOID@125",
                "Void Conqueror",
                $"Complete {Properties.Places.Dungeon_Void} at area level 125",
                Conditions.AchievementUnlockedCondition("DNG:VOID@100"),
                Conditions.DungeonLevelCompletedCondition(Definitions.DungeonIds.Void, 125)));
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

            achievements.Add(new($"HC:{Definitions.DungeonIds.Crypt}",
                "Attentive Dungeoneer",
                $"Complete the Crypt without ever losing a fight",
                ExpressionParser.ParseToFunction($"DNG:{Definitions.DungeonIds.Crypt}"),
                ExpressionParser.ParseToFunction($"dng:{Definitions.DungeonIds.Crypt} > 0 && Losses == 0")));
            achievements.Add(new($"HC:{Definitions.DungeonIds.Lighthouse}",
                "So that All May Find You",
                "Complete the Beacon without ever losing a fight",
                ExpressionParser.ParseToFunction($"DNG:{Definitions.DungeonIds.Lighthouse}"),
                ExpressionParser.ParseToFunction($"dng:{Definitions.DungeonIds.Lighthouse} > 0 && Losses == 0")));
            achievements.Add(new($"HC:{Definitions.DungeonIds.ReturnToLighthouse}",
                "Hardcore",
                "Complete the Lighthouse without ever losing a fight",
                ExpressionParser.ParseToFunction($"DNG:{Definitions.DungeonIds.ReturnToLighthouse}"),
                ExpressionParser.ParseToFunction($"dng:{Definitions.DungeonIds.ReturnToLighthouse} > 0 && Losses == 0")));

            // Ability achievements
            (int Level, string Rank)[] ranks =
            [
                ( 25, "Apprentice"),
                ( 50, "Adept"),
                ( 75, "Expert"),
                (100, "Master"),
                (150, "Grandmaster")
            ];
            foreach (string weapFamily in Definitions.Abilities.Weapons)
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
                        Perk = GetPerkForLeveledAchievement(weapFamily, level)
                    };
                    achievements.Add(achievement);
                }
            }
            foreach (string armorFamily in Definitions.Abilities.Armors)
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
                        Perk = GetPerkForLeveledAchievement(armorFamily, level)
                    };
                    achievements.Add(achievement);
                }
            }

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
                $"Reach ability level {level} for all weapon and armor types with a single character",
                (i > 0) ? ExpressionParser.ParseToFunction($"{oAllRanks[i - 1].Rank[..1]}oALL") : tautology,
                ExpressionParser.ParseToFunction($"abl:AXE >= {level} && abl:BLN >= {level} && abl:LBL >= {level} " +
                    $"&& abl:POL >= {level} && abl:SBL >= {level} && abl:LAR >= {level} && abl:HAR >= {level}"))
                {
                    Perk = GetPerkForLeveledAchievement("oALL", level)
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
                    Perk = GetPerkForLeveledAchievement("ABL_CRAFT", level)
                });
            }
            string[] craftNames = [ "Transmuted", "Augmented", "Regal", "Exalted", "Divine" ];
            for (int i = 0; i < craftNames.Length; i++)
            {
                achievements.Add(new(
                    $"BestReforge+{i + 1}",
                    $"{craftNames[i]} Craft",
                    $"Reforge an item to +{i + 1} rarity",
                    (i > 0) ? ExpressionParser.ParseToFunction($"BestReforge+{i}") : tautology,
                    ExpressionParser.ParseToFunction($"BestReforge >= {i + 1}")));
            }
            Achievement tier0Reforge = new(
                "G0Reforge+3",
                "Still not Viable",
                "Reforge a training item to +3 rarity or higher",
                ExpressionParser.ParseToFunction("BestReforge+3"),
                ExpressionParser.ParseToFunction("BestG0Craft >= 3")
                )
            {
                Hidden = true
            };
            achievements.Add(tier0Reforge);

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
                    Perk = GetPerkForLeveledAchievement("TotalCoins", coins)
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
                    Perk = GetPerkForLeveledAchievement("MaxCoins", coins)
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
            double noArmorBaseEvasion1 = 0.5;
            double noArmorBaseEvasion2 = 1.0;
            achievements.Add(new(
                "NOARMOR",
                "Wollt Ihr Ewig Leben?!",
                $"Complete the {Properties.Places.Dungeon_Lighthouse} without ever raising an armor ability",
                tautology,
                ExpressionParser.ParseToFunction($"dng:{Definitions.DungeonIds.Lighthouse} > 0 && abl:LAR <= 10 && abl:HAR <= 10"))
                {
                    Perk = PerkFactory.MakeCharacterLevelBasedPerk("NOARMOR",
                        "Elusive",
                        $"Gain +{noArmorBaseEvasion1:0.#} evasion rating for each level",
                        ModifierType.AddBase,
                        noArmorBaseEvasion1,
                        [Definitions.Tags.EvasionRating],
                        [Definitions.Tags.Unarmored])
                });
            achievements.Add(new(
                "HC:NOARMOR",
                "Not Today",
                "Complete the Beacon without ever raising an armor ability or losing a fight",
                ExpressionParser.ParseToFunction("NOARMOR"),
                ExpressionParser.ParseToFunction($"dng:{Definitions.DungeonIds.Lighthouse} > 0 && abl:LAR <= 10 && abl:HAR <= 10 && Losses == 0"))
                {
                    Perk = PerkFactory.MakeCharacterLevelBasedPerk("HC:NOARMOR",
                        "Ethereal Form",
                        $"Gain +{noArmorBaseEvasion2:0.#} evasion rating for each level",
                        ModifierType.AddBase,
                        noArmorBaseEvasion2,
                        [Definitions.Tags.EvasionRating],
                        [Definitions.Tags.Unarmored])
            });
            achievements.Add(new(
                "HC:NOARMOR_50",
                "Armored in Faith",
                "Reach level 50 without ever raising an armor ability or losing a fight",
                ExpressionParser.ParseToFunction("HC:NOARMOR"),
                ExpressionParser.ParseToFunction("Level >= 50 && abl:LAR <= 10 && abl:HAR <= 10 && Losses == 0"))
                {
                Perk = PerkFactory.MakeCharacterLevelBasedPerk("HC:NOARMOR_50",
                        "Armored in Faith",
                        "Character level scales unarmored evasion rating like abilities scale regular armors",
                        ModifierType.More,
                        Definitions.Stats.DefensePerAbilityLevel,
                        [Definitions.Tags.EvasionRating],
                        [Definitions.Tags.Unarmored])
            });

            int noWeaponLevel = 5;
            double noWeaponBaseDamageBonus = 3.0;
            double noWeaponLevelDamageBonus = 0.1;
            double noWeaponMaxLevel = 40;
            achievements.Add(new(
                "NOWEAPON",
                "Boxer",
                $"Reach level {noWeaponLevel} without ever raising a weapon ability",
                tautology,
                ExpressionParser.ParseToFunction($"Level >= {noWeaponLevel} && abl:AXE <= 10 && abl:BLN <= 10 " +
                    $"&& abl:LBL <= 10 && abl:POL <= 10 && abl:SBL <= 10"))
                {
                    Perk = PerkFactory.MakeStaticPerk("NOWEAPON",
                        "Boxer",
                        $"Increased base damage with unarmed attacks",
                        ModifierType.AddBase,
                        noWeaponBaseDamageBonus,
                        [Definitions.Tags.Damage],
                        [Definitions.Tags.Unarmed])
                });
            achievements.Add(new(
                "NOWEAPON_DNG",
                "Prize Fighter",
                $"Complete the {Properties.Places.Dungeon_RatDen} without ever raising a weapon ability",
                ExpressionParser.ParseToFunction("NOWEAPON"),
                ExpressionParser.ParseToFunction($"dng:{Definitions.DungeonIds.DenOfRats} > 0 && abl:AXE <= 10 " +
                    $"&& abl:BLN <= 10 && abl:LBL <= 10 && abl:POL <= 10 && abl:SBL <= 10"))
                {
                    Perk = PerkFactory.MakeCharacterLevelBasedPerk("HC:NOWEAPON",
                        "Iron Fists",
                        $"Gain +{noWeaponLevelDamageBonus:0.#} unarmed damage for each level",
                        ModifierType.AddBase,
                        noWeaponLevelDamageBonus,
                        [Definitions.Tags.Damage],
                        [Definitions.Tags.Unarmed])
            });
            achievements.Add(new(
                "NOWEAPON_DNG2",
                "I Am The Greatest!",
                $"Complete the {Properties.Places.Dungeon_Lighthouse} without ever raising a weapon ability",
                ExpressionParser.ParseToFunction("NOWEAPON_DNG"),
                ExpressionParser.ParseToFunction($"dng:{Definitions.DungeonIds.Lighthouse} > 0 && abl:AXE <= 10 " +
                    $"&& abl:BLN <= 10 && abl:LBL <= 10 && abl:POL <= 10 && abl:SBL <= 10"))
            {
                Perk = PerkFactory.MakeCharacterLevelBasedPerk("HC:NOWEAPON",
                        "Force of Spirit",
                        $"Gain {Definitions.Stats.AttackBonusPerLevel:0.#%} increased unarmed damage per character level",
                        ModifierType.Increase,
                        Definitions.Stats.AttackBonusPerLevel,
                        [Definitions.Tags.Damage],
                        [Definitions.Tags.Unarmed])
            });
            achievements.Add(new(
                $"HC:NOWEAPON_{noWeaponMaxLevel}",
                "Path of the Monk",
                $"Reach level {noWeaponMaxLevel} without ever raising a weapon ability or losing a fight",
                ExpressionParser.ParseToFunction("NOWEAPON_DNG2"),
                ExpressionParser.ParseToFunction($"Level >= {noWeaponMaxLevel} && abl:AXE <= 10 && abl:BLN <= 10 && abl:LBL <= 10 " +
                    "&& abl:POL <= 10 && abl:SBL <= 10 && Losses == 0"))
            {
                Perk = PerkFactory.MakeCharacterLevelBasedMultiModPerk($"HC:NOWEAPON_{noWeaponMaxLevel}",
                        "The Blade Within",
                        "Character level scales unarmed damage like abilities scale weapon damage",
                        [ModifierType.More, ModifierType.More],
                        [Definitions.Stats.AttackDamagePerAbilityLevel, Definitions.Stats.AttackSpeedPerAbilityLevel],
                        [
                            [Definitions.Tags.Damage],
                            [Definitions.Tags.AttackSpeed]
                        ],
                        [
                            [Definitions.Tags.Unarmed],
                            [Definitions.Tags.Unarmed]
                        ])
            });

            achievements.Add(new(
                "HC:NOARMOR+NOWEAPON",
                "One with Nothing",
                "Defeat a level 75 enemy in the wilderness without raising any ability or losing a fight",
                ExpressionParser.ParseToFunction($"HC:NOWEAPON_{noWeaponMaxLevel}"),
                ExpressionParser.ParseToFunction("Wilderness >= 75 && abl:AXE <= 10 && abl:BLN <= 10 && abl:LBL <= 10 && abl:POL <= 10 " +
                    "&& abl:SBL <= 10 && abl:LAR <= 10 && abl:HAR <= 10 && Losses == 0"))
                {
                    Perk = PerkFactory.MakeStaticMultiModPerk("HC:NOARMOR+NOWEAPON",
                        "Luminous Being",
                        "Gain a significant bonus to damage and defense while unarmed and unarmored",
                        [ModifierType.More, ModifierType.More],
                        [0.5, 0.5],
                        [
                            [Definitions.Tags.Damage],
                            [Definitions.Tags.Defense]
                        ],
                        [
                            [Definitions.Tags.Unarmed, Definitions.Tags.Unarmored],
                            [Definitions.Tags.Unarmed, Definitions.Tags.Unarmored]
                        ])
                });


            return achievements;
        }

        public static Perk? GetPerkForLeveledAchievement(string id, int level)
        {
            return (id, level) switch
            {
                ("LVL", 150) => PerkFactory.MakeStaticPerk($"{id}{level}", "Experienced",
                                $"Gain {0.3:0.#%} increased experience",
                                ModifierType.Increase,
                                0.3,
                                [Definitions.Tags.CharacterXpGain],
                                []),
                ("KILL", 1000) => PerkFactory.MakeStaticPerk($"{id}{level}", "Battle Tested",
                                $"Gain {0.5:0.#%} increased damage",
                                ModifierType.Increase,
                                0.5,
                                [Definitions.Tags.Damage],
                                []),
                ("AXE", 25) => PerkFactory.MakeStaticPerk($"{id}{level}", "Furious Swings", 
                                $"{0.2:0.#} extra attacks per second with {id.Localize()}s",
                                ModifierType.AddFlat,
                                0.2,
                                [Definitions.Tags.AttackSpeed, Definitions.Abilities.Axe],
                                []),
                ("BLN", 25) => new($"{id}{level}", "Stunning Blow", 
                                $"Gain {5} to base armor value for each defensive item while using {id.Localize()} after first strike",
                                [ UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted ],
                                (e, w, c) =>
                                {
                                    bool firstStrike = (e.GetComponent<BattlerComponent>()?.FirstStrike ?? false);
                                    bool usingBlunt = e.GetComponent<EquipmentComponent>()?.GetItems()
                                        ?.Any(i => i.GetComponent<ItemComponent>()!
                                        .Blueprint.GetRelatedAbilityId() == Definitions.Abilities.Blunt) ?? false;
                                    return [ new($"{id}{level}", ModifierType.AddBase, 
                                        (!firstStrike && usingBlunt) ? 5 : 0,
                                        [Definitions.Tags.ArmorRating],
                                        []
                                        )
                                    ];
                                }),
                ("LBL", 25) => PerkFactory.MakeStaticPerk($"{id}{level}", "Quick Slash",
                                $"{1.0:0.#%} more attack speed during first attack with {id.Localize()}s",
                                ModifierType.More,
                                1.0,
                                [ Definitions.Tags.AttackSpeed, Definitions.Abilities.LongBlade], 
                                [ Definitions.Tags.FirstStrike ]),
                ("POL", 25) => PerkFactory.MakeStaticPerk($"{id}{level}", "Range Advantage",
                                $"{1.0:0.#%} more defenses during first attack with {id.Localize()}s",
                                ModifierType.More,
                                1.0,
                                [ Definitions.Tags.Defense ], 
                                [ Definitions.Tags.FirstStrike, Definitions.Abilities.Polearm ]),
                ("SBL", 25) => PerkFactory.MakeStaticPerk($"{id}{level}", "Sneak Attack",
                                $"Deal double damage with short blades on your first attack every battle",
                                ModifierType.More,
                                1.0,
                                [ Definitions.Tags.Damage, Definitions.Abilities.ShortBlade ],
                                [ Definitions.Tags.FirstStrike ]),
                ("AXE", 75) => new($"{id}{level}", "Frenzy", 
                                $"Gain {0.05:0.#%} more attack speed with {id.Localize()}s after every attack (up to {0.5:0.#%})",
                                [ UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted ],
                                (e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    return [ new($"{id}{level}", ModifierType.More, Math.Min(attacks * 0.05, 0.5),
                                        [ Definitions.Tags.AttackSpeed, Definitions.Abilities.Axe ],
                                        [])
                                    ];
                                }),
                ("BLN", 75) => new($"{id}{level}", "Armor Breaker",
                                $"Gain {0.05:0.#%} more damage with {id.Localize()} after every attack (up to {0.5:0.#%})",
                                [ UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted ],
                                (e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    return [ new($"{id}{level}", ModifierType.More, Math.Min(attacks * 0.05, 0.5),
                                        [ Definitions.Tags.Damage, Definitions.Abilities.Blunt ],
                                        [])
                                    ];
                                }),
                ("LBL", 75) => new($"{id}{level}", "Fluent Technique",
                                $"Gain {0.25:0.#%} more damage or attack speed with {id.Localize()}s (changes after each attack)",
                                [ UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted ],
                                (e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    bool damage = (attacks % 2) == 0;
                                    return
                                    [
                                        new($"{id}{level}_dmg", ModifierType.More, damage ? 0.25 : 0.0,
                                            [ Definitions.Tags.Damage, Definitions.Abilities.LongBlade ],
                                            []
                                        ),
                                        new($"{id}{level}_spd", ModifierType.More, damage ? 0.0 : 0.25,
                                            [ Definitions.Tags.AttackSpeed, Definitions.Abilities.LongBlade ],
                                            []
                                        )
                                    ];
                                }),
                ("POL", 75) => new($"{id}{level}", "Skewer",
                                $"Gain {0.05:0.#%} more damage with {id.Localize()}s after every attack (up to {0.5:0.#%})",
                                [ UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted ],
                                (e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    return [ new($"{id}{level}", ModifierType.More, Math.Min(attacks * 0.05, 0.5),
                                        [ Definitions.Tags.Damage, Definitions.Abilities.Blunt ], 
                                        [])
                                    ];
                                }),
                ("SBL", 75) => new($"{id}{level}", "Critical Strikes",
                                $"Deal {1.0:0.#%} more damage with {id.Localize()}s every third attack",
                                [ UpdateTrigger.AttackPerformed, UpdateTrigger.BattleStarted ],
                                (e, w, c) =>
                                {
                                    int attacks = e.GetComponent<BattlerComponent>()?.AttacksPerformed ?? 0;
                                    bool bonus = (attacks % 3) == 2; 
                                    return [ new($"{id}{level}", ModifierType.More, bonus ? 1.0 : 0.0,
                                        [ Definitions.Tags.Damage, Definitions.Abilities.ShortBlade ],
                                        [])
                                    ];
                                }),
                ("AXE" or "BLN" or "LBL" or "POL" or "SBL", 50)
                            => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Adept",
                                $"Gain a {0.1:0.#%} damage multiplier with {id.Localize()} weapons",
                                ModifierType.More,
                                0.1,
                                [ Definitions.Tags.Damage, id ],
                                []),
                ("AXE" or "BLN" or "LBL" or "POL" or "SBL", 100) 
                            => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Master", 
                                $"Gain a {0.1:0.#%} damage multiplier", 
                                ModifierType.More,
                                0.1,
                                [ Definitions.Tags.Damage ],
                                []),
                ("LAR", 50) => new($"{id}{level}", "Elegant Parry", $"Light shields also grant evasion rating equal to {.25:0.#%} of their armor",
                                [ UpdateTrigger.EquipmentChanged ],
                                (e, w, c) =>
                                {
                                    double shieldArmor = e.GetComponent<EquipmentComponent>()?.GetItems()
                                        ?.Where(i => i.IsShield())?.Sum(i => i.GetComponent<ArmorComponent>()?.Armor ?? 0.0) ?? 0.0;
                                    return [ new("ShieldEvasion", ModifierType.AddBase, 0.25 * shieldArmor, 
                                        [ Definitions.Tags.Shield, 
                                          Definitions.Tags.EvasionRating,
                                          Definitions.Abilities.LightArmor ],
                                        []
                                        ) 
                                    ];
                                }),
                ("HAR", 50) => new($"{id}{level}", "Bulwark", $"{0.5:0.#%} multiplier to defenses from equipped heavy shield",
                                [ UpdateTrigger.EquipmentChanged ],
                                (e, w, c) =>
                                {
                                    double shieldArmor = e.GetComponent<EquipmentComponent>()?.GetItems()
                                        ?.Where(i => i.IsShield())?.Sum(i => i.GetComponent<ArmorComponent>()?.Armor ?? 0.0) ?? 0.0;
                                    return [ new("ShieldArmor", ModifierType.More, 0.5,
                                        [ Definitions.Tags.Shield,
                                          Definitions.Tags.Defense,
                                          Definitions.Abilities.HeavyArmor ],
                                        []
                                        )
                                    ];
                                }),
                ("LAR" or "HAR", 25)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Apprentice",
                                    $"Gain a {0.1:0.#%} defense multiplier with {id.Localize()}",
                                    ModifierType.More,
                                    0.1,
                                    [ Definitions.Tags.Defense, id ],
                                    []),
                ("LAR" or "HAR", 75)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", $"Comfortable in {id.Localize()}",
                                    $"Gain a {0.1:0.#%} attack speed multiplier while wearing {id.Localize()}",
                                    ModifierType.More,
                                    0.1,
                                    [ Definitions.Tags.AttackSpeed ], 
                                    [ id ]),
                ("LAR" or "HAR", 100)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Master",
                                    $"Gain a {0.1:0.#%} defense multiplier",
                                    ModifierType.More,
                                    0.1,
                                    [ Definitions.Tags.Defense ], 
                                    []),
                ("ABL_CRAFT", 25)
                                => new($"{id}{level}", "Crafting Apprentice", 
                                    $"Gain an additional slot in the crafting queue for every 25 levels of the Crafting ability",
                                    [ UpdateTrigger.AbilityIncreased ],
                                    (e, w, c) =>
                                    {
                                        int craftLevel = e.GetComponent<AbilitiesComponent>()?.GetAbility(id)?.Level ?? 0;
                                        return [ new($"{id}{level}", ModifierType.AddFlat, (craftLevel / 25),
                                            [ Definitions.Tags.CraftingSlots ], 
                                            []
                                            )
                                        ];
                                    }),
                ("ABL_CRAFT", 50)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", "Crafting Journeyman",
                                    $"Gain a discount for all crafts",
                                    ModifierType.Increase,
                                    -0.2,
                                    [ Definitions.Tags.CraftingCost ],
                                    []),
                ("ABL_CRAFT", 75)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", "Crafting Expert",
                                    $"Gain increased crafting speed",
                                    ModifierType.Increase,
                                    0.2,
                                    [ Definitions.Tags.CraftingSpeed ], 
                                    []),
                ("ABL_CRAFT", 100)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", "Crafting Master",
                                    $"Gain one more active crafting slot",
                                    ModifierType.AddFlat,
                                    1.0,
                                    [ Definitions.Tags.ActiveCrafts ], 
                                    []),
                ("AXE" or "BLN" or "LBL" or "POL" or "SBL" or "LAR" or "HAR" or "ABL_CRAFT", 150)
                                => PerkFactory.MakeStaticPerk($"{id}{level}", $"{id.Localize()} Savant",
                                    $"{0.3:0.#%} increased experience gain for {id.Localize()} ability",
                                    ModifierType.Increase,
                                    0.3,
                                    [ Definitions.Tags.AbilityXpGain, id ], 
                                    []),
                _ => null
            };
        }
    }
}
