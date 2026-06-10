using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Skills.SkillEffects;

namespace TheIdleScrolls_Core.Skills.SkillEffectGenerators
{
    public class DoTSkillEffectGenerator(DamageType damageType, double dps) : ISkillEffectGenerator
    {
        public DamageType DamageType = damageType;
        public double DPS { get; set; } = dps;
        public string Description => $"Deal {DPS:0.##} damage per second";

        public void Reset()
        {
            
        }

        public List<SkillEffectBundle> Update(double dt)
        {
            SkillEffectBundle bundle = new SkillEffectBundle
            {
                Target = TargetingMode.SingleEnemy,
                Effects = [new DamageSkillEffect(DamageType, dt * DPS, [Tags.DamageOverTime])]
            };
            return [bundle];
        }
    }
}
