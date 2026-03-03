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
    public class CrushingBlowSkill : ActiveSkillDefinition
    {
        const double BaseCooldown = 5.0;
        const double DebuffDuration = 5.0;

        public static CrushingBlowSkill Skill { get; } = new();
        public override string Id => CrushingBlow.BasePerkId;

        public override string Name => Properties.Skills.CrushingBlow_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, CrushingBlow.BasePerkId);
        }

        public override (bool available, string reason) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (false, "");
            var weapon2H = user.HasTag(Tags.TwoHanded);
            return (weapon2H, weapon2H ? "" : "Requires two-handed weapon");
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.AttackSkill];

            Entity? weapon = user.GetComponent<EquipmentComponent>()?.GetItems()?.FirstOrDefault(i => i.IsWeapon());
            DamageCluster baseDamage = new(DamageType.Physical, Stats.UnarmedBaseDamage);
            HashSet<string> tags = [];
            if (weapon is null)
            {
                tags = [Tags.Unarmed, Tags.Melee, Tags.Attack];
            }
            else
            {
                tags = weapon.GetTags().ToHashSet();
                baseDamage = weapon.GetComponent<WeaponComponent>()?.Damage ?? baseDamage;
            }

            var attackComp = user.GetComponent<AttackComponent>();
            var perk = user.GetComponent<PerksComponent>()?.GetPerk(CrushingBlow.BasePerkId);
            if (attackComp is null || attackComp.AttackVectors.Count == 0 || perk is null)
            {
                return;
            }

            DamageCluster damage = skill.ScaleDamage(baseDamage, tags, perk?.Modifiers);

            var effects = DefaultAttack.CreateDefaultSkillEffectsForDamage(damage, [.. skill.Tags]);
            GenericModifierStatusEffect crushedEffect = new("Crushed", DebuffDuration, 
                [perk!.GetModifier(CrushingBlow.BasePerkAntiDefenseModId)!], [Tags.Debuff]);
            effects.Add(new SkillEffects.StatusSkillEffect(ISkillEffect.TargetingMode.SingleEnemy, crushedEffect));

            double cooldownRecovery = skill.ScaleValue(1.0, [Tags.CooldownRecovery]);
            if (cooldownRecovery == 0.0)
                cooldownRecovery = 1.0;

            skill.ActivityStartEffects = effects;
            skill.ChargingTime = attackComp.AttackVectors[0].Cooldown;
            skill.Timer.CooldownDuration = BaseCooldown / cooldownRecovery;
        }
    }
}
