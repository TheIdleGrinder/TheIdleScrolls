using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Skills.Skills;

namespace TheIdleScrolls_Core.Perks
{
    public static class HeavyAttackPerks
    {
        public static readonly string BasePerkId = "HeavyAttack";
        public static readonly string BasePerkDmgModId = BasePerkId + "_dmg";
        public static readonly string BasePerkSpdModId = BasePerkId + "_spd";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            "Heavy Attack",
            "A slow but powerful attack",
            [],
            (l, e, w, c) =>
            {
                double dmgBonus = 0.35 + 0.15 * l;
                double spdMalus = 0.2;
                return [
                    new Modifier(BasePerkDmgModId, ModifierType.More, dmgBonus,  [Tags.Damage], []),
                    new Modifier(BasePerkSpdModId, ModifierType.More, -spdMalus, [Tags.AttackSpeed], [])
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false,
            Skill = HeavyAttackSkill.Skill
        };
    }
}
