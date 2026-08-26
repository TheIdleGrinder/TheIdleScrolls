using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Skills.SkillEffects
{
    public class HealingSkillEffect(double amount, HashSet<string> tags) : ISkillEffect
    {
        public double Amount { get; set; } = Functions.ApplyDamageRounding(amount);
        public HashSet<string> Tags { get; set; } = tags;

        public string Description => $"Heal {Amount} HP";

        public List<ISkillEffectOutcome> ApplyToTarget(Entity target)
        {
            var hpComp = target.GetComponent<LifePoolComponent>();
            if (hpComp is null)
            {
                return [];
            }

            double tmpHealing = Amount;

            double before = hpComp.Current;
            hpComp.AddPoints(tmpHealing);
            double gained = hpComp.Current - before;
            return [new HealingApplied(target, gained)];
        }
    }
}
