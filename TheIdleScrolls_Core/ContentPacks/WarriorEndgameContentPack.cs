using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Properties;
using TheIdleScrolls_Core.Quests;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class WarriorEndgameContentPack : IContentPack
	{
		public string Id => "CP_WarriorEndgame";

		public string Name => "Warrior Endgame Content";

		public string Description => "Pack containing the default endgame quest and related content";

		public const int LevelVoid = 80;
		public const int LevelVoidMax = 125;
		public const int LevelEndgame = 150;
		public const int LevelUberEndgame = 200;

		static bool HasMaxedVoid(Entity e)
		{
			return e.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeonLevels()
				.Any(dl => dl.Dungeon == DungeonIds.Void && dl.Level == LevelVoidMax) ?? false;
		}
		static int[] EndgameDungeonLevels(Entity e, World w)
		{
			if (!HasMaxedVoid(e))
				return [];
			if (Conditions.HasCompletedDungeon(e, DungeonIds.EndgameMagic)
				&& Conditions.HasCompletedDungeon(e, DungeonIds.EndgamePyramid)
				&& Conditions.HasCompletedDungeon(e, DungeonIds.EndgameAges))
				return [LevelEndgame, LevelUberEndgame];
			return [LevelEndgame];
		}

		readonly List<string> VoidMobs = [ "MOB_FLAMETHROWER", "MOB_HORNEDIMP", "MOB_SPIKEDDEMON", "MOB_VOIDCRAWLER",
										   "MOB_ANGRYCHICKEN", "MOB_CONSUMINGOOZE", "MOB_GLIMPSEA", "MOB_GLIMPSEB", "MOB_GLIMPSEC",
										   "MOB_GORGON", "MOB_MARINE", "MOB_SERAPH", "MOB_SHADOWBULL",
										   "MOB_WENDIGO", "MOB_VENGEFULDUMMY" ];

		public static readonly List<string> VoidBosses = [ "BOSS_GHOSTCULTLEADER",
														   "BOSS_GIANT",
														   "BOSS_LEVIATHAN",
														   "BOSS_MONSTROSITY",
														   "BOSS_OBSERVER" ];

		public List<IContentPiece> ContentPieces => [
			new QuestContent(new EndgameQuest()),
			new DungeonContent(new()
				{
					Id = DungeonIds.Void,
					Name = Places.Dungeon_Void,
					Rarity = 2,
					AvailableLevels = (e, w) => { // More levels unlock with each completed one
                        if (!Conditions.HasCompletedDungeon(e, DungeonIds.Threshold))
							return [];
						return e.GetComponent<PlayerProgressComponent>()!.Data
							.GetClearedDungeonLevels()
							.Where(dl => dl.Dungeon == DungeonIds.Void)
							.Select(dl => dl.Level + 5)
							.Where(l => l <= LevelVoidMax)
							.Prepend(LevelVoid)
							.ToArray();
					},
					Description = Places.Dungeon_Void_Description,
					Floors = [
						new(4, 4.3, [ "MOB_FLAMETHROWER", "MOB_HORNEDIMP", "MOB_SPIKEDDEMON", "MOB_VOIDCRAWLER" ], Places.Dng_Void_Floor0),
						new(3, 3.0, [ "MOB_FLAMETHROWER", "MOB_HORNEDIMP", "MOB_SPIKEDDEMON", "MOB_VOIDCRAWLER" ], Places.Dng_Void_Floor1),
						new(2, 4.3, [ "MOB_JUGGERNAUT", "MOB_TAURUS" ], Places.Dng_Void_Floor2),
						new(4, 3.5, VoidMobs, Places.Dng_Void_Floor3),
						new(6, 5.0, VoidMobs, Places.Dng_Void_Floor4),
						new(1, 3.5, VoidBosses, Places.Dng_Void_Floor5)
					],
					LocalMobs = [
						new("MOB_FLAMETHROWER", MobNames.MOB_FLAMETHROWER,     hP: 1.3, damage: 1.4),
						new("MOB_HORNEDIMP", MobNames.MOB_HORNEDIMP,        hP: 1.5, damage: 1.3),
						new("MOB_SPIKEDDEMON", MobNames.MOB_SPIKEDDEMON,      hP: 1.7, damage: 1.2),
						new("MOB_VOIDCRAWLER", MobNames.MOB_VOIDCRAWLER,      hP: 1.5, damage: 1.3),
						new("MOB_JUGGERNAUT", MobNames.MOB_JUGGERNAUT,       hP: 3.5, damage: 1.3),
						new("MOB_TAURUS", MobNames.MOB_TAURUS,           hP: 3.5, damage: 1.3),
						new("MOB_ANGRYCHICKEN", MobNames.MOB_ANGRYCHICKEN,     hP: 1.15, damage: 1.5),
						new("MOB_CONSUMINGOOZE", MobNames.MOB_CONSUMINGOOZE,    hP: 1.5, damage: 1.3),
						new("MOB_GLIMPSEA", MobNames.MOB_GLIMPSEA,         hP: 1.5, damage: 1.3),
						new("MOB_GLIMPSEB", MobNames.MOB_GLIMPSEB,         hP: 1.5, damage: 1.3),
						new("MOB_GLIMPSEC", MobNames.MOB_GLIMPSEC,         hP: 1.5, damage: 1.3),
						new("MOB_GORGON", MobNames.MOB_GORGON,           hP: 1.5, damage: 1.3),
						new("MOB_HYDRAMARINE", MobNames.MOB_HYDRAMARINE,      hP: 1.3, damage: 1.4),
						new("MOB_MARINE", MobNames.MOB_MARINE,           hP: 1.5, damage: 1.3),
						new("MOB_SERAPH", MobNames.MOB_SERAPH,           hP: 1.5, damage: 1.3),
						new("MOB_SHADOWBULL", MobNames.MOB_SHADOWBULL,       hP: 1.5, damage: 1.3),
						new("MOB_WENDIGO", MobNames.MOB_WENDIGO,          hP: 1.5, damage: 1.3),
						new("MOB_VENGEFULDUMMY", MobNames.MOB_VENGEFULDUMMY,    hP: 1.3, damage: 1.4),
						new("BOSS_GHOSTCULTLEADER", MobNames.BOSS_GHOSTCULTLEADER, hP: 6.0, damage: 1.5),
						new("BOSS_GIANT", MobNames.BOSS_GIANT,           hP: 6.0, damage: 1.5),
						new("BOSS_LEVIATHAN", MobNames.BOSS_LEVIATHAN,       hP: 6.0, damage: 1.5),
						new("BOSS_MONSTROSITY", MobNames.BOSS_MONSTROSITY,     hP: 6.0, damage: 1.5),
						new("BOSS_OBSERVER", MobNames.BOSS_OBSERVER,        hP: 6.0, damage: 1.5)
					],
					Rewards = new() { SpecialRewards = [ DropRestrictions.MaterialT4 ] }
				}),
			new DungeonContent(new()
				{
					Id = DungeonIds.EndgameMagic,
					Name = Places.Dungeon_EndgameMagic,
					Rarity = 3,
					AvailableLevels = EndgameDungeonLevels,
					Description = Places.Dungeon_EndgameMagic_Description,
					Floors = [
						new(4, 2.75, [ "MOB_SPIDER", "MOB_SPIDER2" ], "Forest"),
						new(1, 1.5, [ "MOB_WILLOW" ], "Outer Walls"),
						new(1, 1.0, [ "MOB_SECRETGATE" ], "Outer Walls"),
						new(4, 2.7, [ "MOB_POISONGHOST", "MOB_SEWERSNAKE", "MOB_THREEHEADEDDOG" ], "Sewers"),
						new(4, 2.25, [ "MOB_HUFFLE" ], "Earth Wing"),
						new(3, 2.4, [ "MOB_RAVEN" ], "Air Wing"),
						new(2, 1.65, [ "MOB_SNAKE" ], "Water Wing"),
						new(2, 2.2, [ "MOB_GRYPHON" ], "Fire Wing"),
						new(1, 4.0, [ "BOSS_SOREN" ], "Great Hall")
					],
					LocalMobs = [
						new("MOB_SPIDER", MobNames.MOB_SPIDER, hP: 1.5, damage: 1.5),
						new("MOB_SPIDER2", MobNames.MOB_SPIDER2, hP: 1.5, damage: 1.5),
						new("MOB_WILLOW", MobNames.MOB_WILLOW, hP: 3.5, damage: 1.5),
						new("MOB_SECRETGATE", MobNames.MOB_SECRETGATE, hP: 1.5, damage: 0.0),
						new("MOB_POISONGHOST", MobNames.MOB_POISONGHOST, hP: 1.7, damage: 1.5),
						new("MOB_SEWERSNAKE", MobNames.MOB_SEWERSNAKE, hP: 1.7, damage: 1.5),
						new("MOB_THREEHEADEDDOG", MobNames.MOB_THREEHEADEDDOG, hP: 1.7, damage: 1.5),
						new("MOB_HUFFLE", MobNames.MOB_HUFFLE, hP: 1.5, damage: 1.5),
						new("MOB_RAVEN", MobNames.MOB_RAVEN, hP: 2.3, damage: 1.5),
						new("MOB_SNAKE", MobNames.MOB_SNAKE, hP: 2.5, damage: 1.5),
						new("MOB_GRYPHON", MobNames.MOB_GRYPHON, hP: 3.5, damage: 1.5),
						new("BOSS_SOREN", MobNames.BOSS_SOREN, hP: 10.0, damage: 2.0)
					],
					Rewards = new() { DropLevelRange = Stats.DefaultDropLevelRange,
									  SpecialRewards = [ DropRestrictions.MaterialT4 ] }
				}),
			new DungeonContent(new()
				{
					Id = DungeonIds.EndgamePyramid,
					Name = Places.Dungeon_EndgamePyramid,
					Rarity = 3,
					AvailableLevels = EndgameDungeonLevels,
					Description = Places.Dungeon_EndgamePyramid_Description,
					Floors = [
						new(8, 3.6, [ "MOB_TRASH1", "MOB_TRASH2", "MOB_TRASH3" ]),
						new(2, 4.4, [ "MOB_OVERSOUL" ]),
						new(6, 2.2, [ "MOB_TRASH1", "MOB_TRASH2", "MOB_TRASH3" ]),
						new(3, 4.0, [ "MOB_TRIO" ]),
						new(5, 1.55, [ "MOB_TRASH1", "MOB_TRASH2", "MOB_TRASH3" ]),
						new(1, 4.0, [ "BOSS_RENKE" ], "Apex")
					],
					LocalMobs =	[
						new("MOB_TRASH1", MobNames.MOB_TRASH1, hP: 1.0, damage: 1.5),
						new("MOB_TRASH2", MobNames.MOB_TRASH2, hP: 1.0, damage: 1.5),
						new("MOB_TRASH3", MobNames.MOB_TRASH3, hP: 1.0, damage: 1.5),
						new("MOB_OVERSOUL", MobNames.MOB_OVERSOUL, hP: 5.5, damage: 1.5),
						new("MOB_TRIO", MobNames.MOB_TRIO, hP: 4.0, damage: 1.5),
						new("BOSS_RENKE", MobNames.BOSS_RENKE, hP: 10.0, damage: 2.0)
					],
					Rewards = new() { DropLevelRange = Stats.DefaultDropLevelRange,
									  SpecialRewards = [ DropRestrictions.MaterialT4 ] }
				}),
			new DungeonContent(new()
				{
					Id = DungeonIds.EndgameAges,
					Name = Places.Dungeon_EndgameAges,
					Rarity = 3,
					AvailableLevels = EndgameDungeonLevels,
					Description = Places.Dungeon_EndgameAges_Description,
					Floors = [
						new(2, 1.1, [ "MOB_A-INF" ]),
						new(3, 2.3, [ "MOB_1-INF", "MOB_1-CAV" ]),
						new(5, 4.0, [ "MOB_2-INF", "MOB_2-CAV", "MOB_2-ART" ]),
						new(7, 7.0, [ "MOB_3-INF", "MOB_3-CAV", "MOB_3-ART", "MOB_3-AIR" ]),
						new(1, 4.0, [ "BOSS_HEINRICH" ], "Nexus of Ages")
					],
					LocalMobs =	[
						new("MOB_A-INF", MobNames.MOB_A_INF, hP: 1.5, damage: 1.5),
						new("MOB_1-INF", MobNames.MOB_1_INF, hP: 1.9, damage: 1.5),
						new("MOB_1-CAV", MobNames.MOB_1_CAV, hP: 1.9, damage: 1.5),
						new("MOB_2-INF", MobNames.MOB_2_INF, hP: 2.3, damage: 1.5),
						new("MOB_2-CAV", MobNames.MOB_2_CAV, hP: 2.3, damage: 1.5),
						new("MOB_2-ART", MobNames.MOB_2_ART, hP: 2.3, damage: 1.5),
						new("MOB_3-INF", MobNames.MOB_3_INF, hP: 3.2, damage: 1.5),
						new("MOB_3-CAV", MobNames.MOB_3_CAV, hP: 3.2, damage: 1.5),
						new("MOB_3-ART", MobNames.MOB_3_ART, hP: 3.2, damage: 1.5),
						new("MOB_3-AIR", MobNames.MOB_3_AIR, hP: 3.2, damage: 1.5),
						new("BOSS_HEINRICH", MobNames.BOSS_HEINRICH, hP: 10.0, damage: 2.0)
					],
					Rewards = new() { DropLevelRange = Stats.DefaultDropLevelRange,
									  SpecialRewards = [ DropRestrictions.MaterialT4 ] }
				}),
			.. Achievements.Select(a => new AchievementContent(a))
		];

		static readonly List<Achievement> Achievements = [
			new("DNG:VOID",
				"Void Scout",
				$"Complete your first excursion to {Places.Dungeon_Void}",
				Conditions.DungeonAvailableCondition(DungeonIds.Void),
				Conditions.DungeonCompletedCondition(DungeonIds.Void)),
			new("DNG:VOID@100",
                "Void Traveller",
                $"Complete {Places.Dungeon_Void} at area level 100",
                Conditions.AchievementUnlockedCondition("DNG:VOID"),
                Conditions.DungeonLevelCompletedCondition(DungeonIds.Void, 100)),
			new("DNG:VOID@125",
                "Void Explorer",
                $"Complete {Places.Dungeon_Void} at area level 125",
                Conditions.AchievementUnlockedCondition("DNG:VOID@100"),
                Conditions.DungeonLevelCompletedCondition(DungeonIds.Void, 125)),
            new("DNG:ENDGAME",
                "Void Conqueror",
                $"Complete an endgame dungeon",
                Conditions.AchievementUnlockedCondition("DNG:VOID@125"),
                (e, w) => Conditions.HasCompletedDungeon(e, DungeonIds.EndgameAges) 
                            || Conditions.HasCompletedDungeon(e, DungeonIds.EndgameMagic)
                            || Conditions.HasCompletedDungeon(e, DungeonIds.EndgamePyramid)),
            new("DNG:UBERENDGAME",
                "Void Emperor",
                $"Complete an endgame dungeon at area level {DungeonLevels.LevelUberEndgame}",
                Conditions.DungeonLevelAvailableCondition(DungeonIds.EndgameAges, DungeonLevels.LevelUberEndgame),
                (e, w) => Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgameAges, DungeonLevels.LevelUberEndgame)
                            || Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgameMagic, DungeonLevels.LevelUberEndgame)
                            || Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgamePyramid, DungeonLevels.LevelUberEndgame)),

            new("VOIDBOSSES",
                "Void Duelist",
                $"Defeat all different bosses in {Places.Dungeon_Void}",
                Conditions.AchievementUnlockedCondition("DNG:VOID"),
                Conditions.MobsDefeatedCondition(VoidBosses)),

			new($"HC:Endgame",
				"Exalted Conqueror",
				"Complete the endgame dungeons without ever losing a fight",
				Conditions.AchievementUnlockedCondition($"HC:{DungeonIds.ReturnToLighthouse}"),
				ExpressionParser.ParseToFunction($"dng:{DungeonIds.EndgameMagic} > 0 && dng:{DungeonIds.EndgamePyramid} " +
					$"&& dng:{DungeonIds.EndgameAges} && Losses == 0")),
			new($"HC:UberEndgame",
                "Exalted Emperor",
                "Complete the endgame dungeons at area level 200 without ever losing a fight",
                Conditions.AchievementUnlockedCondition($"HC:Endgame"),
                (e, w) => Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgameAges, DungeonLevels.LevelUberEndgame)
                            && Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgameMagic, DungeonLevels.LevelUberEndgame)
                            && Conditions.HasCompletedDungeonLevel(e, DungeonIds.EndgamePyramid, DungeonLevels.LevelUberEndgame)
                            && !Conditions.HasLostFights(e))
		];
	}
}
