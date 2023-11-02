using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    public interface IMapProgressPath
    {
        Location? NextLocation(Location location);
        Location? PreviousLocation(Location location);
        int? LocationLevel(Location location);
    }
}
