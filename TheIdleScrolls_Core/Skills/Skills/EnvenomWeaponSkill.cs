using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Perks;
using TheIdleScrolls_Core.StatusEffects;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class EnvenomWeaponSkill : ActiveSkillDefinition
    {
        public static EnvenomWeaponSkill Skill { get; } = new();

        public override string Id => EnvenomWeapon.BasePerkId;

        public override string Name => Properties.Skills.EnvWeapon_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, EnvenomWeapon.BasePerkId);
        }

        public override (bool available, string reason) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (false, "");
            bool hasWeapon = user.GetComponent<EquipmentComponent>()?.GetItems()?.Any(i => i.GetTags().Contains(Definitions.Tags.Weapon)) ?? false;
            if (!hasWeapon)
                return (false, "Requires equipped weapon");
            return (true, "");
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.AlchemySkill, Tags.BuffSkill, Tags.DurationSkill, DamageType.Poison.ToTag()];

            Perk basePerk = skill.GetPerk(EnvenomWeapon.BasePerkId)!;
            Perk? supportPerk = skill.GetPerk(EnvenomWeapon.DurationPerkId);
            List<Modifier> mods = [];
            if (skill.GetPerkLevel(EnvenomWeapon.DurationPerkId) > 0)
                mods = supportPerk?.Modifiers ?? [];

            double chargeSpeed = skill.ScaleValue(1.0, [.. skill.Tags, Tags.Speed], mods);
            double duration = skill.ScaleValue(1.0, [.. skill.Tags, Tags.Duration], mods);
            double recovery = skill.ScaleValue(1.0, [.. skill.Tags, Tags.CooldownRecovery], mods);

            skill.ChargingTime = 1.0 / (chargeSpeed != 0.0 ? chargeSpeed : 1.0);
            skill.Timer.ActiveDuration = 10.0 * duration;
            skill.Timer.CooldownDuration = 3.0 / (recovery != 0.0 ? recovery : 1.0);

            StatusEffect poisonDamage = new GenericModifierStatusEffect("Envenomed Weapon", 0.0, basePerk.Modifiers, []);
            skill.ActiveEffects.WhileIn = [poisonDamage];
        }
    }
}
