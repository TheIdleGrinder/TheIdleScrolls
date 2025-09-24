using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.StatusEffects
{
    public class LifeRegenerationEffect(double amountPerSec, double duration) : StatusEffect("Regeneration", duration)
    {
        public double Duration { get; init; } = duration;
        public double Amount { get; init; } = amountPerSec;
        public override string Description => $"Regenerates {Amount:0} life per second over {Duration:0.##} seconds.";

        protected override void ActivateEffect(Entity target)
        {
            var lifeRegenComp = target.GetComponent<LifeRegenerationComponent>();
            if (lifeRegenComp is null)
            {
                lifeRegenComp = new LifeRegenerationComponent();
                target.AddComponent(lifeRegenComp);
            }
            lifeRegenComp.AddRegeneration(Amount / Duration);
        }

        protected override void DeactivateEffect(Entity target)
        {
            var lifeRegComp = target.GetComponent<LifeRegenerationComponent>();
            if (lifeRegComp is null)
            {
                return;
            }
            lifeRegComp.RemoveRegeneration(Amount / Duration);
            if (lifeRegComp.RegenerationPerSecond <= 0.0)
            {
                target.RemoveComponent<LifeRegenerationComponent>();
            }
        }
    }
}
