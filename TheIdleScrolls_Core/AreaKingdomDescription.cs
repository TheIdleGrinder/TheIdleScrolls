using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core
{
    public class AreaKingdomDescription
    {
        public List<DungeonDescription> Dungeons => DungeonList.GetAllDungeons();

        public List<MobDescription> GetLocalEnemies(string areaId)
        {
            foreach (var dungeon in Dungeons)
            {
                if (dungeon.Id == areaId)
                {
                    return dungeon.LocalMobs;
                }
            }
            return new();
        }

        public int GetDungeonFloorCount(string dungeonId)
        {
            return Dungeons.Find(d => d.Id == dungeonId)?.Floors?.Count ?? 0;
        }

        public DungeonDescription? GetDungeon(string dungeonId)
        {
            return Dungeons.Find(d => d.Id == dungeonId);
        }
    }
}
