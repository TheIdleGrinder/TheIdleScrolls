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
    public static class DoubleStrike
    {
        public static readonly string BasePerkId = "DoubleStrike";
        public static readonly string BasePerkOffHandMalusModId = BasePerkId + "_offhand";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.DoubleStrike_Name,
            Properties.Skills.DoubleStrike_Description,
            [],
            (l, e, w, c) =>
            {
                double multi = 0.20 + 0.15 * l;

                return [
                    new Modifier(BasePerkOffHandMalusModId, ModifierType.More, -(1.0 - multi),
                        [Tags.Damage, Tags.OffHand], [])
                ];
            }
        )
        {
            MaxLevel = 6,
            ApplyModifiersToOwner = false,
            Skill = DoubleStrikeSkill.Skill
        };
    }
}
