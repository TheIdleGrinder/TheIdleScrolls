using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Components
{
    public enum TutorialStep 
    { 
        Start, Inventory, MobAttacks, Armor, Abilities, 
        Travel, Defeated, DungeonOpen, DungeonComplete, Finished, 
        Evasion, Unarmed, FlatCircle, Selling, Crafting,
        ItemFound, Perks, Bounties
    }

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
        public HashSet<string> SeenItemFamilies { get; set; } = [];
        public HashSet<string> SeenItemGenera { get; set; } = [];
        public HashSet<TutorialStep> TutorialProgress { get; set; } = [];
        public Dictionary<string, Dictionary<int, double>> DungeonTimes { get; set; } = [];
        public Dictionary<string, Dictionary<int, int>> DungeonCompletions { get; set; } = [];
        public int MaxCoins { get; set; } = 0;
        public int TotalCoins { get; set; } = 0;
        public int CoinsSpentOnCrafting { get; set; } = 0;
        public int BestRefine { get; set;} = 0;
        public int BestG0Refine { get; set;} = 0;
        public Dictionary<string, int> DefeatedMobs { get; set; } = [];
        public Dictionary<string, int> ConditionalKills { get; set; } = [];

        public HashSet<string> GetClearedDungeons()
        {
            return DungeonTimes.Keys.ToHashSet();
        }

        public List<(string Dungeon, int Level)> GetClearedDungeonLevels()
        {
            return DungeonTimes.SelectMany(d => d.Value.Select(l => (d.Key, l.Key))).ToList();
        }

        public string GetReport(World world)
        {
            StringBuilder sb = new();
            sb.AppendLine($"Playtime: {TimeSpan.FromSeconds(Playtime).ToString(@"hh\:mm\:ss")}");
            sb.AppendLine($"Highest defeated zone: {HighestWildernessKill}");
            sb.AppendLine($"Enemies defeated: {Kills}");
            sb.AppendLine($"Fights lost: {Losses}");
            if (TotalCoins > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"Total Coins earned: {TotalCoins}");
                sb.AppendLine($"Most coins saved: {MaxCoins}");
            }
            if (CoinsSpentOnCrafting > 0)
            {
                sb.AppendLine($"Coins spent on crafting: {CoinsSpentOnCrafting}");
                sb.AppendLine($"Highest quality from refining: +{BestRefine}");
            }
            sb.AppendLine();
            if (DungeonTimes.Any())
                sb.AppendLine($"Completed dungeons:");
            foreach (var dungeonTime in DungeonTimes)
            {
                var dungeon = world.AreaKingdom.GetDungeon(dungeonTime.Key);
                if (dungeon != null)
                {
                    foreach (var dungeonLevelTime in dungeonTime.Value)
                    {
                        var time = TimeSpan.FromSeconds(dungeonLevelTime.Value);
                        string name = dungeon.Name + (dungeonTime.Value.Count > 1 ? $"[{dungeonLevelTime.Key}]" : "");
                        sb.AppendLine($"    {name} (Time: {time.ToString(@"hh\:mm\:ss")})");
                    }
                }
            }
            return sb.ToString();
        }
    }
}
