using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Systems
{
    public class TravelSystem : AbstractSystem
    {
        bool m_firstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            var player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (player == null)
                return;

            var travelComp = player.GetComponent<TravellerComponent>();
            var progComp = player.GetComponent<PlayerProgressComponent>();
            var locationComp = player.GetComponent<LocationComponent>() 
                ?? throw new Exception("Player entity is missing required component: Location");

            if (m_firstUpdate)
            {
                m_firstUpdate = false;
            }

            // Update available areas
            if (travelComp != null && progComp != null)
            {
                if (progComp.Data.HighestWildernessKill >= travelComp.MaxWilderness)
                {
                    travelComp.MaxWilderness = progComp.Data.HighestWildernessKill + 1;
                    coordinator.PostMessage(this, new AreaUnlockedMessage(travelComp.MaxWilderness));
                }
            }

            var playerLvl = player.GetComponent<LevelComponent>()?.Level ?? 0;
            if (locationComp == null)
            {
                throw new Exception("Player entity is missing required component: Location");
            }
            var currentZone = locationComp.GetCurrentZone(world.Map);
            if (currentZone == null)
            {
                throw new Exception($"Player {player.GetName()} is not in a valid zone");
            }

            // Travel if player has no traveller component
            if (travelComp == null && playerLvl > (locationComp.GetCurrentZone(world.Map)?.Level ?? 0))
            {
                coordinator.PostMessage(this, new TravelRequest(world.Map.GetNextLocation(locationComp.CurrentLocation)!));
            }

            // Translate Battle lost => Travel one zone back
            if (coordinator.MessageTypeIsOnBoard<BattleLostMessage>() && !locationComp.InDungeon) // Player lost battle
            {
                coordinator.PostMessage(this, new SingleStepTravelRequest(false));
                coordinator.PostMessage(this, new AutoProceedRequest(false));
            }
            // Translate Death (victory) && auto proceed => Travel one zone forward
            if (coordinator.MessageTypeIsOnBoard<DeathMessage>() && (travelComp?.AutoProceed ?? false) && !locationComp.InDungeon)
            {
                coordinator.PostMessage(this, new SingleStepTravelRequest(true));
            }

            // Translate latest single-step travel to regular travel request
            var singleStepRequest = coordinator.FetchMessagesByType<SingleStepTravelRequest>().LastOrDefault();
            if (singleStepRequest != null)
            {
                Location? target = singleStepRequest.Forward
                    ? world.Map.GetNextLocation(locationComp.CurrentLocation)
                    : world.Map.GetPreviousLocation(locationComp.CurrentLocation);
                if (target != null)
                    coordinator.PostMessage(this, new TravelRequest(target));
            }

            var travelRequest = coordinator.FetchMessagesByType<TravelRequest>().LastOrDefault();
            if (travelRequest != null)
            {
                int limit = travelComp?.MaxWilderness ?? Int32.MaxValue;
                var zone = world.GetZone(travelRequest.Location);
                if (zone != null && zone.Level <= limit)
                {
                    if (locationComp.TravelToLocation(travelRequest.Location))
                    {
                        coordinator.PostMessage(this, new AreaChangedMessage(player, locationComp.CurrentLocation));
                    }
                }
            }

            // Handle changes to auto proceed status
            var autoProcRequest = coordinator.FetchMessagesByType<AutoProceedRequest>().LastOrDefault(); // Consider only most recent
            if (autoProcRequest != null && travelComp != null)
            {
                travelComp.AutoProceed = autoProcRequest.AutoProceed;
                coordinator.PostMessage(this, new AutoProceedStatusMessage(travelComp.AutoProceed));
            }

            // and do the same for auto grinding of dungeons
            var autoGrindRequest = coordinator.FetchMessagesByType<AutoGrindDungeonsRequest>().LastOrDefault(); // Consider only most recent
            if (autoGrindRequest != null && travelComp != null)
            {
                travelComp.AutoGrindDungeons = autoGrindRequest.AutoGrind;
                coordinator.PostMessage(this, new AutoGrindDungeonsStatusMessage(travelComp.AutoGrindDungeons));
            }
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

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Medium;
        }
    }

    public record AutoProceedStatusMessage(bool AutoProceed) : IMessage
    {
        string IMessage.BuildMessage() => $"Auto proceed status changed: {(AutoProceed ? "Enabled" : "Disabled")}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Low;
    }

    public record AutoGrindDungeonsStatusMessage(bool AutoProceed) : IMessage
    {
        string IMessage.BuildMessage() => $"Auto grind status changed: {(AutoProceed ? "Enabled" : "Disabled")}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Low;
    }

    public class TravelRequest : IMessage
    {
        public Location Location { get; set; }

        public TravelRequest(Location location)
        {
            Location = location;
        }

        public TravelRequest(int x, int y)
        {
            Location = new(x, y);
        }

        string IMessage.BuildMessage()
        {
            return $"Request: Travel to coordinates [{Location}]";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }

    public class SingleStepTravelRequest : IMessage
    {
        public bool Forward { get; set; }

        public SingleStepTravelRequest(bool forward)
        {
            Forward = forward;
        }

        string IMessage.BuildMessage()
        {
            return $"Request: Travel to {((Forward) ? "next" : "previous")} location";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }

    public record AutoProceedRequest(bool AutoProceed) : IMessage
    {
        string IMessage.BuildMessage() => $"Request: {(AutoProceed ? "A" : "Dea")}ctivate automatic proceeding";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }

    public record AutoGrindDungeonsRequest(bool AutoGrind) : IMessage
    {
        string IMessage.BuildMessage() => $"Request: {(AutoGrind ? "A" : "Dea")}ctivate automatic dungeon grinding";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }

    public class AreaUnlockedMessage : IMessage
    {
        public int Level = 0;

        public AreaUnlockedMessage(int level)
        {
            Level = level;
        }

        string IMessage.BuildMessage()
        {
            return $"Unlocked Wilderness Level {Level}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Low;
        }
    }
}
