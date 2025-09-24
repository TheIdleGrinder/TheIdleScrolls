using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public class LifeRegenerationComponent : IComponent
    {
        double _RegPerSecond = 0.0;

        public double RegenerationPerSecond => _RegPerSecond;

        public void AddRegeneration(double amount)
        {
            _RegPerSecond += amount;
        }

        public void RemoveRegeneration(double amount)
        {
            _RegPerSecond = Math.Max(0.0, _RegPerSecond - amount);
        }
    }
}
