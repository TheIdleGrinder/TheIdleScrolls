using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    public interface IWorldMap : IZoneGenerator
    {
        public Location? GetNextLocation(Location location);

        public Location? GetPreviousLocation(Location location);
    }
}
