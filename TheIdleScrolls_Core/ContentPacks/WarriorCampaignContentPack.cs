using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Properties;
using TheIdleScrolls_Core.Quests;
using TheIdleScrolls_Core.Resources;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class WarriorCampaignContentPack : IContentPack
	{
		public string Id => "CP_WarriorCampaign";

		public string Name => "Warrior Campaign Content";

		public string Description => "Pack containing the default campaign quest and related content";

		public List<IContentPiece> ContentPieces => [
			new QuestContent(new StoryQuest()),
			..Dungeons.Select(d => new DungeonContent(d)),
			..Achievements.Select(a => new AchievementContent(a)),
		];

		public const int LevelLighthouse = 20;
		public const int LevelTemple = 30;
		public const int LevelMercCamp = 40;
		public const int LevelCultistCastle = 50;
		public const int LevelLabyrinth = 60;
		public const int LevelReturnToLighthouse = 70;
		public const int LevelThreshold = 75;

		static readonly List<DungeonDescription> Dungeons = [
			new()
			{
				Id = DungeonIds.Lighthouse,
				Name = Places.Dungeon_Lighthouse,
				Rarity = 1,
				AvailableLevels = DungeonList.UnlockedAtEqualWilderness(LevelLighthouse),
				Description = Places.Dungeon_Lighthouse_Description,
				Floors = new()
				{
					new(2, 2.5, [ "MOB_CULTIST" ]),
					new(3, 3.3, [ "MOB_CULTIST", "MOB_DEMONSCOUT" ]),
					new(5, 5.0, [ "MOB_DEMONSCOUT" ]),
					new(1, 4.0, [ "MOB_LESSERDEMON" ])
				},
				LocalMobs = new()
				{
					new("MOB_CULTIST", MobNames.MOB_CULTIST, hP: 1.2, damage: 1.2),
					new("MOB_DEMONSCOUT", MobNames.MOB_DEMONSCOUT, hP: 1.0, damage: 1.4),
					new("MOB_LESSERDEMON", MobNames.MOB_LESSERDEMON, hP: 6.0, damage: 1.1)
				},
			},
			new()
			{
				Id = DungeonIds.Temple,
				Name = Places.Dungeon_Temple,
				Rarity = 1,
				AvailableLevels = DungeonList.UnlockedAfterDungeonAndWilderness(DungeonIds.Lighthouse, LevelTemple),
				Description = Places.Dungeon_Temple_Description,
				Floors = new()
				{
					new(3, 3.6, [ "MOB_CULTIST", "MOB_WARLOCK" ]),
					new(3, 3.2, [ "MOB_CULTIST", "MOB_WARLOCK" ]),
					new(3, 2.9, [ "MOB_CULTIST", "MOB_WARLOCK" ]),
					new(3, 2.7, [ "MOB_CULTIST", "MOB_WARLOCK" ]),
					new(3, 3.5, [ "BOSS_VOIDPRIEST" ])
				},
				LocalMobs = new()
				{
					new("MOB_CULTIST", MobNames.MOB_CULTIST, hP: 1.2, damage: 1.2),
					new("MOB_WARLOCK", MobNames.MOB_WARLOCK, hP: 1.0, damage: 1.4),
					new("BOSS_VOIDPRIEST", MobNames.BOSS_VOIDPRIEST, hP: 1.5, damage: 1.3)
				},
			},
			new()
			{
				Id = DungeonIds.MercenaryCamp,
				Name = Places.Dungeon_MercenaryCamp,
				Rarity = 0,
				AvailableLevels = DungeonList.UnlockedAtEqualWilderness(LevelMercCamp),
				Description = Places.Dungeon_MercenaryCamp_Description,
				Floors = new()
				{
					new(15, 9.0, [ "MOB_MERCENARY", "MOB_MERCENARY2" ]),
					new(1, 4.0, [ "BOSS_MERCENARY" ])
				},
				LocalMobs = new()
				{
					new("MOB_MERCENARY", MobNames.MOB_MERCENARY, hP: 1.0, damage: 1.0),
					new("MOB_MERCENARY2", MobNames.MOB_MERCENARY2, hP: 1.0, damage: 1.0),
					new("BOSS_MERCENARY", MobNames.BOSS_MERCENARY, hP: 6.0, damage: 1.33)
				},
			},
			new()
			{
				Id = DungeonIds.CultistCastle,
				Name = Places.Dungeon_CultistCastle,
				Rarity = 1,
				AvailableLevels = DungeonList.UnlockedAfterDungeonAndWilderness(DungeonIds.Temple, LevelCultistCastle),
				Description = Places.Dungeon_CultistCastle_Description,
				Floors = new()
				{
					new(4, 4.0, [ "MOB_FANATIC", "MOB_WARLOCK" ]),
					new(3, 2.7, [ "MOB_FANATIC", "MOB_WARLOCK" ]),
					new(5, 3.8, [ "MOB_FANATIC", "MOB_WARLOCK" ]),
					new(2, 5.0, [ "MOB_CULTKNIGHT" ])
				},
				LocalMobs = new()
				{
					new("MOB_FANATIC", MobNames.MOB_FANATIC, hP: 1.5, damage: 0.8),
					new("MOB_WARLOCK", MobNames.MOB_WARLOCK, hP: 0.8, damage: 1.5),
					new("MOB_CULTKNIGHT", MobNames.MOB_CULTKNIGHT, hP: 5.0, damage: 1.0)
				},
			},
			new()
			{
				Id = DungeonIds.Labyrinth,
				Name = Places.Dungeon_Labyrinth,
				Rarity = 1,
				AvailableLevels = DungeonList.UnlockedAfterDungeon(DungeonIds.CultistCastle, LevelLabyrinth),
				Description= Places.Dungeon_Labyrinth_Description,
				Floors = new()
				{
					new(1, 1.5, [ "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" ]),
					new(2, 2.9, [ "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" ]),
					new(1, 1.4, [ "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" ], "Floor 4"),
					new(1, 1.3, [ "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" ], "Floor 11"),
					new(2, 2.5, [ "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" ], "Floor 7"),
					new(1, 1.2, [ "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" ], "Floor 3"),
					new(1, 1.1, [ "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" ], "Floor 10"),
					new(3, 3.1, [ "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" ], "Floor 6"),
					new(1, 1.0, [ "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" ]),
					new(1, 4.0, [ "MOB_GREATERDEMON" ], "Inner Sanctum")
				},
				LocalMobs = new()
				{
					new("MOB_IMP", MobNames.MOB_IMP, hP: 1.15, damage: 1.6),
					new("MOB_LESSERDEMON", MobNames.MOB_LESSERDEMON, hP: 1.4, damage: 1.4), // Rescaled from JSON
                    new("MOB_VOIDCULTIST", MobNames.MOB_VOIDCULTIST, hP: 1.6, damage: 1.25),
					new("MOB_GREATERDEMON", MobNames.MOB_GREATERDEMON, hP: 6.0, damage: 1.5)
				},
			},
			new()
			{
				Id = DungeonIds.ReturnToLighthouse,
				Name = Places.Dungeon_ReturnToLighthouse,
				Rarity = 1,
				AvailableLevels = DungeonList.UnlockedAfterDungeonAndWilderness(DungeonIds.Labyrinth, LevelReturnToLighthouse),
				Description = Places.Dungeon_ReturnToLighthouse_Description,
				Floors = new()
				{
					new(3, 3.0, [ "MOB_VOIDCULTIST" ]),
					new(4, 3.5, [ "MOB_VOIDCULTIST", "MOB_IMPWARLOCK" ]),
					new(7, 5.4, [ "MOB_IMPWARLOCK" ]),
					new(1, 5.0, [ "BOSS_CULTLEADER" ])
				},
				LocalMobs = new()
				{
					new("MOB_VOIDCULTIST", MobNames.MOB_VOIDCULTIST, hP: 1.3, damage: 1.3),
					new("MOB_IMPWARLOCK", MobNames.MOB_IMPWARLOCK, hP: 1.8, damage: 1.0), // bit more life to make up for damage scaling
                    new("BOSS_CULTLEADER", MobNames.BOSS_CULTLEADER, hP: 6.0, damage: 2.0)
				},
			},
			new()
			{
				Id = DungeonIds.Threshold,
				Name = Places.Dungeon_Threshold,
				Rarity = 1,
				AvailableLevels = (e, w) => { // Threshold can only be completed once
                    return (Conditions.HasCompletedDungeon(e, DungeonIds.ReturnToLighthouse)
						&& !Conditions.HasCompletedDungeon(e, DungeonIds.Threshold))
						? [ LevelThreshold ] : [];
				},
				Description = Places.Dungeon_Threshold_Description,
				Floors = new()
				{
					new(25, 14.0, [ "MOB_IMPWARLOCK", "MOB_WINGEDDEMON", "MOB_BIGGERIMP" ], "")
				},
				LocalMobs = new()
				{
					new("MOB_IMPWARLOCK", MobNames.MOB_IMPWARLOCK, hP: 1.0, damage: 1.2),
					new("MOB_WINGEDDEMON", MobNames.MOB_WINGEDDEMON, hP: 1.2, damage: 1.0),
					new("MOB_BIGGERIMP", MobNames.MOB_BIGGERIMP, hP: 1.1, damage: 1.1),
				},
			}
		];

		static List<Achievement> Achievements
		{
			get
			{
				List<Achievement> achievements = [];

				(string Id, string Name, string Title, int PerkPoints)[] dungeons =
				[
					(DungeonIds.MercenaryCamp, Places.Dungeon_MercenaryCamp, "Sellsword Slayer", 0),
					(DungeonIds.Lighthouse, Places.Dungeon_Lighthouse, "I Shall Be Light", 1),
					(DungeonIds.Temple, Places.Dungeon_Temple, "I Shall Keep Faith", 1),
					(DungeonIds.CultistCastle, Places.Dungeon_CultistCastle, "I Shall Hone My Craft", 1),
					(DungeonIds.Labyrinth, Places.Dungeon_Labyrinth, "I Shall Have No Mercy", 1),
					(DungeonIds.ReturnToLighthouse, Places.Dungeon_ReturnToLighthouse, "I Shall Have No Fear", 1),
					(DungeonIds.Threshold, Places.Dungeon_Threshold, "I Shall Have No Remorse", 2),
				];
				foreach (var (id, name, title, points) in dungeons)
				{
					var achievement = new Achievement()
					{
						Id = $"DNG:{id}",
						Title = title,
						Description = $"Complete the {name}",
						Prerequisite = Conditions.DungeonAvailableCondition(id),
						Condition = Conditions.DungeonCompletedCondition(id)
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

				return achievements;
			}
		}
	}
}
