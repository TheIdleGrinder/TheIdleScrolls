using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Components
{
    public class EvaderComponent : IComponent
    {
        public bool Active { get; set; } = false;
        public double ChargeDuration { get; set; } = Definitions.Stats.MaxEvasionChargeDuration;
        public double EvasionDuration { get; set; } = 0.0;
        public Cooldown Duration { get; set; } = new Cooldown(Definitions.Stats.MaxEvasionChargeDuration);

        // Usually, evasion prevents 100% of damage, but on the first and last frame of the evasion,
        // it only prevents a fraction of the damage. This reduces variance due to different frame durations.
        public double Prevention { get; set; } = 0.0;

        public EvaderComponent()
        {
            Duration.SingleShot = true;
        }
    }
}
