using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Systems
{
    public class TravelSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            var players = coordinator.GetEntities().Where(e => e.HasComponent<PlayerComponent>() && e.HasComponent<LevelComponent>());
            if (players.Any())
            {
                var player = players.First();
                var level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                if (level > world.AreaLevel)
                {
                    world.AreaLevel = level;
                    coordinator.PostMessage(this, new TextMessage($"Travelled to new area with level {level}"));
                }
            }
        }
    }
}
