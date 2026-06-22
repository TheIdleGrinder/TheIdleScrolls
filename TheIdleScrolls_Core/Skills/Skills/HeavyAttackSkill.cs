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
    public class HeavyAttackSkill : ActiveSkillDefinition
    {
        const double BaseCooldown = 5.0;

        public static HeavyAttackSkill Skill { get; } = new();
        public override string Id => HeavyAttack.BasePerkId;

        public override string Name => Properties.Skills.HeavyAttack_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, HeavyAttack.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            if (!user.IsInBattle())
                return (UsePrevention.NotInBattle, "You can only use this skill in battle.");
            if (ActiveSkill.GetEnemiesInRange(user, user.GetComponent<BattleStatsComponent>()?.AverageRange ?? 0.0).Count == 0)
                return (UsePrevention.NoTargetInRange, "No target in range");
            return (UsePrevention.None, "");
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.AttackSkill];

            List<string> AdditionalTags = [Tags.Attack, Skill.Id];

            List<Modifier> additionalMods = [];
            List<ISkillEffect> additionalEffects = [];

            var basePerk = user.GetComponent<PerksComponent>()?.GetPerk(HeavyAttack.BasePerkId);
            if (basePerk is null)
            {
                return;
            }            
            additionalMods.AddRange(basePerk.Modifiers);

            int dwLevel = skill.GetPerkLevel(HeavyAttack.DualWieldingBonusPerkId);
            if (dwLevel > 0)
            {
                var dwPerk = skill.GetPerk(HeavyAttack.DualWieldingBonusPerkId)!;
                additionalMods.AddRange(dwPerk.Modifiers);
            }

            int shieldLevel = skill.GetPerkLevel(HeavyAttack.ShieldedBonusPerkId);
            if (shieldLevel > 0)
            {
                var shieldPerk = skill.GetPerk(HeavyAttack.ShieldedBonusPerkId)!;
                additionalMods.AddRange(shieldPerk.Modifiers);
            }

            int shLevel = skill.GetPerkLevel(HeavyAttack.SingleHandedBonusPerkId);
            if (shLevel > 0)
            {
                var shPerk = skill.GetPerk(HeavyAttack.SingleHandedBonusPerkId)!;
                additionalMods.AddRange(shPerk.Modifiers);
            }

            int thLevel = skill.GetPerkLevel(HeavyAttack.TwoHandedBonusPerkId);
            if (thLevel > 0)
            {
                var thPerk = skill.GetPerk(HeavyAttack.TwoHandedBonusPerkId)!;
                double stunMod = thPerk.GetModifier(HeavyAttack.TwoHandedBonusStunModId)?.Value ?? 0.0;
                additionalEffects.Add(new StatusSkillEffect(new StunStatusEffect(stunMod)));
            }

            BattleStatsComponent attackComp = new(new(new(DamageType.Physical, 2.0), 1.0, 0.0));
            DefaultAttack.SetupAttackComponent(user, attackComp, additionalMods);
                        
            skill.ActivityStartEffects = [new(DefaultAttack
                .CreateDefaultSkillEffectsForDamage(attackComp.AttackVectors[0].RawDamage, [.. AdditionalTags])
                .Concat(additionalEffects).ToList(), TargetingMode.SingleEnemy)];
            skill.ChargingTime = attackComp.AverageCooldown;
            skill.Timer.CooldownDuration = BaseCooldown;
        }
    }
}
