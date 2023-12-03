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
        public static List<DungeonDescription> GetAllDungeons()
        {
            return new()
            {
                new()
                {
                    Id = Definitions.DungeonIds.DenOfRats,
                    Level = 12,
                    Rarity = 0,
                    Floors = new()
                    {
                        new(2, 1.5, new() { "MOB_RAT" }),
                        new(5, 3.2, new() { "MOB_RAT" }),
                        new(5, 4.2, new() { "MOB_BIGRAT" }),
                        new(1, 3.1, new() { "MOB_GIANTRAT" })
                    },
                    LocalMobs = new()
                    {
                        new("MOB_RAT", hP: 0.8, damage: 1.0),
                        new("MOB_BIGRAT", hP: 1.5, damage: 0.8),
                        new("MOB_GIANTRAT", hP: 4.0, damage: 1.5),
                    },
                    Rewards = new(10, true, new())
                }
            };
        }
    }
}
