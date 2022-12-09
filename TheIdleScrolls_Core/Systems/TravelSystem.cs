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
        bool m_autoProceed = false;

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

            var autoProcRequest = coordinator.FetchMessagesByType<AutoProceedRequest>().LastOrDefault(); // Consider only most recent
            if (autoProcRequest != null)
            {
                m_autoProceed = autoProcRequest.AutoProceed;
            }

            var travelRequest = coordinator.FetchMessagesByType<TravelRequest>().LastOrDefault();
            if (travelRequest != null)
            {
                Travel(travelRequest.AreaLevel, world, coordinator);
                return;
            }

            // Player lost battle
            if (coordinator.MessageTypeIsOnBoard<BattleLostMessage>()) 
            {
                if (world.AreaLevel > 1)
                    Travel(world.AreaLevel - 1, world, coordinator);
            }
            else if (coordinator.MessageTypeIsOnBoard<DeathMessage>() && m_autoProceed)
            {
                Travel(world.AreaLevel + 1, world, coordinator);
            }
        }

        void Travel(int areaLevel, World world, Coordinator coordinator)
        {
            coordinator.GetEntities<MobComponent>().ForEach(e => coordinator.RemoveEntity(e.Id));
            world.AreaLevel = areaLevel;
            world.TimeLimit.Reset();
            coordinator.PostMessage(this, new TravelMessage("Wilderness", areaLevel));
        }
    }

    public class TravelMessage : IMessage 
    { 
        public string AreaName { get; set; }
        public int AreaLevel { get; set; }

        public TravelMessage(string areaName, int areaLevel)
        {
            AreaName = areaName;
            AreaLevel = areaLevel;
        }

        string IMessage.BuildMessage()
        {
            return $"Travelled to {AreaName} (Level {AreaLevel})";
        }
    }

    public class TravelRequest : IMessage
    {
        public int AreaLevel { get; set; }

        public TravelRequest(int areaLevel)
        {
            AreaLevel = areaLevel;
        }

        string IMessage.BuildMessage()
        {
            return $"Request: Travel to area with level {AreaLevel}";
        }
    }

    public class AutoProceedRequest : IMessage
    {
        public bool AutoProceed { get; set; }

        public AutoProceedRequest(bool autoProceed)
        {
            AutoProceed = autoProceed;
        }

        string IMessage.BuildMessage()
        {
            return $"Request: {(AutoProceed ? "A" : "Dea")}ctivate automatic proceeding";
        }
    }
}
