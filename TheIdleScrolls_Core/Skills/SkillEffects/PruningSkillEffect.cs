using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using static TheIdleScrolls_Core.Skills.ISkillEffect;

namespace TheIdleScrolls_Core.Skills.SkillEffects
{
    public class PruningSkillEffect(double magnitude, TargetingMode target) : ISkillEffect
    {
        public double Magnitude => magnitude;

        public string Description => $"Remove {magnitude:0.##%} of current " +
            $"HP from {(Target == TargetingMode.Self ? "self" : "target")}";

        public ISkillEffect.TargetingMode Target => target;

        public double DamageDone { get; private set; } = 0.0;

        public void ApplyToTarget(Entity target)
        {
            var hpComp = target.GetComponent<LifePoolComponent>();
            if (hpComp is null)
            {
                return;
            }
            var damage = hpComp.Current * Magnitude;

            double resistance = target.GetComponent<ModifierComponent>()
                ?.ApplyApplicableModifiers(0.0, [Tags.Resistance, Tags.Prune], target.GetTags()) ?? 0.0;
            resistance = Math.Min(resistance, Stats.MaxResistances);
            damage *= (1.0 - resistance);

            hpComp.ApplyDamage(damage);
            DamageDone = damage;
        }
    }
}
