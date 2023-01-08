using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public enum TutorialStep { Start, Inventory, MobAttacks, Armor, Abilities, 
        Travel, Defeated, DungeonOpen, DungeonComplete, Finished, 
        Evasion, Unarmed, FlatCircle }

    public class PlayerProgressComponent : IComponent
    {
        public ProgressData Data { get; set; } = new();
    }

    public class ProgressData
    {
        public double Playtime { get; set; } = 0.0;
        public int HighestWildernessKill { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public HashSet<string> SeenItemFamilies { get; set; } = new HashSet<string>();
        public HashSet<string> SeenItemGenera { get; set; } = new HashSet<string>();
        public HashSet<TutorialStep> TutorialProgress { get; set; } = new();
        public Dictionary<string, double> DungeonTimes { get; set; } = new();

        public HashSet<string> GetClearedDungeons()
        {
            return DungeonTimes.Keys.ToHashSet();
        }
    }
}
