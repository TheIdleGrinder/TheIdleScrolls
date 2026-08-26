using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Skills;
using TheIdleScrolls_Core.Skills.Skills;

namespace TheIdleScrolls_Core.Perks
{
    public class Unrelenting
    {
        public static readonly string BasePerkId = "Unrelenting";
        public static readonly string BasePerkPercentageId = BasePerkId + "_pct";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            "Unrelenting",
            "",
            [],
            (l, e, w, c) =>
            {
                double percentage = l switch
                {
                    1 => 0.05,
                    2 => 0.06,
                    3 => 0.08,
                    4 => 0.10,
                    5 => 0.12,
                    6 => 0.15,
                    _ => 0.0
                };

                return [
                    new Modifier(BasePerkPercentageId, ModifierType.AddBase, percentage, [Tags.Healing], [])
                    {
                        AlwaysPercentage = true,
                        CoverText = "Recover {0} of maximum HP when you defeat an enemy"
                    }
                ];
            }
        )
        {
            ApplyModifiersToOwner = false,
            MaxLevel = 6,
            Skill = new UnrelentingSkill()
        };
    }
}
