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
    public static class VitalStrike
    {
        public static readonly string BasePerkId = "VitalStrike";
        public static readonly string BasePerkDmgModId = BasePerkId + "_dmg";
        public static readonly string BasePerkCooldownModId = BasePerkId + "_cooldown";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.VitalStrike_Name,
            Properties.Skills.VitalStrike_Description,
            [],
            (l, e, w, c) =>
            {
                double multi = 0.2 * (l - 1);

                return [
                    new Modifier(BasePerkCooldownModId, ModifierType.More, multi,
                        [Tags.CooldownRecovery], [])
                ];
            }
        )
        {
            MaxLevel = 6,
            ApplyModifiersToOwner = false,
            Skill = VitalStrikeSkill.Skill
        };
    }
}
