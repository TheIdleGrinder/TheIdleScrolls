using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Components
{
    public class LocationComponent : IComponent
    {
        public Location CurrentLocation { get; set; } = new();

        public string DungeonId { get; set; } = string.Empty;

        public int DungeonFloor { get; set; } = 0;

        // CornerCut: Keep track of remaining enemies here, used to check the dungeon progress of player characters
        public int RemainingEnemies { get; set; } = Int32.MaxValue;

        public bool InDungeon => DungeonId != string.Empty;

        public bool InOverworld => !InDungeon;

        public ZoneDescription? GetCurrentZone(WorldMap map)
        {
            if (InDungeon)
            {
                return map.GetDungeonZone(DungeonId, DungeonFloor);
            }
            else
            {
                return map.GetZone(CurrentLocation);
            }
        }

        public bool EnterDungeon(string dungeonId)
        {
            DungeonId = dungeonId;
            DungeonFloor = 0;
            return true; // Placeholder for future checks
        }

        public bool LeaveDungeon()
        {
            DungeonId = string.Empty;
            DungeonFloor = -1;
            return true;
        }

        public bool TravelToLocation(Location location)
        {
            CurrentLocation = location;
            return LeaveDungeon();
        }
    }
}
