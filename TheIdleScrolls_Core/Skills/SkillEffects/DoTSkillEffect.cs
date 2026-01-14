using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        public string Description => $"{TotalDamage:0.#} {DamageType.ToTag()} damage over " +
            $"{Duration:0.#}s to {(Target == TargetingMode.Self ? "self" : "target")}";

        public TargetingMode Target => targetingMode;

        public void ApplyToTarget(Entity target)
        {
            throw new NotImplementedException();
        }
    }
}
