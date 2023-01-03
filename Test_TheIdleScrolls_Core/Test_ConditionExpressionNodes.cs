using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Achievements;

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

        [TestCase("Level", 1.0)]
        [TestCase("Kills", 0.0)]
        [TestCase("Losses", 0.0)]
        [TestCase("Playtime", 0.0)]
        [TestCase("dng:CRYPT", -1.0)]
        [TestCase("abl:LAR", 10.0)]
        [TestCase("abl:SBL", 10.0)]
        public void VariableNodesWork(string condition, double result)
        {
            Entity player = PlayerFactory.MakeNewPlayer("Test");
            World world = new();

            VariableNode node = new VariableNode(condition);
            double output = node.Evaluate(player, world);
            Assert.That(output, Is.EqualTo(result));
        }
    }
}
