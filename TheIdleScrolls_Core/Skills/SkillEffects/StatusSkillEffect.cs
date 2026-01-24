using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.StatusEffects;
using static TheIdleScrolls_Core.Skills.ISkillEffect;

namespace TheIdleScrolls_Core.Skills.SkillEffects
{
    public class StatusSkillEffect(TargetingMode targetingMode, StatusEffect effect) : ISkillEffect
    {
        public string Description
        {
            get
            {
                string result = $"Apply to {(targetingMode == TargetingMode.Self ? "self" : "target")}:";
                foreach (string line in effect.Description.Split('\n'))
                {
                    result += $"\n\t{line}";
                }
                return result;
            }
        }

        public TargetingMode Target => targetingMode;

        public void ApplyToTarget(Entity target)
        {
            effect.ActivateOnEntity(target);
        }
    }
}
