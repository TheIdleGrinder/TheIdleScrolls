using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    public interface IZoneGenerator
    {
        public ZoneDescription? GetZone(Location location);

        public List<DungeonDescription> GetDungeons();

        public List<DungeonDescription> GetDungeonsAtLocation(Location location);

        public ZoneDescription? GetDungeonZone(string dungeonId, int dungeonLevel, int floor);
    }
}
