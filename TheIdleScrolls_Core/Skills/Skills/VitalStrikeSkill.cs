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
using TheIdleScrolls_Core.Skills.SkillEffects;
using TheIdleScrolls_Core.StatusEffects;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class VitalStrikeSkill : ActiveSkillDefinition
    {
        const double BaseCooldown = 3.0;

        public static VitalStrikeSkill Skill { get; } = new();
        public override string Id => VitalStrike.BasePerkId;

        public override string Name => Properties.Skills.VitalStrike_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, VitalStrike.BasePerkId);
        }

        public override (bool available, string reason) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (false, "");
            var singleHanded = user.HasTag(Tags.SingleHanded);
            return (singleHanded, singleHanded ? "" : "Requires single-handed style");
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.AttackSkill];

            //Entity? weapon = user.GetComponent<EquipmentComponent>()?.GetItems()?.FirstOrDefault(i => i.IsWeapon());
            //DamageCluster baseDamage = new(DamageType.Physical, Stats.UnarmedBaseDamage);
            //HashSet<string> tags = [];
            //if (weapon is null)
            //{
            //    tags = [Tags.Unarmed, Tags.Melee, Tags.Attack];
            //}
            //else
            //{
            //    tags = weapon.GetTags().ToHashSet();
            //    baseDamage = weapon.GetComponent<WeaponComponent>()?.Damage ?? baseDamage;
            //}

            var attackComp = user.GetComponent<AttackComponent>();
            var perk = user.GetComponent<PerksComponent>()?.GetPerk(VitalStrike.BasePerkId);
            if (attackComp is null || attackComp.AttackVectors.Count == 0 || perk is null)
            {
                return;
            }
            //DamageCluster damage = skill.ScaleDamage(baseDamage, tags, perk?.Modifiers);

            DamageCluster damage = attackComp.AverageDamage;

            ISkillEffect pruningEffect = new PruningSkillEffect(Stats.PruningBaseEffect, ISkillEffect.TargetingMode.SingleEnemy);
            var effects = DefaultAttack.CreateDefaultSkillEffectsForDamage(damage, [.. skill.Tags]);
            effects.Insert(0, pruningEffect);

            // Since the mod provides more cooldown, we can take a shortcut here
            double cooldownBonus = perk.GetModifier(VitalStrike.BasePerkCooldownModId)?.Value ?? 0.0;
            double cooldownRecovery = skill.ScaleValue(1.0, [Tags.CooldownRecovery], []);
            if (cooldownRecovery == 0.0)
                cooldownRecovery = 1.0;
            cooldownRecovery *= 1.0 + cooldownBonus;

            skill.ActivityStartEffects = effects;
            skill.ChargingTime = attackComp.AverageCooldown;
            skill.Timer.CooldownDuration = BaseCooldown / cooldownRecovery;
        }
    }
}
