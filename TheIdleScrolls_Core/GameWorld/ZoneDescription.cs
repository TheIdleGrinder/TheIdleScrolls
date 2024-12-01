using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    /// <summary>
    /// Description of a generic zone (both wilderness and dungeon)
    /// </summary>
    public class ZoneDescription
    {
        public string Name { get; set; } = "??";
        public Biome Biome { get; set; } = Biome.Grassland; 
        public int Level { get; set; } = 0; // CornerCut: Use Level==0 to check for invalid zone
        public double TimeMultiplier { get; set; } = 1.0;
        public int MobCount { get; set; } = 1;
        public List<string> MobTypes { get; set; } = [];
        public List<string> SpecialDrops { get; set; } = [];
    }
}
