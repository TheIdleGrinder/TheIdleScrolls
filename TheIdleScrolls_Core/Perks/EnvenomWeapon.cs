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
    public static class EnvenomWeapon
    {
        public static readonly string BasePerkId = "EnvenomWeapon";
        public static readonly string DurationPerkId = BasePerkId + "_Duration";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.EnvWeapon_Name,
            Properties.Skills.EnvWeapon_Description,
            [],
            (l, e, w, c) => 
            {
                double dmg = 2.0 * l;
                return [new(BasePerkId, ModifierType.AddBase, dmg, [Tags.Damage, DamageType.Poison.ToTag(), Tags.Weapon], [])];
            }
        )
        {
            MaxLevel = 8,
            ApplyModifiersToOwner = false,
            Skill = EnvenomWeaponSkill.Skill
        };

        public static readonly Perk SupportPerk = new(
            DurationPerkId,
            Properties.Skills.EnvWeaponSupport_Name,
            Properties.Skills.EnvWeaponSupport_Description,
            [],
            (l, e, w, c) =>
            {
                double bonus = 0.2 * l;
                return [
                    new(DurationPerkId + "_speed",    ModifierType.Increase, bonus, [Tags.Buff, Tags.Speed], []),
                    new(DurationPerkId + "_recovery", ModifierType.Increase, bonus, [Tags.CooldownRecovery], [])
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false
        };
    }
}
