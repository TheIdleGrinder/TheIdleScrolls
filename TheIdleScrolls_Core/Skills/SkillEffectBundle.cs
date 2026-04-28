using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // TODO: Handle accuracy vs evasion

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
