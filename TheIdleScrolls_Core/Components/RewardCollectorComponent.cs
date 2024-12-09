using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public class RewardCollectorComponent : IComponent
    {
        readonly HashSet<string> Collected = [];

        public void Collect(string reward)
        {
            Collected.Add(reward);
        }

        public bool HasCollected(string reward)
        {
            return Collected.Contains(reward);
        }
    }
}
