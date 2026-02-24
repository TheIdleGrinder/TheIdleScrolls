using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Perks;
using TheIdleScrolls_Core.Properties;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class HeavyAttackSkill : ActiveSkillDefinition
    {
        public static HeavyAttackSkill Skill { get; } = new();
        public override string Id => HeavyAttackPerks.BasePerkId;

        public override string Name => "Heavy Attack";

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, HeavyAttackPerks.BasePerkId);
        }

        public override (bool available, string reason) IsUsableBy(Entity user)
        {
            return (IsAvailableTo(user), string.Empty);
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.AttackSkill];

            var attackComp = user.GetComponent<AttackComponent>();
            var perk = user.GetComponent<PerksComponent>()?.GetPerk(HeavyAttackPerks.BasePerkId);
            if (attackComp is null || perk is null)
            {
                return;
            }

            List<string> AdditionalTags = [Tags.Attack, Skill.Id];
            double dmg = perk.Modifiers.FirstOrDefault(p => p.Id == HeavyAttackPerks.BasePerkDmgModId)?.Value ?? 0.0;
            double spd = perk.Modifiers.FirstOrDefault(p => p.Id == HeavyAttackPerks.BasePerkSpdModId)?.Value ?? 0.0;
                        
            DamageCluster damage = attackComp.AverageDamage.Multiply(1.0 + dmg);
            skill.ActiveEffects.OnEnter = DefaultAttack.CreateDefaultSkillEffectsForDamage(damage, [.. AdditionalTags]);
            skill.ChargingTime = (spd != 0.0) ? attackComp.AverageCooldown / (1.0 + spd) : double.PositiveInfinity;
            skill.Timer.CooldownDuration = 5.0;
        }
    }
}
