using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Systems
{
    public class ApplicationUpdateSystem : AbstractSystem
    {
        IApplicationModel? m_appModel = null;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_appModel == null)
                return;

        }

        public void SetApplicationInterface(IApplicationModel? appInterface)
        {
            m_appModel = appInterface;
        }
    }
}
