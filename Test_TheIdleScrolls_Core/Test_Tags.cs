using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Systems;
using TheIdleScrolls_Core.Definitions;

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

            comp.Reset(new List<string>() { "E" });
            Assert.That(comp.ListTags(), Has.Count.EqualTo(1));
            Assert.That(comp.HasTag("E"));
        }

        [TestCase("W1", "POL", 1, 0, "2H")]
        [TestCase("M2", "SBL", 2, 1, "1H")]
        [TestCase("M1", "HAR", 2, 0, "Head")]
        [TestCase("M0", "HAR", 0, 1, "Chest")]
        [TestCase("L0", "LAR", 3, 2, "Arms")]
        [TestCase("L2", "LAR", 9, 0, "Legs")]
        [TestCase("M2", "HAR", 5, 1, "Shield")]
        public void Correct_tags_are_set_in_items(string material, string family, int genus, int rarity, string slots)
        {
            var item = ItemFactory.MakeItem(new($"{material}-{family}{genus}+{rarity}"));
            Assert.That(item, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(item.HasTag(slots));
                Assert.That(item.HasTag(family));
                Assert.That(item.HasTag($"MAT_{material}"));
            });
            if (rarity > 0)
            {
                Assert.That(item!.HasTag($"+{rarity}"));
            }
            if (item.IsWeapon())
            {
                Assert.That(item.HasTag(Tags.Weapon));
                Assert.That(item.HasTag(Tags.Melee));
            }
            if (item.IsArmor())
            {
                Assert.That(item.HasTag(Tags.Armor));
            }
        }

        [Test]
        public void Correct_tags_are_set_in_player()
        {
            Entity player = new();
            EquipmentComponent equipComp = new();
            player.AddComponent(equipComp);

            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(player.HasTag(Tags.Unarmored));
            Assert.That(player.HasTag(Tags.Unarmed));


            var sword = ItemFactory.MakeItem(new("M1-SBL1"));
            Assert.That(sword, Is.Not.Null);
            Assert.That(equipComp.EquipItem(sword));
            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(!player.HasTag(Tags.Unarmed));

            var sword2 = ItemFactory.MakeItem(new("M0-SBL1"));
            Assert.That(sword2, Is.Not.Null);
            Assert.That(equipComp.EquipItem(sword2));
            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(player.HasTag(Tags.DualWield));

            var axe = ItemFactory.MakeItem(new("M1-AXE1"));
            Assert.That(axe, Is.Not.Null);
            Assert.That(equipComp.UnequipItem(sword2));
            Assert.That(equipComp.EquipItem(axe));
            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(player.HasTag(Tags.DualWield));
            Assert.That(player.HasTag(Tags.MixedWeapons));

            var chest = ItemFactory.MakeItem(new("M1-HAR1"));
            Assert.That(chest, Is.Not.Null);
            Assert.That(equipComp.EquipItem(chest));
            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(!player.HasTag(Tags.Unarmored));

            var helmet = ItemFactory.MakeItem(new("L1-LAR2"));
            Assert.That(helmet, Is.Not.Null);
            Assert.That(equipComp.EquipItem(helmet));
            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(player.HasTag(Tags.MixedArmor));
        }
    }
}
