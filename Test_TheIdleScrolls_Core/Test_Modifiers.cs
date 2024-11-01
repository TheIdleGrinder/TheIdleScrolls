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
        [TestCase("", "", true)]
        [TestCase("A", "B", false)]
        [TestCase("A", "A", true)]
        [TestCase("B", "A", false)]
        [TestCase("A", "A B", true)]
        [TestCase("B C", "A B", false)]
        [TestCase("B C", "A B C", true)]
        public void IsApplicable_works(string reqAll, string tags, bool result)
        {
            Modifier modifier = new()
            {
                RequiredLocalTags = reqAll.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet()
            };
            Assert.That(modifier.IsApplicable(tags.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList(), null), Is.EqualTo(result));
        }

        [Test]
        public void ApplyAllApplicable_works()
        {
            List<Modifier> modifiers = new();
            List<string> tags = new();

            Assert.That(modifiers.ApplyAllApplicable(1.0, tags, null), Is.EqualTo(1.0));

            modifiers.Add(new("addBase", ModifierType.AddBase, 1.0, new() { "A" }, new()));
            Assert.That(modifiers.ApplyAllApplicable(4.0, tags, null), Is.EqualTo(4.0));

            tags.Add("A");
            Assert.That(modifiers.ApplyAllApplicable(1.0, tags, null), Is.EqualTo(2.0));
            Assert.That(modifiers.ApplyAllApplicable(2.2, tags, null), Is.EqualTo(3.2));

            modifiers.Add(new("increase", ModifierType.Increase, 0.5, new() { "A" }, new()));
            Assert.That(modifiers.ApplyAllApplicable(1.0, tags, null), Is.EqualTo(3.0));

            modifiers.Add(new("addBase2", ModifierType.AddBase, 2.0, new() { "B" }, new()));
            Assert.That(modifiers.ApplyAllApplicable(1.0, tags, null), Is.EqualTo(3.0));

            tags.Add("B");
            Assert.That(modifiers.ApplyAllApplicable(5.0, tags, null), Is.EqualTo(12.0));

            modifiers.Add(new("increase2", ModifierType.Increase, 0.5, new() { "A" }, new()));
            Assert.That(modifiers.ApplyAllApplicable(9.2, tags, null), Is.EqualTo(24.4));

            modifiers.Add(new("flat", ModifierType.AddFlat, 0.5, new(), new()));
            Assert.That(modifiers.ApplyAllApplicable(9.2, tags, null), Is.EqualTo(24.9));

            modifiers.Add(new("more", ModifierType.More, 0.5, new(), new()));
            Assert.That(modifiers.ApplyAllApplicable(5.0, tags, null), Is.EqualTo(24.5));

            modifiers.Add(new("more2", ModifierType.More, 0.5, new(), new()));
            Assert.That(modifiers.ApplyAllApplicable(1.0, tags, null), Is.EqualTo(18.5));
        }
    }

}
