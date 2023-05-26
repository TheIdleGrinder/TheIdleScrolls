using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;

namespace ConsoleRunner
{
    internal class ConsoleUpdater : IApplicationModel
    {
        public HashSet<IMessage.PriorityLevel> GetRelevantMessagePriorties()
        {
            return new()
            {
                IMessage.PriorityLevel.VeryHigh,
                IMessage.PriorityLevel.High,
                IMessage.PriorityLevel.Medium,
                IMessage.PriorityLevel.Low,
                IMessage.PriorityLevel.VeryLow,
                IMessage.PriorityLevel.Debug
            };
        }
    }
}
