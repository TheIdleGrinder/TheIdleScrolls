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
    public class DoubleSwingSkill : ActiveSkill
    {
        const double BaseCooldown = 2.0;

        public static DoubleSwingSkill Skill { get; } = new();
        public override string Id => DoubleSwing.BasePerkId;
        public override string Name => Properties.Skills.DoubleSwing_Name;
        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, DoubleSwing.BasePerkId);
        }
        public override UseTrigger Trigger => UseTrigger.OnAttack;

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            if (!user.IsInBattle())
                return (UsePrevention.NotInBattle, "You can only use this skill in battle.");
            bool twoWeapons = user.HasTag(Tags.DualWield);
            if (!twoWeapons)
                return (UsePrevention.WrongEquipment, "Requires two weapons");
            if (ActiveSkill.GetEnemiesInRange(user, user.GetComponent<BattleStatsComponent>()?.AverageRange ?? 0.0).Count == 0)
                return (UsePrevention.NoTargetInRange, "No target in range");

            return (UsePrevention.None, string.Empty);
        }

        protected override void SetupStats(Entity user)
        {
            SkillTags = [Tags.AttackSkill];

            var attackComp = user.GetComponent<BattleStatsComponent>();
            var perk = user.GetComponent<PerksComponent>()?.GetPerk(DoubleSwing.BasePerkId);
            if (attackComp is null || attackComp.AttackVectors.Count == 0 || perk is null)
            {
                return;
            }

            var activationMod = perk.GetModifier(DoubleSwing.BasePerkActivationId);
            TriggerChance = activationMod?.Value ?? 0.0;

            int otherHand = 1 - attackComp.CurrentHand;
            DamageCluster damage = attackComp.AttackVectors[otherHand].RawDamage * 1.0; // multiply to copy :see_no_evil:
            
            var effects = DefaultAttack.CreateDefaultSkillEffectsForDamage(damage, [.. SkillTags]);

            ActivityStartEffects = [new(effects, TargetingMode.SingleEnemy, SkillTags)];
            ChargingTime = 0.0;
            Timer.CooldownDuration = 0.0;
        }
    }
}
