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
using TheIdleScrolls_Core.Properties;
using TheIdleScrolls_Core.StatusEffects;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class DoubleStrikeSkill : ActiveSkillDefinition
    {
        const double BaseCooldown = 2.0;

        public static DoubleStrikeSkill Skill { get; } = new();
        public override string Id => DoubleStrike.BasePerkId;

        public override string Name => Properties.Skills.DoubleStrike_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, DoubleStrike.BasePerkId);
        }

        public override (bool available, string reason) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (false, "");
            var twoWeapons = (user.GetComponent<EquipmentComponent>()?.GetItems()
                ?.Count(i => i.IsWeapon()) ?? 0) > 1;
            return (twoWeapons, twoWeapons ? "" : "Requires two weapons");
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.AttackSkill];

            var attackComp = user.GetComponent<AttackComponent>();
            var perk = user.GetComponent<PerksComponent>()?.GetPerk(DoubleStrike.BasePerkId);
            if (attackComp is null || attackComp.AttackVectors.Count == 0 || perk is null)
            {
                return;
            }

            DamageCluster damage = attackComp.AttackVectors[0].RawDamage * 1.0; // multiply to copy :see_no_evil:
            double offHandMulti = 1.0 + perk.GetModifier(DoubleStrike.BasePerkOffHandMalusModId)!.Value;
            if (attackComp.AttackVectors.Count > 1)
            {
                DamageCluster offHandDamage = offHandMulti * attackComp.AttackVectors[1].RawDamage;
                damage.Add(offHandDamage);
            }

            var effects = DefaultAttack.CreateDefaultSkillEffectsForDamage(damage, [.. skill.Tags]);

            double cooldownRecovery = skill.ScaleValue(1.0, [Tags.CooldownRecovery]);
            if (cooldownRecovery == 0.0)
                cooldownRecovery = 1.0;

            skill.ActivityStartEffects = effects;
            skill.ChargingTime = attackComp.AverageCooldown;
            skill.Timer.CooldownDuration = BaseCooldown / cooldownRecovery;
        }
    }
}
