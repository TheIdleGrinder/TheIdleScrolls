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
            TagsComponent comp = new();
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

        [TestCase(MaterialId.Wood1, ItemFamilies.Polearm, 0, 0, "2H")]
        [TestCase(MaterialId.Metal3, ItemFamilies.ShortSword, 1, 1, "1H")]
        [TestCase(MaterialId.Metal2, ItemFamilies.HeavyHelmet, 1, 0, "Head")]
        [TestCase(MaterialId.Simple, ItemFamilies.HeavyChest, 0, 1, "Chest")]
        [TestCase(MaterialId.Leather1, ItemFamilies.LightGloves, 0, 2, "Arms")]
        [TestCase(MaterialId.Leather3, ItemFamilies.LightBoots, 1, 0, "Legs")]
        [TestCase(MaterialId.Wood3, ItemFamilies.HeavyShield, 1, 1, "Shield")]
        public void Correct_tags_are_set_in_items(MaterialId material, string family, int genus, int quality, string slots)
        {
            ItemBlueprint blueprint = new(family, genus, material, quality);
            var item = ItemFactory.MakeItem(blueprint);
            Assert.That(item, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(item.HasTag(slots));
                Assert.That(item.HasTag(family));
                Assert.That(item.HasTag(blueprint.GetMaterial().Name));
            });
            if (quality > 0)
            {
                Assert.That(item!.HasTag($"+{quality}"));
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


            var sword = ItemFactory.MakeItem(new(ItemFamilies.ShortSword, 1, MaterialId.Metal3));
            Assert.That(sword, Is.Not.Null);
            Assert.That(equipComp.EquipItem(sword));
            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(!player.HasTag(Tags.Unarmed));

            var sword2 = ItemFactory.MakeItem(new(ItemFamilies.ShortSword, 1, MaterialId.Metal3));
            Assert.That(sword2, Is.Not.Null);
            Assert.That(equipComp.EquipItem(sword2));
            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(player.HasTag(Tags.DualWield));

            var axe = ItemFactory.MakeItem(new(ItemFamilies.OneHandedAxe, 1, MaterialId.Metal1));
            Assert.That(axe, Is.Not.Null);
            Assert.That(equipComp.UnequipItem(sword2));
            Assert.That(equipComp.EquipItem(axe));
            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(player.HasTag(Tags.DualWield));
            Assert.That(player.HasTag(Tags.MixedWeapons));

            var chest = ItemFactory.MakeItem(new(ItemFamilies.HeavyChest, 2, MaterialId.Metal2));
            Assert.That(chest, Is.Not.Null);
            Assert.That(equipComp.EquipItem(chest));
            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(!player.HasTag(Tags.Unarmored));

            var helmet = ItemFactory.MakeItem(new(ItemFamilies.LightHelmet, 0, MaterialId.Leather2));
            Assert.That(helmet, Is.Not.Null);
            Assert.That(equipComp.EquipItem(helmet));
            StatUpdateSystem.UpdatePlayerTags(player);
            Assert.That(player.HasTag(Tags.MixedArmor));
        }
    }
}
