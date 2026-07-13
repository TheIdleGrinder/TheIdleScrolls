using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Components
{
    public class BlockerComponent : IComponent
    {
        private Cooldown Cooldown { get; set; } = new Cooldown(Stats.BaseBlockCooldown) { SingleShot = true };

        public double RemainingCooldown => Cooldown.Remaining;
        public double CooldownDuration => Cooldown.Duration;
        public bool IsReady => Cooldown.HasFinished;
        public double BlockMitigation { get; set; } = Stats.BaseBlockMitigation;

        public void StartCooldown()
        {
            Cooldown.Reset();
        }

        public void UpdateCooldown(double dt)
        {
            Cooldown.Update(dt);
        }

        public void SetCooldownDuration(double duration)
        {
            Cooldown.ChangeDuration(duration);
        }
    }
}
