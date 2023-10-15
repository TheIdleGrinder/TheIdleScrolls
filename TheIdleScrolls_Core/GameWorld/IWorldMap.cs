using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    public interface IWorldMap : IZoneGenerator
    {
        public ZoneDescription? GetNextLocation(Location location);

        public ZoneDescription? GetPreviousLocation(Location location);
    }
}
