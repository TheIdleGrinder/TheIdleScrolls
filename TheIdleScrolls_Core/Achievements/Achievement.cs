using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Achievements
{
    using ConditionChecker = Func<Entity, GameWorld.World, bool>;

    public enum AchievementStatus { Unavailable, Available, Awarded }
    public class Achievement
    {
        public string Id { get; set; } = "";
        public AchievementStatus Status { get; set; } = AchievementStatus.Unavailable;
        public ConditionChecker Prerequisite { get; set; } = (e, w) => false;
        public ConditionChecker Condition { get; set; } = (e, w) => false;
        public bool Hidden { get; set; } = false; 
        public string Title { get; set; } = "???";
        public string Description { get; set; } = "";
        public Perk? Perk { get; set; } = null;

        public Achievement(AchievementDescription description)
        {
            Id = description.Id;
            Status = AchievementStatus.Unavailable;
            Prerequisite = ExpressionParser.ParseToFunction(description.Prerequisite);
            Condition = ExpressionParser.ParseToFunction(description.Condition);
            Hidden = description.Hidden;
            Title = description.Title;
            Description = description.Description;
        }

        public Achievement()
        {

        }

        public Achievement(string id,
                           string title,
                           string description,
                           ConditionChecker prerequisite, 
                           ConditionChecker condition)
        {
            Id = id;
            Prerequisite = prerequisite;
            Condition = condition;
            Title = title;
            Description = description;
        }
    }
}
