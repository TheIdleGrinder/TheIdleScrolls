using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.StatusEffects;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Components
{
    public class MomentumComponent : IComponent
    {
        public int Momentum => (int)PreciseMomentum;
        public int MaxMomentum { get; set; } = 10;
        public int MomentumAtLastUpdate { get; set; } = 0; // Used by system to determine if momentum has changed since last update
        public Cooldown Timer { get; } = new(2.0) { SingleShot = true };
        public List<Modifier> Modifiers { get; } = [];
        public bool IsChanged => MomentumAtLastUpdate != Momentum;
        public double GainOnAttackHit { get; set; } = 1.0;
        public double GainOnEvade { get; set; } = 0.0;
        public double GainOnBlock { get; set; } = 0.0;

        double PreciseMomentum { get; set; } = 0.0;

        public void AddMomentum(double amount)
        {
            PreciseMomentum += amount;
            if (Momentum > MaxMomentum)
            {
                PreciseMomentum = MaxMomentum;
            }
            Timer.Reset();
        }

        public void RemoveMomentum(double amount)
        {
            PreciseMomentum -= amount;
            if (PreciseMomentum < 0.0)
                PreciseMomentum = 0;
        }

        public void UpdateTimer(double dt)
        {
            Timer.Update(dt);
            if (Timer.HasFinished && PreciseMomentum > 0.0)
            {
                PreciseMomentum = 0.0;
            }
        }

        public void ScoreAttackHit() => AddMomentum(GainOnAttackHit);
        public void ScoreEvade() => AddMomentum(GainOnEvade);
        public void ScoreBlock() => AddMomentum(GainOnBlock);
        public void TakeHit() => RemoveMomentum(Math.Max(1.0, 0.2 * PreciseMomentum));

        public void MarkAsUpdated()
        {
            MomentumAtLastUpdate = Momentum;
        }
    }
}
