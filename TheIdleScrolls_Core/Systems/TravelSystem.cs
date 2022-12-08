using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Systems
{
    public class TravelSystem : AbstractSystem
    {
        bool m_autoProgress = true;

        bool m_firstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_firstUpdate)
            {
                var players = coordinator.GetEntities().Where(e => e.HasComponent<PlayerComponent>() && e.HasComponent<LevelComponent>());
                if (players.Any())
                {
                    var player = players.First();
                    var level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                    if (level > world.AreaLevel)
                    {
                        Travel(level, world, coordinator);
                    }
                    m_firstUpdate = false;
                }
            }

            // Player lost battle
            if (coordinator.MessageTypeIsOnBoard<BattleLostMessage>()) 
            {
                if (world.AreaLevel > 1)
                    Travel(world.AreaLevel - 1, world, coordinator);
                world.TimeLimit.Reset();
            }
            else if (coordinator.MessageTypeIsOnBoard<DeathMessage>() && m_autoProgress)
            {
                Travel(world.AreaLevel + 1, world, coordinator);
            }
        }

        void Travel(int areaLevel, World world, Coordinator coordinator)
        {
            coordinator.GetEntities<MobComponent>().ForEach(e => coordinator.RemoveEntity(e.Id));
            world.AreaLevel = areaLevel;
            world.TimeLimit.Reset();
            coordinator.PostMessage(this, new TextMessage($"Travelled to new area with level {areaLevel}"));
        }
    }

    
}
