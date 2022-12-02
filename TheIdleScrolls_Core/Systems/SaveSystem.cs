using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Storage;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Systems
{
    public class SaveSystem : AbstractSystem
    {
        DataAccessHandler m_dataAccessHandler;

        Cooldown m_cooldown = new(10.0);

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            bool trigger = m_cooldown.Update(dt) > 0;
            if (trigger)
            {
                Entity? player = coordinator.GetEntities().Where(e => e.HasComponent<PlayerComponent>()).FirstOrDefault();
                if (player != null)
                {
                    m_dataAccessHandler.StoreEntity(player);
                    coordinator.PostMessage(this, new TextMessage("Game saved"));
                }
            }
        }

        public SaveSystem(DataAccessHandler dataHandler, double cooldown = 10.0)
        {
            m_dataAccessHandler = dataHandler;
            m_cooldown = new(cooldown);
        }
    }
}
