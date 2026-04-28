using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Skills.SkillEffects;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Skills.SkillEffectGenerators
{
    public class ChargingDamageSkillEffectGenerator(DamageSkillEffect effect, double chargeTime) : ISkillEffectGenerator
    {
        DamageSkillEffect Effect { get; set; } = effect;
        public Cooldown Cooldown { get; set; } = new(chargeTime);

        public string Description => $"Deal {Effect.Description} every {Cooldown.Duration:0.#}";

        public void Reset()
        {
            Cooldown.Reset();
        }

        public List<SkillEffectBundle> Update(double dt)
        {
            return (Cooldown.Update(dt) > 0) ? [new SkillEffectBundle([Effect], TargetingMode.SingleEnemy)] : []; // CornerCut: Only one trigger per frame
        }
    }
}
