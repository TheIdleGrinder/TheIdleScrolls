using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Resources
{
    internal static class DungeonList
    {
        public static DungeonDescription? GetDungeon(string dungeonId)
        {
            return s_dungeonDescriptions.Where(d => d.Id == dungeonId).FirstOrDefault();
        }

        static List<DungeonDescription> s_dungeonDescriptions = GenerateDungeons();

        public static List<DungeonDescription> GetAllDungeons() => s_dungeonDescriptions;

        static List<DungeonDescription> GenerateDungeons()
        {
            return new()
            {
                new()
                {
                    Id = Definitions.DungeonIds.DenOfRats,
                    Name = Properties.Places.Dungeon_RatDen,
                    Level = 12,
                    Rarity = 0,
                    Condition = "",
                    Description = Properties.Places.Dungeon_RatDen_Description,
                    Floors = new()
                    {
                        new(2, 1.5, new() { "MOB_RAT" }),
                        new(5, 3.2, new() { "MOB_RAT" }),
                        new(5, 4.2, new() { "MOB_BIGRAT" }),
                        new(1, 3.1, new() { "BOSS_GIANTRAT" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_RAT", hP: 0.8, damage: 1.0),
                        new("MOB_BIGRAT", hP: 1.5, damage: 0.8),
                        new("BOSS_GIANTRAT", hP: 4.0, damage: 1.5),
                    },
                    Rewards = new(10, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.Crypt,
                    Name = Properties.Places.Dungeon_Crypt,
                    Level = 18,
                    Rarity = 0,
                    Condition = "",
                    Description = Properties.Places.Dungeon_Crypt_Description,
                    Floors = new()
                    {
                        new(2, 2.2, new() { "MOB_ZOMBIE" }),
                        new(2, 2.0, new() { "MOB_SKELETON" }),
                        new(3, 2.6, new() { "MOB_ZOMBIE", "MOB_SKELETON" }),
                        new(2, 5.1, new() { "MOB_ABOMINATION" }),
                        new(1, 3.1, new() { "BOSS_NECROMANCER" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_ZOMBIE", hP: 1.4, damage: 0.8),
                        new("MOB_SKELETON", hP: 0.8, damage: 1.4),
                        new("MOB_ABOMINATION", hP: 4.0, damage: 1.0),
                        new("BOSS_NECROMANCER", hP: 3.5, damage: 1.5)
                    },
                    Rewards = new(13, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.Lighthouse,
                    Name = Properties.Places.Dungeon_Lighthouse,
                    Level = 20,
                    Rarity = 1,
                    Condition = "Wilderness >= 20",
                    Description = Properties.Places.Dungeon_Lighthouse_Description,
                    Floors = new()
                    {
                        new(2, 2.5, new() { "MOB_CULTIST" }),
                        new(3, 3.3, new() { "MOB_CULTIST", "MOB_DEMONSCOUT" }),
                        new(5, 5.0, new() { "MOB_DEMONSCOUT" }),
                        new(1, 4.0, new() { "MOB_LESSERDEMON" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_CULTIST", hP: 1.2, damage: 1.2),
                        new("MOB_DEMONSCOUT", hP: 1.0, damage: 1.4),
                        new("MOB_LESSERDEMON", hP: 6.0, damage: 1.1)
                    },
                    Rewards = new(15, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.Temple,
                    Name = Properties.Places.Dungeon_Temple,
                    Level = 30,
                    Rarity = 1,
                    Condition = $"dng:{Definitions.DungeonIds.Lighthouse} && Wilderness >= 30",
                    Description = Properties.Places.Dungeon_Temple_Description,
                    Floors = new()
                    {
                        new(3, 3.6, new() { "MOB_CULTIST", "MOB_WARLOCK" }),
                        new(3, 3.2, new() { "MOB_CULTIST", "MOB_WARLOCK" }),
                        new(3, 2.9, new() { "MOB_CULTIST", "MOB_WARLOCK" }),
                        new(3, 2.7, new() { "MOB_CULTIST", "MOB_WARLOCK" }),
                        new(3, 3.5, new() { "BOSS_VOIDPRIEST" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_CULTIST", hP: 1.2, damage: 1.2),
                        new("MOB_WARLOCK", hP: 1.0, damage: 1.4),
                        new("BOSS_VOIDPRIEST", hP: 1.5, damage: 1.3)
                    },
                    Rewards = new(21, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.MercenaryCamp,
                    Name = Properties.Places.Dungeon_MercenaryCamp,
                    Level = 40,
                    Rarity = 0,
                    Condition = "Wilderness >= 40",
                    Description = Properties.Places.Dungeon_MercenaryCamp_Description,
                    Floors = new()
                    {
                        new(15, 9.0, new() { "MOB_MERCENARY", "MOB_MERCENARY2" }),
                        new(1, 4.0, new() { "BOSS_MERCENARY" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_MERCENARY", hP: 1.0, damage: 1.0),
                        new("MOB_MERCENARY2", hP: 1.0, damage: 1.0),
                        new("BOSS_MERCENARY", hP: 6.0, damage: 1.33)
                    },
                    Rewards = new(31, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.CultistCastle,
                    Name = Properties.Places.Dungeon_CultistCastle,
                    Level = 50,
                    Rarity = 1,
                    Condition = $"dng:{Definitions.DungeonIds.Temple} && Wilderness >= 50",
                    Description = Properties.Places.Dungeon_CultistCastle_Description,
                    Floors = new()
                    {
                        new(4, 4.0, new() { "MOB_FANATIC", "MOB_WARLOCK" }),
                        new(3, 2.7, new() { "MOB_FANATIC", "MOB_WARLOCK" }),
                        new(5, 3.8, new() { "MOB_FANATIC", "MOB_WARLOCK" }),
                        new(2, 5.0, new() { "MOB_CULTKNIGHT" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_FANATIC", hP: 1.5, damage: 0.8),
                        new("MOB_WARLOCK", hP: 0.8, damage: 1.5),
                        new("MOB_CULTKNIGHT", hP: 5.0, damage: 1.0)
                    },
                    Rewards = new(41, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.Labyrinth,
                    Name = Properties.Places.Dungeon_Labyrinth,
                    Level = 60,
                    Rarity = 1,
                    Condition = $"dng:{Definitions.DungeonIds.CultistCastle}",
                    Description= Properties.Places.Dungeon_Labyrinth_Description,
                    Floors = new()
                    {
                        new(1, 1.5, new() { "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(2, 2.9, new() { "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 1.4, new() { "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 1.3, new() { "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(2, 2.5, new() { "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 1.2, new() { "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 1.1, new() { "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(3, 3.1, new() { "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 1.0, new() { "MOB_IMP", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 4.0, new() { "BOSS_CULTLEADER" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_IMP", hP: 1.15, damage: 1.6),
                        new("MOB_LESSERDEMON", hP: 1.4, damage: 1.4), // Rescaled from JSON
                        new("MOB_VOIDCULTIST", hP: 1.6, damage: 1.25),
                        new("BOSS_CULTLEADER", hP: 6.0, damage: 1.5)
                    },
                    Rewards = new(51, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.ReturnToLighthouse,
                    Name = Properties.Places.Dungeon_ReturnToLighthouse,
                    Level = 70,
                    Rarity = 1,
                    Condition = $"dng:{Definitions.DungeonIds.Labyrinth} && Wilderness >= 70",
                    Description = Properties.Places.Dungeon_ReturnToLighthouse_Description,
                    Floors = new()
                    {
                        new(3, 3.0, new() { "MOB_VOIDCULTIST" }),
                        new(4, 3.5, new() { "MOB_VOIDCULTIST", "MOB_IMPWARLOCK" }),
                        new(7, 5.4, new() { "MOB_IMPWARLOCK" }),
                        new(1, 5.0, new() { "MOB_GREATERDEMON" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_VOIDCULTIST", hP: 1.3, damage: 1.3),
                        new("MOB_IMPWARLOCK", hP: 1.8, damage: 1.0), // bit more life to make up for damage scaling
                        new("MOB_GREATERDEMON", hP: 10.0, damage: 1.2)
                    },
                    Rewards = new(61, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.Threshold,
                    Name = Properties.Places.Dungeon_Threshold,
                    Level = 75,
                    Rarity = 1,
                    Condition = $"dng:{Definitions.DungeonIds.ReturnToLighthouse}",
                    Description = Properties.Places.Dungeon_Threshold_Description,
                    Floors = new()
                    {
                        new(25, 17.0, new() { "MOB_IMPWARLOCK", "MOB_WINGEDDEMON", "MOB_BIGGERIMP" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_IMPWARLOCK", hP: 1.0, damage: 1.2),
                        new("MOB_WINGEDDEMON", hP: 1.2, damage: 1.0),
                        new("MOB_BIGGERIMP", hP: 1.1, damage: 1.1),
                    },
                    Rewards = new(61, true, new())
                },
                //new()
                //{
                //    Id = Definitions.DungeonIds.EndgameTechnology,
                //    Name = Properties.Places.Dungeon_EndgameTechnology,
                //    Level = 150,
                //    Rarity = 3,
                //    Condition = $"Wilderness >= 150 || dng:{Definitions.DungeonIds.Threshold}",
                //    Description = Properties.Places.Dungeon_EndgameTechnology_Description,
                //    Floors = new()
                //    {
                //        new(3, 3.0, new() { "MOB_SHARK" }),
                //        new(4, 4.0, new() { "MOB_DEEPONE" }),
                //        new(2, 2.0, new() { "MOB_ANCIENTMACHINE" }),
                //        new(5, 4.5, new() { "MOB_ANCIENTMACHINE", "MOB_DEEPONE2" }),
                //        new(1, 5.2, new() { "BOSS_RENKE" })
                //    },
                //    LocalMobs = new()
                //    {
                //        new("MOB_SHARK", hP: 2.0, damage: 1.7),
                //        new("MOB_DEEPONE", hP: 2.3, damage: 1.7),
                //        new("MOB_ANCIENTMACHINE", hP: 2.5, damage: 1.7),
                //        new("MOB_DEEPONE2", hP: 2.0, damage: 2.125),
                //        new("BOSS_RENKE", hP: 13.0, damage: 2.0)
                //    },
                //    Rewards = new(61, true, new())
                //},
                new()
                {
                    Id = Definitions.DungeonIds.EndgameMagic,
                    Name = Properties.Places.Dungeon_EndgameMagic,
                    Level = 150,
                    Rarity = 3,
                    Condition = $"Wilderness >= 150 || dng:{Definitions.DungeonIds.Threshold}",
                    Description = Properties.Places.Dungeon_EndgameMagic_Description,
                    Floors = new()
                    {
                        new(5, 4.7, new() { "MOB_HUFFLE" }),
                        new(4, 4.0, new() { "MOB_RAVEN" }),
                        new(3, 3.0, new() { "MOB_SNAKE" }),
                        new(2, 2.3, new() { "MOB_GRYPHON" }),
                        new(1, 5.2, new() { "BOSS_SOREN" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_HUFFLE", hP: 2.05, damage: 1.5),
                        new("MOB_RAVEN", hP: 2.6, damage: 1.5),
                        new("MOB_SNAKE", hP: 2.9, damage: 1.5),
                        new("MOB_GRYPHON", hP: 3.65, damage: 1.5),
                        new("BOSS_SOREN", hP: 13.0, damage: 2.0)
                    },
                    Rewards = new(61, true, new())
                },
                //new()
                //{
                //    Id = Definitions.DungeonIds.EndgameDistantLands,
                //    Name = Properties.Places.Dungeon_EndgameDistant,
                //    Level = 150,
                //    Rarity = 3,
                //    Condition = $"Wilderness >= 150 || dng:{Definitions.DungeonIds.Threshold}",
                //    Description = Properties.Places.Dungeon_EndgameDistant_Description,
                //    Floors = new()
                //    {
                //        new(3, 3.0, new() { "MOB_MARTIALARTIST" }),
                //        new(3, 3.0, new() { "MOB_ASSASSIN" }),
                //        new(3, 2.75, new() { "MOB_ASSASSIN", "MOB_TERRACOTTA" }),
                //        new(3, 2.6, new() { "MOB_TERRACOTTA" }),
                //        new(3, 3.0, new() { "MOB_TERRACOTTA2" }),
                //        new(1, 5.2, new() { "BOSS_HEINRICH" })
                //    },
                //    LocalMobs = new()
                //    {
                //        new("MOB_MARTIALARTIST", hP: 2.0, damage: 1.7),
                //        new("MOB_ASSASSIN", hP: 1.7, damage: 2.3),
                //        new("MOB_TERRACOTTA", hP: 2.3, damage: 1.7),
                //        new("MOB_TERRACOTTA2", hP: 3.2, damage: 1.5),
                //        new("BOSS_HEINRICH", hP: 13.0, damage: 2.0)
                //    },
                //    Rewards = new(61, true, new())
                //},
                new()
                {
                    Id = Definitions.DungeonIds.EndgamePyramid,
                    Name = Properties.Places.Dungeon_EndgamePyramid,
                    Level = 150,
                    Rarity = 3,
                    Condition = $"Wilderness >= 150 || dng:{Definitions.DungeonIds.Threshold}",
                    Description = Properties.Places.Dungeon_EndgamePyramid_Description,
                    Floors = new()
                    {
                        new(5, 2.3, new() { "MOB_TRASH1", "MOB_TRASH2", "MOB_TRASH3" }),
                        new(2, 4.1, new() { "MOB_OVERSOUL" }),
                        new(6, 2.2, new() { "MOB_TRASH1", "MOB_TRASH2", "MOB_TRASH3" }),
                        new(3, 3.6, new() { "MOB_TRIO" }),
                        new(7, 2.2, new() { "MOB_TRASH1", "MOB_TRASH2", "MOB_TRASH3" }),
                        new(1, 5.2, new() { "BOSS_RENKE" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_TRASH1", hP: 1.0, damage: 1.5),
                        new("MOB_TRASH2", hP: 1.0, damage: 1.5),
                        new("MOB_TRASH3", hP: 1.0, damage: 1.5),
                        new("MOB_OVERSOUL", hP: 5.0, damage: 1.5),
                        new("MOB_TRIO", hP: 3.5, damage: 1.5),
                        new("BOSS_RENKE", hP: 13.0, damage: 2.0)
                    },
                    Rewards = new(61, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.EndgameAges,
                    Name = Properties.Places.Dungeon_EndgameAges,
                    Level = 150,
                    Rarity = 3,
                    Condition = $"Wilderness >= 150 || dng:{Definitions.DungeonIds.Threshold}",
                    Description = Properties.Places.Dungeon_EndgameAges_Description,
                    Floors = new()
                    {
                        new(2, 1.1, new() { "MOB_A-INF" }),
                        new(3, 2.3, new() { "MOB_1-INF", "MOB_1-CAV" }),
                        new(5, 4.0, new() { "MOB_2-INF", "MOB_2-CAV", "MOB_2-ART" }),
                        new(7, 7.0, new() { "MOB_3-INF", "MOB_3-CAV", "MOB_3-ART", "MOB_3-AIR" }),
                        new(1, 5.2, new() { "BOSS_HEINRICH" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_A-INF", hP: 1.5, damage: 1.5),
                        new("MOB_1-INF", hP: 1.9, damage: 1.5),
                        new("MOB_1-CAV", hP: 1.9, damage: 1.5),
                        new("MOB_2-INF", hP: 2.3, damage: 1.5),
                        new("MOB_2-CAV", hP: 2.3, damage: 1.5),
                        new("MOB_2-ART", hP: 2.3, damage: 1.5),
                        new("MOB_3-INF", hP: 3.2, damage: 1.5),
                        new("MOB_3-CAV", hP: 3.2, damage: 1.5),
                        new("MOB_3-ART", hP: 3.2, damage: 1.5),
                        new("MOB_3-AIR", hP: 3.2, damage: 1.5),
                        new("BOSS_HEINRICH", hP: 13.0, damage: 2.0)
                    },
                    Rewards = new(61, true, new())
                }
            };
        }
    }
}
