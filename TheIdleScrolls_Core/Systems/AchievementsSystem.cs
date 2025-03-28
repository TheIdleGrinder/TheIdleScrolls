﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Resources;
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
                var achievements = AchievementList.GetAllAchievements();
                foreach (var achievement in achievements)
                {
                    achievementsComp.AddAchievement(achievement);
                }
            }

            CheckPrerequisites(achievementsComp.Achievements, coordinator, world);

            CheckConditions(achievementsComp.Achievements, coordinator, world);

            UpdateRewards(achievementsComp.Achievements, coordinator, world);
        }

        void CheckPrerequisites(List<Achievement> achievements, Coordinator coordinator, World world)
        {
            var player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (player == null)
                return;

            foreach (var achievement in achievements)
            {
                if (achievement.Status == AchievementStatus.Unavailable)
                {
                    if (achievement.Prerequisite(player, world))
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
                    if (achievement.Condition(player, world))
                    {
                        achievement.Status = AchievementStatus.Awarded;
                        coordinator.PostMessage(this, new AchievementStatusMessage(achievement));
                    }
                }
            }
        }

        void UpdateRewards(List<Achievement> achievements, Coordinator coordinator, World world)
        {
            var player = coordinator.GetEntities<PlayerComponent, RewardCollectorComponent>().FirstOrDefault();
            if (player == null)
                return;
            var rewardComp = player.GetComponent<RewardCollectorComponent>()!;

            var perksComp = player.GetComponent<PerksComponent>();
            if (perksComp == null)
                return;

            foreach (var achievement in achievements)
            {
                if (achievement.Status == AchievementStatus.Awarded
                    && achievement.Reward != null
                    && !rewardComp.HasCollected(achievement.Id))
                {
                    if (achievement.Reward.GiveReward(player, world, (IMessage m) => coordinator.PostMessage(this, m as dynamic)))
                        rewardComp.Collect(achievement.Id);
                }
            }
        }
    }

    public class AchievementStatusMessage : IMessage
    {
        public Achievement Achievement { get; set; }

        // Store status because it may change before the message is processed
        public AchievementStatus Status { get; set; }

        public AchievementStatusMessage(Achievement achievement)
        {
            Achievement = achievement;
            Status = achievement.Status;
        }

        string IMessage.BuildMessage()
        {
            return Status switch
            {
                AchievementStatus.Unavailable => $"Achievement '{Achievement.Title}' has become unavailable",
                AchievementStatus.Available => $"Achievement '{Achievement.Title}' has become available",
                AchievementStatus.Awarded => $"Achievement unlocked: '{Achievement.Title}'",
                _ => string.Empty,
            };
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return (Status == AchievementStatus.Awarded) 
                    ? IMessage.PriorityLevel.VeryHigh 
                    : IMessage.PriorityLevel.Debug;
        }
    }
}
