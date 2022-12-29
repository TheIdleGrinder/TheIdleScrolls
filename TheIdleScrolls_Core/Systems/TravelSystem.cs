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
            var player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (player == null)
                return;

            if (m_firstUpdate)
            {
                if (player.HasComponent<LevelComponent>())
                {
                    var level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                    int zoneNum = world.IsInDungeon() ? 0 : level;
                    Travel(world.DungeonId, zoneNum, world, coordinator);

                    m_firstUpdate = false;
                }

                coordinator.PostMessage(this, new AutoProceedStatusMessage(m_autoProceed)); // CornerCut: make info accessible to app
            }

            // Update available areas
            var travelComp = player.GetComponent<TravellerComponent>();
            var progComp = player.GetComponent<PlayerProgressComponent>();
            if (travelComp != null && progComp != null)
            {
                travelComp.MaxWilderness = progComp.Data.HighestWildernessKill + 1;
            }

            var playerLvl = player.GetComponent<LevelComponent>()?.Level ?? 0;

            // Travel if player has no traveller component
            if (travelComp == null && playerLvl != world.Zone.Level)
            {
                Travel("", playerLvl, world, coordinator);
            }

            var travelRequest = coordinator.FetchMessagesByType<TravelRequest>().LastOrDefault();
            if (travelRequest != null)
            {
                int limit = travelComp?.MaxWilderness ?? Int32.MaxValue;
                Travel(travelRequest.AreaId, Math.Min(travelRequest.ZoneNumber, limit), world, coordinator); // CornerCut: Checks limit for dungeons
            }
            else if (coordinator.MessageTypeIsOnBoard<BattleLostMessage>() && !world.IsInDungeon()) // Player lost battle
            {
                if (world.Zone.Level > 1)
                    Travel("", world.Zone.Level - 1, world, coordinator);
                coordinator.PostMessage(this, new AutoProceedRequest(false));
            }
            else if (coordinator.MessageTypeIsOnBoard<DeathMessage>() && m_autoProceed && !world.IsInDungeon())
            {
                Travel("", world.Zone.Level + 1, world, coordinator);
            }

            // Handle changes to auto proceed status
            var autoProcRequest = coordinator.FetchMessagesByType<AutoProceedRequest>().LastOrDefault(); // Consider only most recent
            if (autoProcRequest != null)
            {
                m_autoProceed = autoProcRequest.AutoProceed;
                coordinator.PostMessage(this, new AutoProceedStatusMessage(m_autoProceed));
            }
        }

        void Travel(string areaId, int zoneNumber, World world, Coordinator coordinator)
        {
            coordinator.GetEntities<MobComponent>().ForEach(e => coordinator.RemoveEntity(e.Id));
            world.Zone = world.AreaKingdom.GetZoneDescription(areaId, zoneNumber);
            string areaName = world.Zone.Name;
            world.DungeonId = areaId;
            world.DungeonFloor = zoneNumber;
            world.RemainingEnemies = world.Zone.MobCount;
            world.TimeLimit.Reset(world.TimeLimit.Duration * world.Zone.TimeMultiplier);
            coordinator.PostMessage(this, new TravelMessage(areaName, world.Zone.Level));
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

    public class AutoProceedStatusMessage : IMessage
    {
        public bool AutoProceed { get; set; }

        public AutoProceedStatusMessage(bool autoProceed)
        {
            AutoProceed = autoProceed;
        }

        string IMessage.BuildMessage()
        {
            return $"Auto proceed status changed: {(AutoProceed ? "Enabled" : "Disabled")}";
        }
    }

    public class TravelRequest : IMessage
    {
        public string AreaId { get; set; }
        public int ZoneNumber { get; set; }

        public TravelRequest(string areaId, int zoneNumber)
        {
            AreaId = areaId;
            ZoneNumber = zoneNumber;
        }

        string IMessage.BuildMessage()
        {
            return $"Request: Travel to area '{AreaId}' (#{ZoneNumber})";
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
