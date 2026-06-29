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
    public class EnvenomWeaponSkill : ActiveSkill
    {
        public static EnvenomWeaponSkill Skill { get; } = new();

        public override string Id => EnvenomWeapon.BasePerkId;

        public override string Name => Properties.Skills.EnvWeapon_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, EnvenomWeapon.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            if (!user.IsInBattle())
                return (UsePrevention.NotInBattle, "You can only use this skill in battle.");
            bool hasWeapon = user.GetComponent<EquipmentComponent>()?.GetItems()?.Any(i => i.IsWeapon()) ?? false;
            if (!hasWeapon)
                return (UsePrevention.WrongEquipment, "Requires equipped weapon");

            return (UsePrevention.None, string.Empty);
        }

        protected override void SetupStats(Entity user)
        {
            SkillTags = [Tags.AlchemySkill, Tags.BuffSkill, Tags.DurationSkill, DamageType.Poison.ToTag()];

            Perk basePerk = GetPerk(EnvenomWeapon.BasePerkId)!;
            Perk? supportPerk = GetPerk(EnvenomWeapon.DurationPerkId);
            List<Modifier> mods = [];
            if (GetPerkLevel(EnvenomWeapon.DurationPerkId) > 0)
                mods = supportPerk?.Modifiers ?? [];

            double chargeSpeed = ScaleValue(1.0, [.. SkillTags, Tags.Speed], mods);
            double duration = ScaleValue(1.0, [.. SkillTags, Tags.Duration], mods);
            double recovery = ScaleValue(1.0, [.. SkillTags, Tags.CooldownRecovery], mods);

            ChargingTime = 1.0 / (chargeSpeed != 0.0 ? chargeSpeed : 1.0);
            Timer.ActiveDuration = 10.0 * duration;
            Timer.CooldownDuration = 3.0 / (recovery != 0.0 ? recovery : 1.0);
            StatusEffect poisonDamage = new GenericModifierStatusEffect("Envenomed Weapon", 0.0, basePerk.Modifiers, []);
            ActivityWhileInEffects = [poisonDamage];
        }
    }
}
