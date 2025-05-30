using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Properties;
using TheIdleScrolls_Core.Quests;
using TheIdleScrolls_Core.Resources;
using TheIdleScrolls_Core.Definitions;
using System.Xml.Linq;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class WarriorGettingStartedContentPack : IContentPack
	{
		public string Id => "CP_WarriorGettingStarted";

		public string Name => "Warrior Getting Started Content";

		public string Description => "The tutorial quest and content for the warrior (default mode)";

		public List<IContentPiece> ContentPieces => [
			new QuestContent(new GettingStartedQuest()),
			..Dungeons.Select(d => new DungeonContent(d)),
			..Achievements.Select(a => new AchievementContent(a)),
		];

		public const int LevelDenOfRats = 12;
		public const int LevelCrypt = 18;
		public const int LevelUberCrypt = 225;

		static List<DungeonDescription> Dungeons => [
			new()
			{
				Id = DungeonIds.DenOfRats,
				Name = Places.Dungeon_RatDen,
				Rarity = 0,
				AvailableLevels = DungeonList.UnlockedAtEqualWilderness(LevelDenOfRats),
				Description = Places.Dungeon_RatDen_Description,
				Floors = new()
				{
					new(2, 1.5, [ "MOB_RAT" ]),
					new(5, 3.2, [ "MOB_RAT" ]),
					new(5, 4.2, [ "MOB_BIGRAT" ]),
					new(1, 3.1, [ "BOSS_GIANTRAT" ])
				},
				LocalMobs = new()
				{
					new("MOB_RAT",       MobNames.MOB_RAT,       hP: 0.8, damage: 1.0),
					new("MOB_BIGRAT",    MobNames.MOB_BIGRAT,    hP: 1.5, damage: 0.8),
					new("BOSS_GIANTRAT", MobNames.BOSS_GIANTRAT, hP: 4.0, damage: 1.5),
				}
			},
			new()
			{
				Id = DungeonIds.Crypt,
				Name = Places.Dungeon_Crypt,
				Rarity = 0,
				AvailableLevels = DungeonList.UnlockedForEachWildernessLevel([LevelCrypt, LevelUberCrypt]),
				Description = Places.Dungeon_Crypt_Description,
				Floors = new()
				{
					new(2, 2.2, [ "MOB_ZOMBIE" ]),
					new(2, 2.0, [ "MOB_SKELETON" ]),
					new(3, 2.6, [ "MOB_ZOMBIE", "MOB_SKELETON" ]),
					new(2, 5.1, [ "MOB_ABOMINATION" ]),
					new(1, 3.1, [ "BOSS_NECROMANCER", "BOSS_UBERNECRO" ])
				},
				LocalMobs = new()
				{
					new("MOB_ZOMBIE", MobNames.MOB_ZOMBIE, hP: 1.4, damage: 0.8),
					new("MOB_SKELETON", MobNames.MOB_SKELETON, hP: 0.8, damage: 1.4),
					new("MOB_ABOMINATION", MobNames.MOB_ABOMINATION, hP: 4.0, damage: 1.0),
					new("BOSS_NECROMANCER", MobNames.BOSS_NECROMANCER, hP: 3.5, damage: 1.5) { CanSpawn = (zone) => zone.Level < LevelUberCrypt },
					new("BOSS_UBERNECRO", MobNames.BOSS_UBERNECRO, hP: 5.0, damage: 2.0) { CanSpawn = (zone) => zone.Level >= LevelUberCrypt }
				},
				Rewards = new() {
					DropLevelRange = LevelCrypt - 12, // Prevents weapons from dropping at level 18
                    SpecialRewards = [ DropRestrictions.MaterialT4 ] // T4 can drop from uber crypt
                }
			}
		];

		static List<Achievement> Achievements => [
			new($"DNG:{DungeonIds.Crypt}", 
				"Cryptkeeper", 
				$"Complete the {Places.Dungeon_Crypt}",
				Conditions.DungeonAvailableCondition(DungeonIds.Crypt),
				Conditions.DungeonCompletedCondition(DungeonIds.Crypt)
				),
			new(
				"FOUNDUBERCRYPT",
				"Archaeologist",
				$"Discover the {Places.Dungeon_Crypt}'s high level version",
				Conditions.DungeonLevelAvailableCondition(DungeonIds.Crypt, LevelUberCrypt),
				Conditions.DungeonLevelAvailableCondition(DungeonIds.Crypt, LevelUberCrypt)
				),
			new($"HC:{DungeonIds.Crypt}",
				"Attentive Dungeoneer",
				$"Complete the {Places.Dungeon_Crypt} without ever losing a fight",
				Conditions.DungeonAvailableCondition(DungeonIds.Crypt),
				ExpressionParser.ParseToFunction($"dng:{DungeonIds.Crypt} > 0 && Losses == 0"))
		];
	}
}
