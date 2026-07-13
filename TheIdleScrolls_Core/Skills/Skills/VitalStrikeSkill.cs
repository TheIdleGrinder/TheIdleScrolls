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
    public class VitalStrikeSkill : ActiveSkill
    {
        const double BaseCooldown = 3.0;

        public static VitalStrikeSkill Skill { get; } = new();
        public override string Id => VitalStrike.BasePerkId;

        public override string Name => Properties.Skills.VitalStrike_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, VitalStrike.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            if (!user.IsInBattle())
                return (UsePrevention.NotInBattle, "You can only use this skill in battle.");
            var singleHanded = user.HasTag(Tags.SingleHanded);
            if (!singleHanded)
                return (UsePrevention.WrongEquipment, "Requires single-handed style");
            if (ActiveSkill.GetEnemiesInRange(user, user.GetComponent<BattleStatsComponent>()?.AverageRange ?? 0.0).Count == 0)
                return (UsePrevention.NoTargetInRange, "No target in range");
            return (UsePrevention.None, string.Empty);
        }

        protected override void SetupStats(Entity user)
        {
            SkillTags = [Tags.AttackSkill];

            var attackComp = user.GetComponent<BattleStatsComponent>();
            var perk = user.GetComponent<PerksComponent>()?.GetPerk(VitalStrike.BasePerkId);
            if (attackComp is null || attackComp.AttackVectors.Count == 0 || perk is null)
            {
                return;
            }

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


            //DamageCluster damage = skill.ScaleDamage(baseDamage, tags, perk?.Modifiers);

            DamageCluster damage = attackComp.AverageDamage;

            ISkillEffect pruningEffect = new PruningSkillEffect(Stats.PruningBaseEffect);
            var effects = DefaultAttack.CreateDefaultSkillEffectsForDamage(damage, [.. SkillTags]);
            effects.Insert(0, pruningEffect);

            // Since the mod provides more cooldown, we can take a shortcut here
            double cooldownBonus = perk.GetModifier(VitalStrike.BasePerkCooldownModId)?.Value ?? 0.0;
            double cooldownRecovery = ScaleValue(1.0, [Tags.CooldownRecovery], []);
            if (cooldownRecovery == 0.0)
                cooldownRecovery = 1.0;
            cooldownRecovery *= 1.0 + cooldownBonus;

            ActivityStartEffects = [new(effects, TargetingMode.SingleEnemy, [.. SkillTags])];
            ChargingTime = attackComp.AverageCooldown;
            Timer.CooldownDuration = BaseCooldown / cooldownRecovery;
        }
    }
}
