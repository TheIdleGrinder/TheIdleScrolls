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
        [TestCase(1, 3)]
        [TestCase(1, 25)]
        [TestCase(10, 30)]
        public void ProducesCorrectLeveledTables(int minLevel, int maxLevel)
        {
            var rewards = new DungeonRewardsDescription()
            {
                MinDropLevel = minLevel,
                UseLeveledLoot = true,
                SpecialRewards = new()
            };

            List<string> expected = new();
            foreach (var item in ItemFactory.GetAllItemGenusCodes())
            {
                var genus = new ItemIdentifier(item).GetGenusDescription();
                if (genus.DropLevel >= minLevel && genus.DropLevel <= maxLevel)
                {
                    expected.Add(item);
                }
            }

            var lootTable = DungeonSystem.BuildBasicLootTable(rewards, maxLevel);
            
            Assert.That(lootTable.Count, Is.EqualTo(expected.Count));
            foreach (string loot in expected)
            {
                Assert.That(lootTable.ContainsKey(loot), Is.True);
            }
        }
    }
}
