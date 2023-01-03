using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Utility;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Achievements;

namespace Test_TheIdleScrolls_Core
{
    internal class Test_AchievementKingdomParsing
    {
        [Test]
        public void CanParseAchievementKingdom()
        {
            Assert.DoesNotThrow(() => ResourceAccess.ParseResourceFile<AchievementKingdomDescription>("TheIdleScrolls_Core", "Achievements.json"));

            var achievementKingdom = ResourceAccess.ParseResourceFile<AchievementKingdomDescription>("TheIdleScrolls_Core", "Achievements.json");
            Assert.That(achievementKingdom, Is.Not.Null);
            Assert.That(achievementKingdom.Achievements, Is.Not.Empty);
            HashSet<string> ids = new();
            foreach (var achievement in achievementKingdom.Achievements)
            {
                Assert.That(achievement, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(achievement.Id, Is.Not.Empty);
                    Assert.That(ids.Add(achievement.Id), Is.True);
                    Assert.That(achievement.Title, Is.Not.Empty);
                    Assert.That(achievement.Description, Is.Not.Empty);
                    Assert.That(achievement.Condition, Is.Not.Empty);
                });
                if (achievement.Prerequisite != String.Empty)
                {
                    var referenced = achievementKingdom.Achievements.Where(a => a.Id == achievement.Prerequisite);
                    Assert.That(referenced, Is.Not.Empty);
                }
            }
        }
    }

    internal class Test_AchievementKingdom
    {
        AchievementKingdomDescription kingdom;

        [SetUp]
        public void Setup()
        {
            kingdom = ResourceAccess.ParseResourceFile<AchievementKingdomDescription>("TheIdleScrolls_Core", "Achievements.json");
        }
    }
}
