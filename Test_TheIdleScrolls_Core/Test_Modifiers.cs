using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Modifiers;

namespace Test_TheIdleScrolls_Core
{
    public class Test_Modifiers
    {
        [TestCase("", "", "", true)]
        [TestCase("A", "", "B", false)]
        [TestCase("A", "", "A", true)]
        [TestCase("", "A", "A", true)]
        [TestCase("", "A", "B", false)]
        [TestCase("A", "B", "A", false)]
        [TestCase("A", "B", "A B", true)]
        [TestCase("A", "B C", "A B", true)]
        [TestCase("", "B C", "A B", true)]
        public void IsApplicable_works(string reqAll, string reqAny, string tags, bool result)
        {
            Modifier modifier = new()
            {
                RequiredTags = (reqAll.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet(), 
                                reqAny.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet())
            };
            Assert.That(modifier.IsApplicable(tags.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList()), Is.EqualTo(result));
        }
    }
}
