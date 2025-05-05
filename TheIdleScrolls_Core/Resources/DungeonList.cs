using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

using TheIdleScrolls_Core.Properties;

namespace TheIdleScrolls_Core.Resources
{
    internal static class DungeonList
    {
		public const int LevelDenOfRats = 12;
		public const int LevelCrypt = 18;
		public const int LevelLighthouse = 20;
		public const int LevelTemple = 30;
		public const int LevelMercCamp = 40;
		public const int LevelCultistCastle = 50;
		public const int LevelLabyrinth = 60;
		public const int LevelReturnToLighthouse = 70;
		public const int LevelThreshold = 75;
		public const int LevelVoid = 80;
		public const int LevelVoidMax = 125;
		public const int LevelEndgame = 150;
		public const int LevelUberEndgame = 200;
		public const int LevelUberCrypt = 225;

		public static readonly List<string> VoidBosses = [ "BOSS_GHOSTCULTLEADER", 
                                                           "BOSS_GIANT", 
                                                           "BOSS_LEVIATHAN", 
                                                           "BOSS_MONSTROSITY", 
                                                           "BOSS_OBSERVER" ];

        public static DungeonDescription? GetDungeon(string dungeonId)
        {
            return s_dungeonDescriptions.Where(d => d.Id == dungeonId).FirstOrDefault();
        }

        static List<DungeonDescription> s_dungeonDescriptions = GenerateDungeons();

        public static List<DungeonDescription> GetAllDungeons() => s_dungeonDescriptions;

