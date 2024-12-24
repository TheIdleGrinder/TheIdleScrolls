using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Systems
{
    public class LootSystem : AbstractSystem
    {
        const double WildDropChance = 0.04;
        const double FirstClearRarityBonus = 2.5;

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
                    // -17: dungeons are mostly at multiples of 10, so this excludes weapons at -20
                    // -9: only include the most recent tiers of items
                    int range = 20;
                    // 0.0 rarity => no improved items from normal mobs
                    LootTableParameters parameters = new(zone.Level, range, 0.0, zone.SpecialDrops);
                    GiveRandomLoot(parameters, coordinator);
                }
            }

            foreach (var dungeon in coordinator.FetchMessagesByType<DungeonCompletedMessage>())
            {
                GiveDungeonReward(dungeon.DungeonId, dungeon.DungeonLevel, world, coordinator, dungeon.FirstCompletion);
            }
        }

        void GiveDungeonReward(string dungeonId, int level, World world, Coordinator coordinator, bool firstClear)
        {
            double rarity = world.RarityMultiplier * (firstClear ? FirstClearRarityBonus : 1.0);
            var dungeon = world.AreaKingdom.GetDungeon(dungeonId) ?? throw new Exception($"Invalid dungeon id: {dungeonId}");
            // CornerCut: Scaling dungeons should have scaling reward levels, so use -1 as a shortcut for "drop highest available tiers"
            int levelRange = dungeon.Rewards.DropLevelRange;
            LootTableParameters parameters = new(level, levelRange, rarity, dungeon.Rewards.SpecialRewards);
            GiveRandomLoot(parameters, coordinator);
        }

        void GiveRandomLoot(LootTableParameters lootParameters, Coordinator coordinator)
        {
            var lootTable = LootTable.Generate(lootParameters);

            // Select random reward
            string? selection = lootTable.GetRandomEntry();
            if (selection == null)
                return;

            Entity item = ItemFactory.ExpandCode(selection) ?? throw new Exception($"Invalid item code: {selection}");

            coordinator.AddEntity(item);
            coordinator.PostMessage(this, new ItemReceivedMessage(m_player!, item));
        }
    }

    public record LootTableParameters(
        int ItemLevel, 
        int LevelRange,
        double RarityMultiplier,
        List<string> FulfilledRestrictions);

    public class LootTable
    {
        private static readonly List<ItemBlueprint> _blueprints = [];

        private readonly Dictionary<string, double> m_table = []; // code -> weight

        public int Count { get { return m_table.Count; } }

        public void AddEntry(string itemCode, double weight = 1.0)
        {
            m_table[itemCode] = weight;
        }

        public List<string> GetItemCodes()
        {
            return m_table.Keys.ToList();
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

        private static void GenerateAllBlueprints()
        {
            _blueprints.Clear();
            foreach (var f in ItemKingdom.Families)
            {
                for (int g = 0; g < f.Genera.Count; g++)
                {
                    var genus = ItemKingdom.GetGenusDescriptionByIdAndIndex(f.Id, g)
                        ?? throw new Exception($"Ítem family '{f.Id}' does not have {g + 1} genera");
                    var materials = genus.ValidMaterials.Select(m => ItemKingdom.GetMaterial(m)!);
                    foreach (var material in materials)
                    {
                        int dropLevel = material.MinimumLevel + genus.DropLevel;
                        if (dropLevel == 0)
                        {
                            // Tutorial items cannot drop
                            continue;
                        }
                        var bp = new ItemBlueprint(f.Id, g, material.Id);
                        _blueprints.Add(bp);
                    }
                }
            }
        }

        public static LootTable Generate(LootTableParameters parameters)
        {
            if (_blueprints.Count == 0)
            {
                GenerateAllBlueprints();
            }

            LootTable table = new();
            List<double> qualityWeights = ItemFactory.GetQualityWeights(parameters.ItemLevel, parameters.RarityMultiplier);

            // TODO:
            // 1. Filter out items that are too high level or have unfulfilled restrictions
            // 2. Find highest level item that can drop
            // 3. Calculate lower limit for item level
            // 4. Filter for items that are in range

            var validDrops = _blueprints.Where(b => b.GetDropLevel() <= parameters.ItemLevel
                                && b.GetDropRestrictions().All(r => parameters.FulfilledRestrictions.Contains(r)));

            int highestDropLevel = validDrops.Max(b => b.GetDropLevel());
            int minLevel = Math.Max(0, highestDropLevel - parameters.LevelRange);

            validDrops = validDrops.Where(b => b.GetDropLevel() >= minLevel);
            foreach (var item in validDrops)
            {
                for (int q = 0; q < qualityWeights.Count; q++)
                {
                    if (qualityWeights[q] == 0.0)
                        continue;
                    ItemBlueprint rareId = item with { Quality = q };
                    table.AddEntry(rareId.ToString(), qualityWeights[q]);
                }
            }

            double sum = table.m_table.Values.Sum();
            foreach (var key in table.m_table.Keys)
            {
                table.m_table[key] = table.m_table[key] / sum;
            }
            //Console.WriteLine($"Loot: L{minLevel}-{highestDropLevel} ({validDrops.Count()} candidates)");
            return table;
        }

        public static List<ItemBlueprint> GetAllBlueprints()
        {
            if (_blueprints.Count == 0)
            {
                GenerateAllBlueprints();
            }
            return [.. _blueprints];
        }
    }
}
