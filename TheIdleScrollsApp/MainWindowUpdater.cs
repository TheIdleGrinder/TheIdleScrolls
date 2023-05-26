using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrollsApp
{
    internal class MainWindowUpdater : IApplicationModel
    {
        public HashSet<IMessage.PriorityLevel> GetRelevantMessagePriorties()
        {
            return new() { IMessage.PriorityLevel.VeryHigh, IMessage.PriorityLevel.High, IMessage.PriorityLevel.Medium };
        }
    }
}
