using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Utility;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Achievements;
using MiniECS;

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
                // Achievements no longer reference other achievements in their prerequisites
                //if (achievement.Prerequisite != String.Empty)
                //{
                //    var referenced = achievementKingdom.Achievements.Where(a => a.Id == achievement.Prerequisite);
                //    Assert.That(referenced, Is.Not.Empty);
                //}
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

        [Test]
        public void TransformationToAchievementsWorks()
        {
            Assert.That(kingdom, Is.Not.Null);
            List<Achievement> achievements = new();
            Assert.DoesNotThrow(() => kingdom.Achievements.ForEach(a => achievements.Add(new Achievement(a))));
            Assert.That(achievements, Is.Not.Empty);

            Entity player = PlayerFactory.MakeNewPlayer("Test");
            World world = new();
            Assert.DoesNotThrow(() => achievements.ForEach(a => a.Condition.Evaluate(player, world)));
        }
    }
}
