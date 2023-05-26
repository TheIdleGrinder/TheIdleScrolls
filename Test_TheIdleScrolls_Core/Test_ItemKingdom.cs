using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace Test_TheIdleScrolls_Core
{
    public class Test_ItemKingdom
    {
        [Test]
        public void CanParseEquipmentSlot()
        {
            var slot = EquipmentSlot.Hand;
            string serialized = JsonSerializer.Serialize(slot);
            string stringified = slot.ToString();
            slot = (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), stringified);

            Assert.DoesNotThrow(() => JsonSerializer.Deserialize<EquipmentSlot>(serialized));
            var deserialized = JsonSerializer.Deserialize<EquipmentSlot>(serialized);
            Assert.That(deserialized, Is.EqualTo(slot));
        }

        [Test]
        public void ParsingWorks()
        {
            Assert.DoesNotThrow(() => ResourceAccess.ParseResourceFile<ItemKingdomDescription>("TheIdleScrolls_Core", "Items.json"));

            var itemKingdom = ResourceAccess.ParseResourceFile<ItemKingdomDescription>("TheIdleScrolls_Core", "Items.json");
            Assert.That(itemKingdom, Is.Not.Null);
            Assert.That(itemKingdom.Families, Has.Count.EqualTo(7));
            Assert.That(itemKingdom.Families[0].Genera, Has.Count.EqualTo(4));
            Assert.That(itemKingdom.Families[0].Genera[0].Weapon, Is.Not.Null);
            Assert.That(itemKingdom.Families[0].Genera[0].Equippable, Is.Not.Null);
            Assert.That(itemKingdom.Families[0].Genera[0].Equippable!.Slots, Is.EqualTo(new List<string>() { "Hand" }));
            Assert.That(itemKingdom.Families[0].Genera[0].Armor, Is.Null);
        }
    }
}
