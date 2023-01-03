using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Achievements
{
    public enum AchievementStatus { Unavailable, Available, Awarded }
    public class Achievement
    {
        public string Id { get; set; } = "";
        public AchievementStatus Status { get; set; }
        public string Prerequisite { get; set; } = "";
        public IConditionExpressionNode Condition { get; set; } = new NumericNode(0.0);
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        public Achievement(AchievementDescription description)
        {

        }
    }
}
