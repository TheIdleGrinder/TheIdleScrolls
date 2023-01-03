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

        public void ValidConditionsWork(string condition, double result)
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
    }
}
