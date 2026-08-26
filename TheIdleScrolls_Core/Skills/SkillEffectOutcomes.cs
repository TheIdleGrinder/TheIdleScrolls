using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.StatusEffects;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Skills
{
    public record DamageApplied(Entity Target, DamageType Type, double Amount) : ISkillEffectOutcome
    {
        public string Description => $"{Target.GetName()} took {Amount} {Type} damage";
    }

    public record DamageClusterApplied(Entity Target, DamageCluster Damage) : ISkillEffectOutcome
    {
        public string Description => $"{Target.GetName()} took {Damage.TotalDamage:0.##} damage";
    }

    public record DamagePrevented(Entity Target, DamageType Type, double Amount) : ISkillEffectOutcome
    {
        public string Description => $"{Target.GetName()} prevented {Amount} {Type} damage";
    }

    public record DamageClusterPrevented(Entity Target, DamageCluster Damage) : ISkillEffectOutcome
    {
        public string Description => $"{Target.GetName()} prevented {Damage.TotalDamage:0.##} damage";
    }

    public record StatusEffectApplied(Entity Target, StatusEffect Effect, double Effectiveness) : ISkillEffectOutcome
    {
        public string Description => $"{Target.GetName()} had {Effect.Name} applied with {Effectiveness:0.##%} effectiveness";
    }

    public record StatusEffectPrevented(Entity Target, StatusEffect Effect, double Prevention) : ISkillEffectOutcome
    {
        public string Description => $"{Target.GetName()} prevented {Prevention:0.##%} of {Effect.Name}";
    }

    public record HitEvaded(Entity Target) : ISkillEffectOutcome
    {
        public string Description => $"{Target.GetName()} evaded a hit";
    }

    public record HitBlocked(Entity Target, double PreventionPercentage) : ISkillEffectOutcome
    {
        public string Description => $"{Target.GetName()} blocked {PreventionPercentage:0.##%} of a hit's damage";
    }

    public record HealingApplied(Entity Target, double Amount) : ISkillEffectOutcome
    {
        public string Description => $"{Target.GetName()} was healed for {Amount} HP";
    }
}
