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
    public class ShieldChargeSkill : ActiveSkillDefinition
    {
        public static ShieldChargeSkill Skill { get; } = new();
        public override string Id => ShieldCharge.BasePerkId;

        public override string Name => Properties.Skills.ShieldCharge_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, ShieldCharge.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            if (!user.IsInBattle())
                return (UsePrevention.NotInBattle, "You can only use this skill in battle.");
            bool hasShield = user.HasTag(Tags.Shielded);
            if (!hasShield)
                return (UsePrevention.WrongEquipment, "Requires Shield");
            if (ActiveSkill.GetEnemiesInRange(user, user.GetComponent<BattleStatsComponent>()?.AverageRange ?? 0.0).Count == 0)
                return (UsePrevention.NoTargetInRange, "No target in range");

            return (UsePrevention.None, string.Empty);
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.AttackSkill];
            var battleStatsComp = user.GetComponent<BattleStatsComponent>();

            Entity? weapon = user.GetComponent<EquipmentComponent>()?.GetItems()?.FirstOrDefault(i => i.IsWeapon());
            DamageCluster baseDamage = battleStatsComp?.BaseAttack.RawDamage ?? new(DamageType.Physical, 2.0);
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

            var perk = user.GetComponent<PerksComponent>()?.GetPerk(ShieldCharge.BasePerkId);
            if (battleStatsComp is null || battleStatsComp.AttackVectors.Count == 0 || perk is null)
            {
                return;
            }

            DamageCluster damage = skill.ScaleDamage(baseDamage, tags, perk?.Modifiers);

            double cooldownRecovery = skill.ScaleValue(1.0, [Tags.CooldownRecovery]);
            if (cooldownRecovery == 0.0)
                cooldownRecovery = 1.0;

            var effects = DefaultAttack.CreateDefaultSkillEffectsForDamage(damage, [.. skill.Tags]);
            skill.ActivityStartEffects = [new(effects, TargetingMode.SingleEnemy)];
            skill.ChargingTime = battleStatsComp.AttackVectors[0].AttackTime;
            skill.Timer.CooldownDuration = 5.0 / cooldownRecovery;

            Modifier defMod = perk?.GetModifier(ShieldCharge.BasePerkArmorModId)!;
            skill.ChargingWhileInEffects = [new GenericModifierStatusEffect("Raised Shield", 0.0, [defMod], [])];
        }
    }
}
