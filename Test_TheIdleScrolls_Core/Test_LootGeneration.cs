using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Systems;

namespace Test_TheIdleScrolls_Core
{
    internal class Test_LootGeneration
    {
        // Disabled because it does not use materials. TODO: Update
        //[TestCase(1, 3)]
        //[TestCase(1, 25)]
        //[TestCase(10, 30)]
        //public void ProducesCorrectLeveledTables(int minLevel, int maxLevel)
        //{
        //    var rewards = new DungeonRewardsDescription()
        //    {
        //        MinDropLevel = minLevel,
        //        UseLeveledLoot = true,
        //        SpecialRewards = new()
        //    };

        //    List<string> expected = new();
        //    foreach (var item in ItemFactory.GetAllItemGenusCodes())
        //    {
        //        var genus = new ItemIdentifier(item).GetGenusDescription();
        //        if (genus.DropLevel >= minLevel && genus.DropLevel <= maxLevel)
        //        {
        //            expected.Add(item);
        //        }
        //    }

        //    var lootTable = LootSystem.BuildBasicLootTable(rewards.MinDropLevel, maxLevel);

        //    Assert.That(lootTable.Count, Is.EqualTo(expected.Count));
        //    foreach (string loot in expected)
        //    {
        //        Assert.That(lootTable.ContainsKey(loot), Is.True);
        //    }
        //}

        [TestCase(1, 1.0)]
        [TestCase(50, 1.0)]
        [TestCase(100, 1.0)]
        [TestCase(150, 1.0)]
        [TestCase(1, 2.0)]
        [TestCase(50, 2.0)]
        [TestCase(100, 2.0)]
        [TestCase(150, 2.0)]
        [TestCase(50, 4.0)]
        [TestCase(250, 100.0)]
        [TestCase(250, 0.0)]
        public void ProducesCorrectRarityTable(int level, double rarityMulti)
        {
            var rarities = ItemFactory.GetRarityWeights(level, rarityMulti);

            Assert.That(rarities, Has.Count.EqualTo(ItemKingdom.Rarities.Count + 1));
            Assert.That(rarities.Sum(), Is.EqualTo(1.0).Within(0.0001));
            double remaining = 1.0;
            for (int i = rarities.Count - 1; i > 0; i--)
            {
                double weight = Math.Min(rarityMulti / ItemKingdom.Rarities[i - 1].InverseWeight, remaining);
                remaining -= weight;
                if (level >= ItemKingdom.Rarities[i - 1].MinLevel)
                    Assert.That(rarities[i], Is.EqualTo(weight).Within(0.0001));
                else
                    Assert.That(rarities[i], Is.EqualTo(0.0));
            }
        }

        [TestCase(70, 1.0)]
        public void ProducesCorrectLootTables(int level, double rarityMulti)
        {
            var table = LootTable.Generate(new(level, 9, rarityMulti, []));

            Assert.That(table, Is.Not.Null);
            Assert.That(table, Has.Count.Not.EqualTo(0));
        }
    }
}
