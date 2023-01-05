using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Achievements
{
    public class AchievementKingdomDescription
    {
        public List<AchievementDescription> Achievements { get; set; } = new();
    }

    public class AchievementDescription
    {
        public string Id { get; set; } = "";
        public string Prerequisite { get; set; } = "";
        public string Condition { get; set; } = "";
        public bool Hidden { get; set; } = false; // Achievement is hidden until it has been earned
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
