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

        public string GetReport(World world)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Playtime: {TimeSpan.FromSeconds(Playtime).ToString(@"hh\:mm\:ss")}");
            sb.AppendLine($"Highest defeated zone: {HighestWildernessKill}");
            sb.AppendLine($"Enemies defeated: {Kills}");
            sb.AppendLine($"Fights lost: {Losses}");
            sb.AppendLine();
            sb.AppendLine($"Completed dungeons:");
            foreach (var dungeonTime in DungeonTimes)
            {
                var dungeon = world.AreaKingdom.GetDungeon(dungeonTime.Key);
                if (dungeon != null)
                {
                    var time = TimeSpan.FromSeconds(dungeonTime.Value);
                    sb.AppendLine($"    {dungeon.Name} (Time: {time.ToString(@"hh\:mm\:ss")})");
                }
            }
            return sb.ToString();
        }
    }
}
