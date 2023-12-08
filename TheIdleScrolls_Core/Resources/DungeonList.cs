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
                    Floors = new()
                    {
                        new(2, 2.5, new() { "MOB_CULTIST" }),
                        new(3, 3.3, new() { "MOB_CULTIST", "MOB_IMP" }),
                        new(5, 5.0, new() { "MOB_IMP" }),
                        new(1, 4.0, new() { "MOB_LESSERDEMON" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_CULTIST", hP: 1.2, damage: 1.2),
                        new("MOB_IMP", hP: 1.0, damage: 1.4),
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
                    Condition = $"dng:{Definitions.DungeonIds.Lighthouse}",
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
                    Floors = new()
                    {
                        new(15, 1.0, new() { "MOB_MERCENARY" }),
                        new(1, 4.0, new() { "BOSS_MERCENARY" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_MERCENARY", hP: 1.0, damage: 1.0),
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
                    Condition = $"dng:{Definitions.DungeonIds.Temple}",
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
                    Floors = new()
                    {
                        new(1, 1.5, new() { "MOB_VOIDFIEND", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(2, 2.9, new() { "MOB_VOIDFIEND", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 1.4, new() { "MOB_VOIDFIEND", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 1.3, new() { "MOB_VOIDFIEND", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(2, 2.5, new() { "MOB_VOIDFIEND", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 1.2, new() { "MOB_VOIDFIEND", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 1.1, new() { "MOB_VOIDFIEND", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(3, 3.1, new() { "MOB_VOIDFIEND", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 1.0, new() { "MOB_VOIDFIEND", "MOB_LESSERDEMON", "MOB_VOIDCULTIST" }),
                        new(1, 4.0, new() { "BOSS_CULTLEADER" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_VOIDFIEND", hP: 2.0, damage: 1.0),
                        new("MOB_LESSERDEMON", hP: 1.42, damage: 1.42), // Rescaled from JSON
                        new("MOB_VOIDCULTIST", hP: 1.5, damage: 1.33),
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
                    Condition = $"dng:{Definitions.DungeonIds.Labyrinth}",
                    Floors = new()
                    {
                        new(3, 2.5, new() { "MOB_VOIDCULTIST" }),
                        new(4, 3.0, new() { "MOB_VOIDCULTIST", "MOB_VOIDFIEND" }),
                        new(7, 4.2, new() { "MOB_VOIDFIEND" }),
                        new(1, 3.5, new() { "MOB_GREATERDEMON" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_VOIDCULTIST", hP: 1.3, damage: 1.3),
                        new("MOB_VOIDFIEND", hP: 1.0, damage: 1.7),
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
                    Floors = new()
                    {
                        new(25, 20.0, new() { "MOB_VOIDFIEND", "MOB_DEMON1", "MOB_DEMON2" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_VOIDFIEND", hP: 1.0, damage: 1.2),
                        new("MOB_DEMON1", hP: 1.2, damage: 1.0),
                        new("MOB_DEMON2", hP: 1.1, damage: 1.1),
                    },
                    Rewards = new(61, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.EndgameTechnology,
                    Name = Properties.Places.Dungeon_EndgameTechnology,
                    Level = 150,
                    Rarity = 3,
                    Condition = "Wilderness >= 150",
                    Floors = new()
                    {
                        new(3, 3.0, new() { "MOB_SHARK" }),
                        new(4, 4.0, new() { "MOB_DEEPONE" }),
                        new(2, 2.0, new() { "MOB_ANCIENTMACHINE" }),
                        new(6, 4.5, new() { "MOB_ANCIENTMACHINE", "MOB_DEEPONE2" }),
                        new(1, 5.0, new() { "BOSS_RENKE" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_SHARK", hP: 3.0, damage: 3.0),
                        new("MOB_DEEPONE", hP: 3.0, damage: 3.5),
                        new("MOB_ANCIENTMACHINE", hP: 4.0, damage: 3.0),
                        new("MOB_DEEPONE2", hP: 3.0, damage: 4.0),
                        new("BOSS_RENKE", hP: 10.0, damage: 10.0)
                    },
                    Rewards = new(61, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.EndgameMagic,
                    Name = Properties.Places.Dungeon_EndgameMagic,
                    Level = 150,
                    Rarity = 3,
                    Condition = "Wilderness >= 150",
                    Floors = new()
                    {
                        new(5, 5.0, new() { "MOB_HUFFLE" }),
                        new(4, 4.0, new() { "MOB_RAVEN" }),
                        new(3, 3.0, new() { "MOB_SNAKE" }),
                        new(2, 2.0, new() { "MOB_GRYPHON" }),
                        new(1, 5.0, new() { "BOSS_SOREN" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_HUFFLE", hP: 3.0, damage: 3.0),
                        new("MOB_RAVEN", hP: 3.5, damage: 3.0),
                        new("MOB_SNAKE", hP: 4.0, damage: 3.5),
                        new("MOB_GRYPHON", hP: 4.5, damage: 3.8),
                        new("BOSS_SOREN", hP: 10.0, damage: 10.0)
                    },
                    Rewards = new(61, true, new())
                },
                new()
                {
                    Id = Definitions.DungeonIds.EndgameDistantLands,
                    Name = Properties.Places.Dungeon_EndgameDistant,
                    Level = 150,
                    Rarity = 3,
                    Condition = "Wilderness >= 150",
                    Floors = new()
                    {
                        new(3, 3.0, new() { "MOB_MARTIALARTIST" }),
                        new(3, 3.0, new() { "MOB_ASSASSIN" }),
                        new(3, 2.75, new() { "MOB_ASSASSIN", "MOB_TERRACOTTA" }),
                        new(3, 2.5, new() { "MOB_TERRACOTTA" }),
                        new(3, 3.0, new() { "MOB_TERRACOTTA2" }),
                        new(1, 5.0, new() { "BOSS_HEINRICH" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_MARTIALARTIST", hP: 3.0, damage: 3.0),
                        new("MOB_ASSASSIN", hP: 3.0, damage: 4.0),
                        new("MOB_TERRACOTTA", hP: 4.0, damage: 3.0),
                        new("MOB_TERRACOTTA2", hP: 5.5, damage: 3.0),
                        new("BOSS_HEINRICH", hP: 10.0, damage: 10.0)
                    },
                    Rewards = new(61, true, new())
                }
            };
        }
    }
}
