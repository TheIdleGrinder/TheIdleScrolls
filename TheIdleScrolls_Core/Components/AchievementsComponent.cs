using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;

namespace TheIdleScrolls_Core.Components
{
    public class AchievementsComponent : IComponent
    {
        public List<Achievement> Achievements { get; set; } = new();

        public HashSet<string> EarnedAchievements { get; set; } = new(); // Used for storing in save file

        public AchievementsComponent()
        {

        }

        public void AddAchievement(Achievement achievement)
        {
            Achievements.RemoveAll(a => a.Id == achievement.Id);

            if (EarnedAchievements.Contains(achievement.Id))
            {
                EarnedAchievements.Remove(achievement.Id);
                achievement.Status = AchievementStatus.Awarded;
            }

            Achievements.Add(achievement);
        }

        public bool IsAchievementUnlocked(string id)
        {
            return (Achievements.FirstOrDefault(a => a.Id == id)?.Status ?? AchievementStatus.Unavailable) == AchievementStatus.Awarded;
        }
    }
}
