using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using static TheIdleScrolls_Core.Skills.ISkillEffect;

namespace TheIdleScrolls_Core.Skills.SkillEffects
{
    public class DoTSkillEffect(DamageType damageType, double damage, double duration, HashSet<string> tags) : ISkillEffect
    {
        public DamageType DamageType { get; } = damageType;
        public double TotalDamage { get; } = damage;
        public double Duration { get; } = duration;
        public HashSet<string> Tags { get; } = tags;
        public int StackLimit { get; set; } = int.MaxValue;
        public string Description => $"{TotalDamage:0.#} {DamageType.ToTag()} damage over {Duration:0.#}s";

        public List<ISkillEffectOutcome> ApplyToTarget(Entity target)
        {
            if (Duration == 0.0)
                return [];

            // Consider global damage taken modifiers first
            // Currently, this is only used to 'inject' the damage reduction from blocking for the duration of 
            // the evaluation of a skill effect bundle.
            double tmpDamage = TotalDamage * target.ApplyAllApplicableModifiers(1.0, [Definitions.Tags.DamageTakenMultiplier], target.GetTags());

            double resistance = target.GetComponent<ModifierComponent>()
                ?.ApplyApplicableModifiers(0.0, [Definitions.Tags.Resistance, .. DamageType.GetMatchingTags(), .. Tags], target.GetTags()) ?? 0.0;
            resistance = Math.Min(resistance, Stats.MaxResistances);
            double damage = tmpDamage * (1.0 - resistance / 100.0);

            var dotComp = target.GetComponent<DoTComponent>();
            if (dotComp is null)
            {
                dotComp = new();
                target.AddComponent(dotComp);
            }
            dotComp.Add(new(DamageType, damage / Duration, Duration), StackLimit);

            List<ISkillEffectOutcome> outcomes = [new DamageApplied(target, DamageType, damage)]; // CornerCut: Treat hits and dots the same for now
            if (damage < TotalDamage)
            {
                outcomes.Add(new DamagePrevented(target, DamageType, TotalDamage - damage));
            }
            return outcomes;
        }
    }
}
