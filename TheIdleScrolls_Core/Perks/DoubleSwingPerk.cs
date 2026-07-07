using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Skills.Skills;

namespace TheIdleScrolls_Core.Perks
{
    public static class DoubleSwing
    {
        public static readonly string BasePerkId = "DoubleSwing";
        public static readonly string BasePerkActivationId = BasePerkId + "_activation";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.DoubleSwing_Name,
            Properties.Skills.DoubleSwing_Description,
            [],
            (l, e, w, c) =>
            {
                double chance = Math.Round(0.10 + Math.Pow(l, 1.5) / 100, 2);

                return [
                    new Modifier(BasePerkActivationId, ModifierType.AddBase, chance,
                        [Tags.ActivationChance], [])
                    {
                        AlwaysPercentage = true
                    }
                ];
            }
        )
        {
            MaxLevel = 6,
            ApplyModifiersToOwner = false,
            Skill = DoubleSwingSkill.Skill
        };
    }
}