        static List<DungeonDescription> GenerateDungeons()
        {
            static Func<Entity, World, int[]> UnlockedAtEqualWilderness(int level)
            {
                return (e, w) => Utility.Conditions.HasClearedWildernessLevel(e, level) ? [level] : [];
            }
            static Func<Entity, World, int[]> UnlockedForEachWildernessLevel(int[] levels)
            {
                return (e, w) => levels.Where(l => Utility.Conditions.HasClearedWildernessLevel(e, l))
									   .ToArray();
			}
            static Func<Entity, World, int[]> UnlockedAfterDungeon(string dungeonId, int level)
            {
                return (e, w) => Utility.Conditions.HasCompletedDungeon(e, dungeonId) ? [level] : [];
            }
            static Func<Entity, World, int[]> UnlockedAfterDungeonAndWilderness(string dungeonId, int level)
            {
                return (e, w) => Utility.Conditions.HasCompletedDungeon(e, dungeonId) 
                                && Utility.Conditions.HasClearedWildernessLevel(e, level) ? [level] : [];
            }
            
            

            static bool HasMaxedVoid(Entity e)
            {
                return e.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeonLevels()
                    .Any(dl => dl.Dungeon == Definitions.DungeonIds.Void && dl.Level == LevelVoidMax) ?? false;
            }
            static int[] EndgameDungeonLevels(Entity e, World w)
            {
                if (!HasMaxedVoid(e))
                    return [];
                if (Utility.Conditions.HasCompletedDungeon(e, Definitions.DungeonIds.EndgameMagic)
                    && Utility.Conditions.HasCompletedDungeon(e, Definitions.DungeonIds.EndgamePyramid)
                    && Utility.Conditions.HasCompletedDungeon(e, Definitions.DungeonIds.EndgameAges))
                    return [LevelEndgame, LevelUberEndgame];
                return [LevelEndgame];
            }

            List<string> voidMobs = [ "MOB_FLAMETHROWER", "MOB_HORNEDIMP", "MOB_SPIKEDDEMON", "MOB_VOIDCRAWLER",
                                      "MOB_ANGRYCHICKEN", "MOB_CONSUMINGOOZE", "MOB_GLIMPSEA", "MOB_GLIMPSEB", "MOB_GLIMPSEC", 
                                      "MOB_GORGON", "MOB_MARINE", "MOB_SERAPH", "MOB_SHADOWBULL",
                                      "MOB_WENDIGO", "MOB_VENGEFULDUMMY" ];

            return new()
            {
                new()
                {
                    Id = Definitions.DungeonIds.DenOfRats,
                    Name = Places.Dungeon_RatDen,
                    Rarity = 0,
                    AvailableLevels = UnlockedAtEqualWilderness(LevelDenOfRats),
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
                    Id = Definitions.DungeonIds.Crypt,
                    Name = Places.Dungeon_Crypt,
                    Rarity = 0,
                    AvailableLevels = UnlockedForEachWildernessLevel([LevelCrypt, LevelUberCrypt]),
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
                    Rewards = new() { DropLevelRange = LevelCrypt - 12 } // Prevents weapons from dropping
                },
                new()
                {
                    Id = Definitions.DungeonIds.Lighthouse,
                    Name = Places.Dungeon_Lighthouse,
                    Rarity = 1,
                    AvailableLevels = UnlockedAtEqualWilderness(LevelLighthouse),
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
                    Id = Definitions.DungeonIds.Temple,
                    Name = Places.Dungeon_Temple,
                    Rarity = 1,
                    AvailableLevels = UnlockedAfterDungeonAndWilderness(Definitions.DungeonIds.Lighthouse, LevelTemple),
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
                    Id = Definitions.DungeonIds.MercenaryCamp,
                    Name = Places.Dungeon_MercenaryCamp,
                    Rarity = 0,
                    AvailableLevels = UnlockedAtEqualWilderness(LevelMercCamp),
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
                    Id = Definitions.DungeonIds.CultistCastle,
                    Name = Places.Dungeon_CultistCastle,
                    Rarity = 1,
                    AvailableLevels = UnlockedAfterDungeonAndWilderness(Definitions.DungeonIds.Temple, LevelCultistCastle),
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
                    Id = Definitions.DungeonIds.Labyrinth,
                    Name = Places.Dungeon_Labyrinth,
                    Rarity = 1,
                    AvailableLevels = UnlockedAfterDungeon(Definitions.DungeonIds.CultistCastle, LevelLabyrinth),
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
                    Id = Definitions.DungeonIds.ReturnToLighthouse,
                    Name = Places.Dungeon_ReturnToLighthouse,
                    Rarity = 1,
                    AvailableLevels = UnlockedAfterDungeonAndWilderness(Definitions.DungeonIds.Labyrinth, LevelReturnToLighthouse),
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
                    Id = Definitions.DungeonIds.Threshold,
                    Name = Places.Dungeon_Threshold,
                    Rarity = 1,
                    AvailableLevels = (e, w) => { // Threshold can only be completed once
                        return (Utility.Conditions.HasCompletedDungeon(e, Definitions.DungeonIds.ReturnToLighthouse)
                            && !Utility.Conditions.HasCompletedDungeon(e, Definitions.DungeonIds.Threshold))
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
                },
                new()
                {
                    Id = Definitions.DungeonIds.Void,
                    Name = Places.Dungeon_Void,
                    Rarity = 2,
                    AvailableLevels = (e, w) => { // More levels unlock with each completed one
                        if (!Utility.Conditions.HasCompletedDungeon(e, Definitions.DungeonIds.Threshold))
                            return [];
                        return e.GetComponent<PlayerProgressComponent>()!.Data
                            .GetClearedDungeonLevels()
                            .Where(dl => dl.Dungeon == Definitions.DungeonIds.Void)
                            .Select(dl => dl.Level + 5)
                            .Where(l => l <= LevelVoidMax)
                            .Prepend(LevelVoid)
                            .ToArray();
                    },
                    Description = Places.Dungeon_Void_Description,
                    Floors = new()
                    {
                        new(4, 4.3, [ "MOB_FLAMETHROWER", "MOB_HORNEDIMP", "MOB_SPIKEDDEMON", "MOB_VOIDCRAWLER" ], Places.Dng_Void_Floor0),
                        new(3, 3.0, [ "MOB_FLAMETHROWER", "MOB_HORNEDIMP", "MOB_SPIKEDDEMON", "MOB_VOIDCRAWLER" ], Places.Dng_Void_Floor1),
                        new(2, 4.3, [ "MOB_JUGGERNAUT", "MOB_TAURUS" ], Places.Dng_Void_Floor2),
                        new(4, 3.5, voidMobs, Places.Dng_Void_Floor3),
                        new(6, 5.0, voidMobs, Places.Dng_Void_Floor4),
                        new(1, 3.5, VoidBosses, Places.Dng_Void_Floor5)
                    },
                    LocalMobs = new()
                    {
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
                    },
                    Rewards = new() { SpecialRewards = [ Definitions.DropRestrictions.MaterialT4 ] }
                },
                new()
                {
                    Id = Definitions.DungeonIds.EndgameMagic,
                    Name = Places.Dungeon_EndgameMagic,
                    Rarity = 3,
                    AvailableLevels = EndgameDungeonLevels,
                    Description = Places.Dungeon_EndgameMagic_Description,
                    Floors = new()
                    {
                        new(4, 2.75, [ "MOB_SPIDER", "MOB_SPIDER2" ], "Forest"),
                        new(1, 1.5, [ "MOB_WILLOW" ], "Outer Walls"),
                        new(1, 1.0, [ "MOB_SECRETGATE" ], "Outer Walls"),
                        new(4, 2.7, [ "MOB_POISONGHOST", "MOB_SEWERSNAKE", "MOB_THREEHEADEDDOG" ], "Sewers"),
                        new(4, 2.25, [ "MOB_HUFFLE" ], "Earth Wing"),
                        new(3, 2.4, [ "MOB_RAVEN" ], "Air Wing"),
                        new(2, 1.65, [ "MOB_SNAKE" ], "Water Wing"),
                        new(2, 2.2, [ "MOB_GRYPHON" ], "Fire Wing"),
                        new(1, 4.0, [ "BOSS_SOREN" ], "Great Hall")
                    },
                    LocalMobs = new()
                    {
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
                    },
                    Rewards = new() { DropLevelRange = Definitions.Stats.DefaultDropLevelRange, 
                                      SpecialRewards = [ Definitions.DropRestrictions.MaterialT4 ] }
                },
                new()
                {
                    Id = Definitions.DungeonIds.EndgamePyramid,
                    Name = Places.Dungeon_EndgamePyramid,
                    Rarity = 3,
                    AvailableLevels = EndgameDungeonLevels,
                    Description = Places.Dungeon_EndgamePyramid_Description,
                    Floors = new()
                    {
                        new(8, 3.6, [ "MOB_TRASH1", "MOB_TRASH2", "MOB_TRASH3" ]),
                        new(2, 4.4, [ "MOB_OVERSOUL" ]),
                        new(6, 2.2, [ "MOB_TRASH1", "MOB_TRASH2", "MOB_TRASH3" ]),
                        new(3, 4.0, [ "MOB_TRIO" ]),
                        new(5, 1.55, [ "MOB_TRASH1", "MOB_TRASH2", "MOB_TRASH3" ]),
                        new(1, 4.0, [ "BOSS_RENKE" ], "Apex")
                    },
                    LocalMobs = new()
                    {
                        new("MOB_TRASH1", MobNames.MOB_TRASH1, hP: 1.0, damage: 1.5),
                        new("MOB_TRASH2", MobNames.MOB_TRASH2, hP: 1.0, damage: 1.5),
                        new("MOB_TRASH3", MobNames.MOB_TRASH3, hP: 1.0, damage: 1.5),
                        new("MOB_OVERSOUL", MobNames.MOB_OVERSOUL, hP: 5.5, damage: 1.5),
                        new("MOB_TRIO", MobNames.MOB_TRIO, hP: 4.0, damage: 1.5),
                        new("BOSS_RENKE", MobNames.BOSS_RENKE, hP: 10.0, damage: 2.0)
                    },
                    Rewards = new() { DropLevelRange = Definitions.Stats.DefaultDropLevelRange,
                                      SpecialRewards = [ Definitions.DropRestrictions.MaterialT4 ] }
                },
                new()
                {
                    Id = Definitions.DungeonIds.EndgameAges,
                    Name = Places.Dungeon_EndgameAges,
                    Rarity = 3,
                    AvailableLevels = EndgameDungeonLevels,
                    Description = Places.Dungeon_EndgameAges_Description,
                    Floors = new()
                    {
                        new(2, 1.1, [ "MOB_A-INF" ]),
                        new(3, 2.3, [ "MOB_1-INF", "MOB_1-CAV" ]),
                        new(5, 4.0, [ "MOB_2-INF", "MOB_2-CAV", "MOB_2-ART" ]),
                        new(7, 7.0, [ "MOB_3-INF", "MOB_3-CAV", "MOB_3-ART", "MOB_3-AIR" ]),
                        new(1, 4.0, [ "BOSS_HEINRICH" ], "Nexus of Ages")
                    },
                    LocalMobs = new()
                    {
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
                    },
                    Rewards = new() { DropLevelRange = Definitions.Stats.DefaultDropLevelRange,
                                      SpecialRewards = [ Definitions.DropRestrictions.MaterialT4 ] }
                }
            };
        }
    }
}
