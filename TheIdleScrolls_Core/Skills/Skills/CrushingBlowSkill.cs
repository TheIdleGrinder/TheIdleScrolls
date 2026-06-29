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
    public class CrushingBlowSkill : ActiveSkill
    {
        const double BaseCooldown = 5.0;
        const double DebuffDuration = 5.0;

        public override string Id => CrushingBlow.BasePerkId;

        public override string Name => Properties.Skills.CrushingBlow_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, CrushingBlow.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            if (!user.IsInBattle())
                return (UsePrevention.NotInBattle, "You can only use this skill in battle.");
            bool weapon2H = user.HasTag(Tags.TwoHanded);
            if (!weapon2H)
                return (UsePrevention.WrongEquipment, "Requires two-handed weapon");
            if (ActiveSkill.GetEnemiesInRange(user, user.GetComponent<BattleStatsComponent>()?.AverageRange ?? 0.0).Count == 0)
                return (UsePrevention.NoTargetInRange, "No target in range");

            return (UsePrevention.None, string.Empty);
        }

        protected override void SetupStats(Entity user)
        {
            SkillTags = [Tags.AttackSkill];

            Entity? weapon = user.GetComponent<EquipmentComponent>()?.GetItems()?.FirstOrDefault(i => i.IsWeapon());
            var battleStatsComp = user.GetComponent<BattleStatsComponent>();
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
            
            var perk = user.GetComponent<PerksComponent>()?.GetPerk(CrushingBlow.BasePerkId);
            if (battleStatsComp is null || battleStatsComp.AttackVectors.Count == 0 || perk is null)
            {
                return;
            }

            DamageCluster damage = ScaleDamage(baseDamage, tags, perk?.Modifiers);

            var effects = DefaultAttack.CreateDefaultSkillEffectsForDamage(damage, [.. SkillTags]);
            GenericModifierStatusEffect crushedEffect = new("Crushed", DebuffDuration, 
                [perk!.GetModifier(CrushingBlow.BasePerkAntiDefenseModId)!], [Tags.Debuff]);
            effects.Add(new SkillEffects.StatusSkillEffect(crushedEffect));

            double cooldownRecovery = ScaleValue(1.0, [Tags.CooldownRecovery]);
            if (cooldownRecovery == 0.0)
                cooldownRecovery = 1.0;

            ActivityStartEffects = [new(effects, TargetingMode.SingleEnemy)];
            ChargingTime = battleStatsComp.AttackVectors[0].AttackTime;
            Timer.CooldownDuration = BaseCooldown / cooldownRecovery;
        }
    }
}
