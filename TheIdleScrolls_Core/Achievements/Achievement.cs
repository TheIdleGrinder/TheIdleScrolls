using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Achievements
{
    public enum AchievementStatus { Unavailable, Available, Awarded }
    public class Achievement
    {
        public string Id { get; set; } = "";
        public AchievementStatus Status { get; set; }
        public IConditionExpressionNode Prerequisite { get; set; } = new NumericNode(0.0);
        public IConditionExpressionNode Condition { get; set; } = new NumericNode(0.0);
        public bool Hidden { get; set; } = false;
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        public Achievement(AchievementDescription description)
        {
            Id = description.Id;
            Status = AchievementStatus.Unavailable;
            Prerequisite = ExpressionParser.Parse(description.Prerequisite);
            Condition = ExpressionParser.Parse(description.Condition);
            Hidden = description.Hidden;
            Title = description.Title;
            Description = description.Description;
        }
    }
}
