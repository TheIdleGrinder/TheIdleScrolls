using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Systems
{
    internal class LootSystem : AbstractSystem
    {
        const double WildDropChance = 0.05;
        Entity? m_player = null;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;

            foreach (var kill in coordinator.FetchMessagesByType<DeathMessage>())
            {
                
            }

            foreach (var dungeon in coordinator.FetchMessagesByType<DungeonCompletedMessage>())
            {
                GiveDungeonReward(world, coordinator);
            }
        }

        void GiveDungeonReward(World world, Coordinator coordinator)
        {
            var dungeon = world.AreaKingdom.GetDungeon(world.DungeonId) ?? throw new Exception($"Invalid dungeon id: {world.DungeonId}");
            var lootTable = BuildDungeonLootTable(dungeon.Rewards, dungeon.Level);

            // Select random reward
            string? selection = lootTable.GetRandomEntry();
            if (selection == null)
                return;

            Entity item = new ItemFactory().ExpandCode(selection) ?? throw new Exception($"Invalid item code: {selection}");
            int rarity = ItemFactory.GetRandomRarity(dungeon.Level, world.RarityMultiplier);
            ItemFactory.SetItemRarity(item, rarity);

            coordinator.AddEntity(item);
            coordinator.PostMessage(this, new ItemReceivedMessage(m_player!, item));
        }

        public static LootTable BuildBasicLootTable(int minLevel, int maxLevel)
        {
            LootTable table = new();
            foreach (var f in ItemFactory.ItemKingdom.Families)
            {
                for (int i = 0; i < f.Genera.Count; i++)
                {
                    var g = ItemFactory.ItemKingdom.GetGenusDescriptionByIdAndIndex(f.Id, i);
                    if (g == null)
                        throw new Exception($"Ítem family '{f.Id}' does not have {i + 1} genera");
                    if (g.DropLevel == 0) // Tutorial items cannot drop in the wild
                        continue;
                    var mats = g.ValidMaterials.Select(m => ItemFactory.ItemKingdom.GetMaterial(m)!);
                    foreach (var m in mats)
                    {
                        int dropLevel = g.DropLevel + m.MinimumLevel;
                        if (dropLevel >= minLevel && dropLevel <= maxLevel)
                        {
                            var id = new ItemIdentifier(f.Id, i, m.Id);
                            table.AddEntry(id.Code, 1.0); // CornerCut: Any idea for other weights?
                        }
                    }
                }
            }
            return table;
        }

        public static LootTable BuildDungeonLootTable(DungeonRewardsDescription rewardSettings, int lootLevel)
        {
            LootTable lootTable = new();
            if (rewardSettings.UseLeveledLoot)
                lootTable = BuildBasicLootTable(rewardSettings.MinDropLevel, lootLevel);
            rewardSettings.SpecialRewards.ForEach(r => 
            {
                lootTable.AddEntry(r, 1.0);
            } );
            return lootTable;
        }

        private record LootTableParameters(int ItemLevel, int MinDropLevel, int MinLootTableSize);

        public class LootTable
        {
            private Dictionary<string, double> m_table = new(); // code -> weight

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
        }
    }
}
