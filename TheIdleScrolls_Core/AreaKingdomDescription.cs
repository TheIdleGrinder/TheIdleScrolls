using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core
{
    public class AreaKingdomDescription
    {
        public List<DungeonDescription> Dungeons { get; set; } = new();
    }

    public class DungeonDescription
    {
        public string Name { get; set; } = "";
        public string Id { get; set; } = "";
        public int Level { get; set; } = 1;
        public List<FloorDescription> Zones { get; set; } = new();
        public List<MobDescription> Enemies { get; set; } = new();
    }

    public class FloorDescription
    {
        public int Enemies { get; set; } = 1;
        public double TimeMultiplier { get; set; } = 1.0;
        public List<string> EnemyTypes { get; set; } = new();
    }
}
