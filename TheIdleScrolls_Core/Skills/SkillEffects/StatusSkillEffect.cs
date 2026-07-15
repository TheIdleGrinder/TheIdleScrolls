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
    public class StatusSkillEffect(StatusEffect effect) : ISkillEffect
    {
        public string Description
        {
            get
            {
                return effect.Description;
            }
        }

        public List<ISkillEffectOutcome> ApplyToTarget(Entity target)
        {
            return effect.ActivateOnEntity(target);
        }
    }
}
