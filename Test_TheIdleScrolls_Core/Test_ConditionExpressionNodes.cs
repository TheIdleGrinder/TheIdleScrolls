using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Utility;
using TheIdleScrolls_Core.World;

namespace Test_TheIdleScrolls_Core
{
    internal class Test_ConditionExpressionNodes
    {
        [TestCase("5", 5.0)]
        [TestCase("5 > 4", 1.0)]
        [TestCase("4 > 4.0", 0.0)]
        [TestCase("3 < 4", 1.0)]
        [TestCase("5 >= 4 && 1 == 1", 1.0)]
        [TestCase("5 >= 4 && 1 != 1", 0.5)]
        [TestCase("3 <= 4 || 2 != 1", 1.0)]
        [TestCase("0.0 && 3 < 4 || 2 == 1", 0.5)]
        [TestCase("3 || 2", 1.0)]

        public void ConditionSyntaxWorks(string condition, double result)
        {
            Entity target = new();
            World world = new();

            IConditionExpressionNode? node = null;
            Assert.DoesNotThrow(() => node = ExpressionParser.Parse(condition));
            Assert.That(node, Is.Not.Null);

            double output = -1.0;
            Assert.DoesNotThrow(() => output = node.Evaluate(target, world));
            Assert.That(output, Is.EqualTo(result));
        }

        [TestCase("Level", 12.0)]
        [TestCase("Kills", 123.0)]
        [TestCase("Losses", 4.0)]
        [TestCase("Playtime", 56.78)]
        [TestCase("dng:A", 9.9)]
        [TestCase("dng:B", -1.0)]
        [TestCase("abl:LAR", 10.0)]
        [TestCase("abl:XXX", -1.0)]
        public void VariableNodesWork(string condition, double result)
        {
            Entity player = PlayerFactory.MakeNewPlayer("Test");
            var lvlComp = player.GetComponent<LevelComponent>();
            Assert.That(lvlComp, Is.Not.Null);
            lvlComp.Level = 12;
            var progData = player.GetComponent<PlayerProgressComponent>()?.Data;
            Assert.That(progData, Is.Not.Null);
            progData.Kills = 123;
            progData.Losses = 4;
            progData.Playtime = 56.78;
            progData.DungeonTimes["A"] = 9.9;

            World world = new();

            VariableNode node = new VariableNode(condition);
            double output = node.Evaluate(player, world);
            Assert.That(output, Is.EqualTo(result));
        }
    }
}
