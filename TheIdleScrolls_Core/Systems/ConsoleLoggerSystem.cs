using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Systems
{
    public class ConsoleLoggerSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            coordinator.FetchAllMessages().ForEach(m => Console.WriteLine(m.Message));
        }
    }
}
