using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Systems
{
    public class DungeonSystem : AbstractSystem
    {
        Entity? m_player = null;
        bool m_firstUpdate = true;

        int m_highestWilderness = 0; // Used to determine whether accessible dungeons need to be checked again
        int m_dungeonsDone = 0;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;
            var locationComp = m_player.GetComponent<LocationComponent>();
            if (locationComp == null)
                return;

            var progLevel = m_player.GetComponent<PlayerProgressComponent>()?.Data.HighestWildernessKill ?? 0;
            var dungeonsDone = m_player.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeons().Count ?? 0;
            if (progLevel > m_highestWilderness || dungeonsDone > m_dungeonsDone) // CornerCut: Assumes that only wilderness progress and dungeon clear unlock dungeons
            {
                var travelComp = m_player.GetComponent<TravellerComponent>();
                if (travelComp != null)
                {
                    m_highestWilderness = progLevel;
                    m_dungeonsDone = dungeonsDone;
                    foreach (var dungeon in world.AreaKingdom.Dungeons)
                    {
                        if (travelComp.AvailableDungeons.Contains(dungeon.Name))
                            continue;
                        string condition = dungeon.Condition;
                        if (condition == String.Empty)
                        {
                            condition = $"Wilderness >= {dungeon.Level}"; // Default condition: Wilderness >= Dungeon level
                        }
                        var condExpression = ExpressionParser.Parse(condition);
                        if (condExpression.Evaluate(m_player, world) >= 1.0)
                        {
                            travelComp.AvailableDungeons.Add(dungeon.Name);
                            if (!m_firstUpdate)
                                coordinator.PostMessage(this, new DungeonOpenedMessage(dungeon.Name.Localize()));
                        }
                    }
                }
            }
            m_firstUpdate = false;

            // Handle requests
            // Note current zone, if in wilderness
            // Send travel request
            bool floorChanged = false;
            var request = coordinator.FetchMessagesByType<EnterDungeonRequest>().LastOrDefault();
            if (request != null)
            {
                if (world.Map.GetDungeonsAtLocation(locationComp.CurrentLocation).Any(d => d.Name == request.DungeonId))
                {
                    locationComp.EnterDungeon(request.DungeonId);
                    floorChanged = true;
                    
                    coordinator.PostMessage(this, 
                        new AreaChangedMessage(m_player, locationComp.CurrentLocation, request.DungeonId, 0, AreaChangeType.EnteredDungeon));
                }
                else
                {
                    coordinator.PostMessage(this, 
                        new TextMessage($"Dungeon {request.DungeonId.Localize()} is not at the player's location", 
                        IMessage.PriorityLevel.High));
                }
            }

            // Leave dungeon if requested
            if (coordinator.MessageTypeIsOnBoard<LeaveDungeonRequest>())
            {
                locationComp.LeaveDungeon();
                coordinator.PostMessage(this, 
                    new AreaChangedMessage(m_player, locationComp.CurrentLocation, string.Empty, -1, AreaChangeType.LeftDungeon));
            }

            // In dungeon?
                // Time expired => return to wilderness
                // Mob defeated => check remaining
                    // No mobs remaining => check floor
                        // Next floor exists => move there
                        // No next floor
                            // Update PlayerProgress
                            // Give reward
                            // return to wilderness
            if (locationComp.InDungeon)
            {

                if (coordinator.MessageTypeIsOnBoard<BattleLostMessage>())
                {
                    locationComp.LeaveDungeon();
                    coordinator.PostMessage(this,
                        new AreaChangedMessage(m_player, locationComp.CurrentLocation, string.Empty, -1, AreaChangeType.LeftDungeon));
                    return;
                }

                var kills = coordinator.FetchMessagesByType<DeathMessage>().Count;
                locationComp.RemainingEnemies -= kills;
                if (locationComp.RemainingEnemies <= 0)
                {
                    if (world.AreaKingdom.GetDungeonFloorCount(locationComp.DungeonId) > locationComp.DungeonFloor + 1) // There are more floors
                    {
                        locationComp.DungeonFloor += 1;
                        floorChanged = true;
                        coordinator.PostMessage(this,
                            new AreaChangedMessage(m_player, locationComp.CurrentLocation, 
                                locationComp.DungeonId, locationComp.DungeonFloor, AreaChangeType.FloorChange));
                    }
                    else // Dungeon cleared
                    {
                        bool first = !m_player.GetComponent<PlayerProgressComponent>()?.Data.DungeonTimes.ContainsKey(locationComp.DungeonId) ?? true;
                        coordinator.PostMessage(this, new DungeonCompletedMessage(locationComp.DungeonId, first));
                        locationComp.EnterDungeon(locationComp.DungeonId);
                        floorChanged = true;
                        coordinator.PostMessage(this,
                            new AreaChangedMessage(m_player, locationComp.CurrentLocation, locationComp.DungeonId, 0, AreaChangeType.EnteredDungeon));
                    }
                }

                if (locationComp.InDungeon && floorChanged)
                {
                    var zone = world.Map.GetDungeonZone(locationComp.DungeonId, locationComp.DungeonFloor);
                    if (zone == null)
                    {
                        throw new Exception($"Player entered invalid dungeon: {locationComp.DungeonId}");
                    }
                    locationComp.RemainingEnemies = zone.MobCount;
                }
            }
        }
    }

    class EnterDungeonRequest : IMessage
    {
        public string DungeonId { get; set; }
        public EnterDungeonRequest(string dungeonId)
        {
            DungeonId = dungeonId;
        }

        string IMessage.BuildMessage()
        {
            return $"Request: Enter dungeon '{DungeonId}'";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }

    class LeaveDungeonRequest : IMessage
    {
        string IMessage.BuildMessage()
        {
            return "Request: Leave Dungeon";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }

    class DungeonOpenedMessage : IMessage
    {
        public string DungeonId { get; set; }

        public DungeonOpenedMessage(string dungeonId)
        {
            DungeonId = dungeonId;
        }

        string IMessage.BuildMessage()
        {
            return $"Dungeon '{DungeonId}' is now open";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.High;
        }
    }

    class DungeonCompletedMessage : IMessage
    {
        public string DungeonId { get; set; }

        public bool FirstCompletion { get; set; }

        public DungeonCompletedMessage(string dungeonId, bool firstCompletion)
        {
            DungeonId = dungeonId;
            FirstCompletion = firstCompletion;
        }

        string IMessage.BuildMessage()
        {
            return $"Dungeon '{DungeonId.Localize()}' completed" + ((FirstCompletion) ? " for the first time" : "");
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.High;
        }
    }

    record LootTableEntry(string Item, double Weight);
}
