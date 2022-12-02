using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Systems
{
    public abstract class AbstractSystem : ISystem
    {
        /*Logger? m_logger;

        public void SetLogger(Logger logger)
        {
            m_logger = logger;
        }

        protected Logger? Logger => m_logger;
*/
        public abstract void Update(World world, Coordinator coordinator, double dt);
    }
}
