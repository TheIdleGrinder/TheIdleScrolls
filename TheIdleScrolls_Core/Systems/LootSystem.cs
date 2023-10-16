using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Systems
{
    public class LootSystem : AbstractSystem
    {
        const double WildDropChance = 0.025;
        const double FirstClearRarityBonus = 2.5;
        const int MinDropCutoff = 51;

        Entity? m_player = null;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;

            var locationComp = m_player.GetComponent<LocationComponent>();
            if (locationComp == null)
                return;

            foreach (var _ in coordinator.FetchMessagesByType<DeathMessage>())
            {
                double dropChance = WildDropChance * world.QuantityMultiplier;
                if (new Random().NextDouble() < dropChance)
                {
                    var zone = locationComp.GetCurrentZone(world.Map) ?? new();
                    int MinDropLevel = Math.Min(zone.Level - 20, MinDropCutoff);
                    LootTableParameters parameters = new(zone.Level, MinDropLevel, 0, 0.0); // 0 rarity => no "magic" items from normal mobs
                    GiveRandomLoot(parameters, coordinator);
                }
            }

            foreach (var dungeon in coordinator.FetchMessagesByType<DungeonCompletedMessage>())
            {
                GiveDungeonReward(dungeon.DungeonId, world, coordinator, dungeon.FirstCompletion);
            }
        }

        void GiveDungeonReward(string dungeonId, World world, Coordinator coordinator, bool firstClear)
        {
            double rarity = world.RarityMultiplier * (firstClear ? FirstClearRarityBonus : 1.0);
            var dungeon = world.AreaKingdom.GetDungeon(dungeonId) ?? throw new Exception($"Invalid dungeon id: {dungeonId}");
            LootTableParameters parameters = new(dungeon.Level, dungeon.Rewards.MinDropLevel, 0, rarity);
            GiveRandomLoot(parameters, coordinator);
        }

        void GiveRandomLoot(LootTableParameters lootParameters, Coordinator coordinator)
        {
            var lootTable = LootTable.Generate(lootParameters);

            // Select random reward
            string? selection = lootTable.GetRandomEntry();
            if (selection == null)
                return;

            Entity item = new ItemFactory().ExpandCode(selection) ?? throw new Exception($"Invalid item code: {selection}");

            coordinator.AddEntity(item);
            coordinator.PostMessage(this, new ItemReceivedMessage(m_player!, item));
        }
    }

    public record LootTableParameters(
        int ItemLevel, 
        int MinDropLevel, 
        int MinLootTableSize,   // Currently unused
        double RarityMultiplier);

    public class LootTable
    {
        private readonly Dictionary<string, double> m_table = new(); // code -> weight

        public int Count { get { return m_table.Count; } }

        public void AddEntry(string itemCode, double weight = 1.0)
        {
            m_table[itemCode] = weight;
        }

        public string? GetRandomEntry()
        {
            if (!m_table.Any())
                return null;
            double weightSum = m_table.Sum(e => e.Value);
            double pointer = new Random().NextDouble() * weightSum;
            foreach (var reward in m_table)
            {
                if (reward.Value > pointer)
                {
                    return reward.Key;
                }
                pointer -= reward.Value;
            }
            return null;
        }

        public static LootTable Generate(LootTableParameters parameters)
        {
            LootTable table = new();
            List<double> rarityWeights = ItemFactory.GetRarityWeights(parameters.ItemLevel, parameters.RarityMultiplier);
            
            foreach (var f in ItemFactory.ItemKingdom.Families)
            {
                for (int i = 0; i < f.Genera.Count; i++)
                {
                    var g = ItemFactory.ItemKingdom.GetGenusDescriptionByIdAndIndex(f.Id, i);
                    if (g == null)
                        throw new Exception($"Ítem family '{f.Id}' does not have {i + 1} genera");
                    if (g.DropLevel == 0) // Tutorial items cannot drop
                        continue;
                    var mats = g.ValidMaterials.Select(m => ItemFactory.ItemKingdom.GetMaterial(m)!);
                    foreach (var m in mats)
                    {
                        int dropLevel = g.DropLevel + m.MinimumLevel;
                        if (dropLevel >= parameters.MinDropLevel && dropLevel <= parameters.ItemLevel)
                        {
                            var id = new ItemIdentifier(f.Id, i, m.Id);
                            for (int r = 0; r < rarityWeights.Count; r++)
                            {
                                if (rarityWeights[r] == 0)
                                    continue;
                                ItemIdentifier rareId = new(id.Code) { RarityLevel = r };
                                table.AddEntry(rareId.Code, rarityWeights[r]);
                            }
                        }
                    }
                }
            }
            double sum = table.m_table.Values.Sum();
            foreach (var key in table.m_table.Keys)
            {
                table.m_table[key] = table.m_table[key] / sum;
            }
            return table;
        }
    }
}
