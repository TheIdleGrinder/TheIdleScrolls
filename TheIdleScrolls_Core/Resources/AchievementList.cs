using MiniECS;
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
			achievements.Add(new("DUNGEONGRIND12",
	            "Dungeon Delver's Dozen",
	            $"Complete dungeons 12 times",
	            Conditions.DungeonAvailableCondition(DungeonIds.DenOfRats),
	            Conditions.DungeonClearCountCondition(12)
            )
			{
				Reward = new FeatureReward(GameFeature.DungeonGrinding, "Auto-grind dungeons")
			});

            // Hardcore achievements
            int hcWildLevel = 75;
            achievements.Add(new("HC:WILDERNESS",
                "Attentive Grinder",
                $"Defeat a level {hcWildLevel} enemy in the wilderness without ever losing a fight",
                ExpressionParser.ParseToFunction("WILD50"),
                ExpressionParser.ParseToFunction($"Wilderness >= {hcWildLevel} && Losses == 0")));

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
