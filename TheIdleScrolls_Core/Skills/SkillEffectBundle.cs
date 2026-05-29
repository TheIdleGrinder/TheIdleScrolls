using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;

namespace TheIdleScrolls_Core.Skills
{
    public enum TargetingMode { Self, SingleEnemy }

    public class SkillEffectBundle
    {
        public TargetingMode Target { get; set; }
        public List<ISkillEffect> Effects { get; set; } = [];
        public double? Accuracy { get; set; } = null;

        public SkillEffectBundle() { }
        public SkillEffectBundle(List<ISkillEffect> effects, TargetingMode target)
        {
            Effects = effects;
            Target = target;
        }

        public SkillEffectBundle(ISkillEffect effect, TargetingMode target)
        {
            Effects = [effect];
            Target = target;
        }

        public string Description =>
            $"To {TargetToString()}:\n" +
            string.Join("\n", Effects.Select(e => "\t" + e.Description));

        public void ApplyToTarget(Entity target)
        {
            // Handle accuracy
            if (Accuracy.HasValue)
            {
                double evasion = target.GetComponent<BattleStatsComponent>()?.Evasion ?? 0.0;
                double charge = Math.Min(Stats.MaxResistanceFromEvasion, evasion / (evasion + Accuracy.Value));
                var chanceComp = target.GetOrAddComponent<ChanceChargeComponent>();
                int chargeCount = chanceComp.GetFullChargeCount(Tags.Evasion);
                if (chargeCount > 0)
                {
                    chanceComp.RemoveCharge(Tags.Evasion, chargeCount);
                }
                chanceComp.AddCharge(Tags.Evasion, charge);
                if (chargeCount > 0)
                    return; // Attack was evaded, so we don't apply the effects
            }

            foreach (var effect in Effects)
            {
                effect.ApplyToTarget(target);
            }
        }

        string TargetToString()
        {
            return Target switch
            {
                TargetingMode.Self => "self",
                TargetingMode.SingleEnemy => "single enemy",
                _ => "unknown target"
            };
        }
    }
}
