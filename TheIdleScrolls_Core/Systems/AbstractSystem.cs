using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Systems
{
    public abstract class AbstractSystem : ISystem
    {
        public abstract void Update(World world, Coordinator coordinator, double dt);
    }
}
