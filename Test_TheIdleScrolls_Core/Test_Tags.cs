using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;

namespace Test_TheIdleScrolls_Core
{
    public class Test_Tags
    {
        [Test]
        public void Constructing_default_works()
        {
            var comp = new TagsComponent();
            Assert.That(comp, Is.Not.Null);
            Assert.That(comp.ListTags(), Is.Empty);
        }

        [Test]
        public void Constructing_works_with_parameters()
        {
            var tags = new List<string>() { "A" };
            var comp = new TagsComponent(tags);
            Assert.That(comp, Is.Not.Null);
            Assert.That(comp.ListTags(), Has.Count.EqualTo(1));
            Assert.That(comp.ListTags()[0], Is.EqualTo("A"));
        }

        [Test]
        public void Generally_works()
        {
            TagsComponent comp = new TagsComponent();
            Assert.That(comp.ListTags(), Is.Empty);

            Assert.That(comp.AddTag("A"));
            Assert.That(comp.ListTags(), Has.Count.EqualTo(1));

            comp.AddTags(new List<string>() { "D", "B" });
            var tags = comp.ListTags();
            Assert.That(tags, Has.Count.EqualTo(3));
            Assert.That(tags[1], Is.EqualTo("B"));

            comp.AddTags(new List<string>() { "B", "C" });
            tags = comp.ListTags();
            Assert.That(tags, Has.Count.EqualTo(4));
            Assert.That(tags[2], Is.EqualTo("C"));

            Assert.That(!comp.AddTag("D"));
            Assert.That(comp.ListTags, Has.Count.EqualTo(4));

            Assert.That(comp.HasTag("A"));
            Assert.That(comp.RemoveTag("A"));
            Assert.That(!comp.HasTag("A"));
            Assert.That(comp.ListTags(), Has.Count.EqualTo(3));
        }

        [Test]
        public void Correct_tags_are_set_in_items()
        {
            var spear = ItemFactory.MakeItem(new("POL0"));
            Assert.That(spear, Is.Not.Null);
            Assert.That(spear.HasTag("2H"));
            Assert.That(spear.HasTag("POL"));
        }
    }
}
