using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace Test_TheIdleScrolls_Core
{
    internal class Test_AreaKingdom
    {
        [Test]
        public void CanParseAreas()
        {
            Assert.DoesNotThrow(() => ResourceAccess.ParseResourceFile<AreaKingdomDescription>("TheIdleScrolls_Core", "Dungeons.json"));

            var itemKingdom = ResourceAccess.ParseResourceFile<AreaKingdomDescription>("TheIdleScrolls_Core", "Dungeons.json");
            Assert.That(itemKingdom, Is.Not.Null);
            Assert.That(itemKingdom.Dungeons, Has.Count.GreaterThanOrEqualTo(2));
            Assert.That(itemKingdom.Dungeons[0].Name, Is.Not.EqualTo(""));
            Assert.That(itemKingdom.Dungeons[0].Level, Is.GreaterThan(0));
            Assert.That(itemKingdom.Dungeons[0].Floors, Has.Count.GreaterThan(0));
            Assert.That(itemKingdom.Dungeons[0].LocalMobs, Has.Count.GreaterThan(0));
        }
    }
}
