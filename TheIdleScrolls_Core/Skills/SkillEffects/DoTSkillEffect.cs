using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Definitions;
using static TheIdleScrolls_Core.Skills.ISkillEffect;

namespace TheIdleScrolls_Core.Skills.SkillEffects
{
    public class DoTSkillEffect(DamageType damageType, double damage, double duration, TargetingMode targetingMode, HashSet<string> tags) : ISkillEffect
    {
        public DamageType DamageType { get; } = damageType;
        public double TotalDamage { get; } = damage;
        public double Duration { get; } = duration;
        public HashSet<string> Tags { get; } = tags;
        public TargetingMode Target { get; } = targetingMode;
        public int StackLimit { get; set; } = int.MaxValue;
        public string Description => $"{TotalDamage:0.#} {DamageType.ToTag()} damage over " +
            $"{Duration:0.#}s to {(Target == TargetingMode.Self ? "self" : "target")}";

        public void ApplyToTarget(Entity target)
        {
            if (Duration == 0.0)
                return;

            double resistance = target.GetComponent<ModifierComponent>()
                ?.ApplyApplicableModifiers(0.0, [Definitions.Tags.Resistance, .. DamageType.GetMatchingTags(), .. Tags], target.GetTags()) ?? 0.0;
            resistance = Math.Min(resistance, Stats.MaxResistances);
            double damage = TotalDamage * (1.0 - resistance / 100.0);

            var dotComp = target.GetComponent<DoTComponent>();
            if (dotComp is null)
            {
                dotComp = new();
                target.AddComponent(dotComp);
            }
            dotComp.Add(new(DamageType, damage / Duration, Duration), StackLimit);
        }
    }
}
