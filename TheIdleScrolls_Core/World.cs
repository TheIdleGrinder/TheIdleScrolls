using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core
{
    public class World
    {
        public int AreaLevel = 1;
        public ZoneDescription Zone;

        public double XpMultiplier = 1.0;
        public double SpeedMultiplier = 1.0;

        public ItemKingdomDescription ItemKingdom = new();

        public AreaKingdomDescription AreaKingdowm = new();

        public Cooldown TimeLimit = new(10.0);

        public World()
        {
            Zone = new();
            TimeLimit.SingleShot = true;
        }
    }
}
