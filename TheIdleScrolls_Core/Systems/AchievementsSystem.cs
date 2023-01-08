using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Systems
{
    internal class AchievementsSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            var globalEntity = coordinator.GetEntities<AchievementsComponent>().FirstOrDefault();
            if (globalEntity == null)
                return;

            var achievementsComp = globalEntity.GetComponent<AchievementsComponent>();
            if (achievementsComp == null)
                return;

            var player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (player == null)
                return;

            if (achievementsComp.Achievements.Count == 0)
            {
                var kingdom = ResourceAccess.ParseResourceFile<AchievementKingdomDescription>("TheIdleScrolls_Core", "Achievements.json");
                foreach (var achievement in kingdom.Achievements)
                {
                    achievementsComp.AddAchievement(new Achievement(achievement));
                }
            }

            CheckPrerequisites(achievementsComp.Achievements, coordinator);

            CheckConditions(achievementsComp.Achievements, coordinator, world);
        }

        void CheckPrerequisites(List<Achievement> achievements, Coordinator coordinator)
        {
            foreach (var achievement in achievements)
            {
                if (achievement.Status == AchievementStatus.Unavailable)
                {
                    if (achievement.Prerequisite == String.Empty || 
                        achievements.Where(a => a.Id == achievement.Prerequisite 
                            && a.Status == AchievementStatus.Awarded).Any())
                    {
                        achievement.Status = AchievementStatus.Available;
                        coordinator.PostMessage(this, new AchievementStatusMessage(achievement));
                    }
                }
            }
        }

        void CheckConditions(List<Achievement> achievements, Coordinator coordinator, World world)
        {
            var player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (player == null)
                return;

            foreach (var achievement in achievements)
            {
                if (achievement.Status == AchievementStatus.Available)
                {
                    double result = achievement.Condition.Evaluate(player, world);
                    if (result >= 1.0)
                    {
                        achievement.Status = AchievementStatus.Awarded;
                        coordinator.PostMessage(this, new AchievementStatusMessage(achievement));
                    }
                }
            }
        }
    }

    public class AchievementStatusMessage : IMessage
    {
        public Achievement Achievement { get; set; }

        public AchievementStatusMessage(Achievement achievement)
        {
            Achievement = achievement;
        }

        string IMessage.BuildMessage()
        {
            return Achievement.Status switch
            {
                AchievementStatus.Unavailable => $"Achievement '{Achievement.Title}' has become unavailable",
                AchievementStatus.Available => $"Achievement '{Achievement.Title}' has become available",
                AchievementStatus.Awarded => $"Achievement unlocked: '{Achievement.Title}'",
                _ => string.Empty,
            };
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return (Achievement.Status == AchievementStatus.Awarded) ? IMessage.PriorityLevel.VeryHigh : IMessage.PriorityLevel.Debug;
        }
    }
}
