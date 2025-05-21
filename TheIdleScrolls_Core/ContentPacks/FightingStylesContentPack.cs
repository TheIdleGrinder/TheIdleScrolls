using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Utility;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class FightingStylesContentPack : IContentPack
	{
		public string Id => "CP_FightingStyles";
		public string Name => "Fighting Styles";
		public string Description => "This pack contains content that is related to unlocking and using fighting styles";
		public List<IContentPiece> ContentPieces
        {
            get
            {
                List<IContentPiece> contentPieces = [];
                
                List<Achievement> achievements = [];
                const int mobsForStyles = 200;
                const int mobsForAdvStyles = 1000;

                achievements.Add(new(Abilities.DualWield, "Dual Wielder", 
                    $"Defeat {mobsForStyles} enemies while using two weapons",
                    (e, w) => true,
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
                    (e, w) => true,
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
                    (e, w) => true,
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
                    (e, w) => true,
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
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.DualWield, 25)
                });
                achievements.Add(new(Abilities.DualWield + "50", $"Dual Weapon Adept",
                    $"Reach ability level {50} for the {AbilityList.GetAbility(Abilities.DualWield)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.DualWield + "25"),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.DualWield], 50))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.DualWield, 50)
                });
                achievements.Add(new(Abilities.DualWield + "75", "Dual Weapon Expert",
                    $"Reach ability level {75} for the {AbilityList.GetAbility(Abilities.DualWield)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.DualWield + "50"),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.DualWield], 75))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.DualWield, 75)
                });
                achievements.Add(new(Abilities.DualWield + "100", "Dual Weapon Master",
                    $"Reach ability level {100} for the {AbilityList.GetAbility(Abilities.DualWield)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.DualWield + "75"),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.DualWield], 100))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.DualWield, 100)
                });
                achievements.Add(new(Abilities.Shielded + "50", "Shield Adept",
                    $"Reach ability level {50} for the {AbilityList.GetAbility(Abilities.Shielded)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.Shielded),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.Shielded], 50))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.Shielded, 50)
                });
                achievements.Add(new(Abilities.Shielded + "75", "Shield Expert",
                    $"Reach ability level {75} for the {AbilityList.GetAbility(Abilities.Shielded)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.Shielded + "50"),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.Shielded], 75))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.Shielded, 75)
                });
                achievements.Add(new(Abilities.Shielded + "100", "Shield Master",
                    $"Reach ability level {100} for the {AbilityList.GetAbility(Abilities.Shielded)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.Shielded + "75"),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.Shielded], 100))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.Shielded, 100)
                });
                achievements.Add(new(Abilities.SingleHanded + "25", "Fire Dancer",
                    $"Reach ability level {25} for the {AbilityList.GetAbility(Abilities.SingleHanded)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.SingleHanded),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.SingleHanded], 25))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.SingleHanded, 25)
                });
                achievements.Add(new(Abilities.SingleHanded + "50", "Single Weapon Adept",
                    $"Reach ability level {50} for the {AbilityList.GetAbility(Abilities.SingleHanded)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.SingleHanded + "25"),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.SingleHanded], 50))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.SingleHanded, 50)
                });
                achievements.Add(new(Abilities.SingleHanded + "75", "Single Weapon Expert",
                    $"Reach ability level {75} for the {AbilityList.GetAbility(Abilities.SingleHanded)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.SingleHanded + "50"),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.SingleHanded], 75))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.SingleHanded, 75)
                });
                achievements.Add(new(Abilities.SingleHanded + "100", "Single Weapon Master",
                    $"Reach ability level {100} for the {AbilityList.GetAbility(Abilities.SingleHanded)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.SingleHanded + "75"),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.SingleHanded], 100))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.SingleHanded, 100)
                });
                achievements.Add(new(Abilities.TwoHanded + "50", "Two-Handed Adept",
                    $"Reach ability level {50} for the {AbilityList.GetAbility(Abilities.TwoHanded)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.TwoHanded),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.TwoHanded], 50))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.TwoHanded, 50)
                });
                achievements.Add(new(Abilities.TwoHanded + "75", "Two-Handed Expert",
                    $"Reach ability level {75} for the {AbilityList.GetAbility(Abilities.TwoHanded)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.TwoHanded + "50"),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.TwoHanded], 75))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.TwoHanded, 75)
                });
                achievements.Add(new(Abilities.TwoHanded + "100", "Two-Handed Master",
                    $"Reach ability level {100} for the {AbilityList.GetAbility(Abilities.TwoHanded)!.Name} style",
                    Conditions.AchievementUnlockedCondition(Abilities.TwoHanded + "75"),
                    Conditions.HasLevelledAllAbilitiesCondition([Abilities.TwoHanded], 100))
                {
                    Reward = AchievementList.GetPerkForLeveledAchievement(Abilities.TwoHanded, 100)
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
                    (i > 0) ? ExpressionParser.ParseToFunction($"{oAllRanks[i - 1].Rank[..1]}oALL") : (e, w) => true,
                    Conditions.HasLevelledAllAbilitiesCondition(Abilities.Styles, level))
                    {
                        Reward = AchievementList.GetRewardForLeveledAchievement("oALL", level)
                    };
                    achievements.Add(achievement);
                }

                achievements.ForEach(a => contentPieces.Add(new AchievementContent(a)));

                return contentPieces;
            }
        }
	}
}
