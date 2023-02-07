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
            var lootTable = BuildLootTable(dungeon.Rewards, dungeon.Level);

            if (!lootTable.Any())
                return;

            // Select random reward
            double weightSum = lootTable.Sum(e => e.Value);
            double pointer = new Random().NextDouble() * weightSum;
            string selection = "";
            foreach (var reward in lootTable)
            {
                if (reward.Value > pointer)
                {
                    selection = reward.Key;
                    break;
                }
                pointer -= reward.Value;
            }

            Entity item = new ItemFactory().ExpandCode(selection) ?? throw new Exception($"Invalid item code: {selection}");
            int rarity = ItemFactory.GetRandomRarity(dungeon.Level, world.RarityMultiplier);
            ItemFactory.SetItemRarity(item, rarity);

            coordinator.AddEntity(item);
            coordinator.PostMessage(this, new ItemReceivedMessage(m_player!, item));
        }

        public static Dictionary<string, double> BuildBasicLootTable(int minLevel, int maxLevel)
        {
            HashSet<string> validIds = new();
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
                            validIds.Add(id.Code);
                        }
                    }
                }
            }

            Dictionary<string, double> result = new();
            foreach (var id in validIds)
            {
                result[id] = 1.0;
            }

            return result;
        }

        public static Dictionary<string, double> BuildLootTable(DungeonRewardsDescription rewardSettings, int lootLevel)
        {
            Dictionary<string, double> lootTable = new();
            if (rewardSettings.UseLeveledLoot)
                lootTable = BuildBasicLootTable(rewardSettings.MinDropLevel, lootLevel);
            rewardSettings.SpecialRewards.ForEach(r => 
            {
                lootTable.Add(r, 1.0);
            } );
            return lootTable;
        }
    }
}
