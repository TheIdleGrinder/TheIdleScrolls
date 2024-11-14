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
using TheIdleScrolls_Core.Resources;
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
            var dungeonsDone = m_player.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeonLevels().Count ?? 0;
            // CornerCut: Assumes that only wilderness progress and dungeon clear unlock dungeons
            if (progLevel > m_highestWilderness || dungeonsDone > m_dungeonsDone)
            {
                var travelComp = m_player.GetComponent<TravellerComponent>();
                if (travelComp != null)
                {
                    m_highestWilderness = progLevel;
                    m_dungeonsDone = dungeonsDone;
                    UpdateAvailableDungeons(m_player, world, coordinator);
                }
            }
            m_firstUpdate = false;

            // Handle requests
            // Note current zone, if in wilderness
            // Send travel request
            var request = coordinator.FetchMessagesByType<EnterDungeonRequest>().LastOrDefault();
            if (request != null)
            {
                var dungeon = world.Map.GetDungeonsAtLocation(locationComp.CurrentLocation).Where(d => d.Id == request.DungeonId).FirstOrDefault();
                if (world.Map.GetDungeonsAtLocation(locationComp.CurrentLocation).Any(d => d.Id == request.DungeonId))
                {
                    if (dungeon != null && dungeon.AvailableLevels(m_player, world).Contains(request.Level))
                    {
                        locationComp.EnterDungeon(request.DungeonId, request.Level);
                        coordinator.PostMessage(this, 
                            new AreaChangedMessage(m_player, locationComp.CurrentLocation, request.DungeonId, 0, AreaChangeType.EnteredDungeon));
                    }
                    else
                    {
                        coordinator.PostMessage(this, 
                            new TextMessage($"Dungeon {DungeonList.GetDungeon(request.DungeonId)?.Name ?? "??"} level {request.Level} is not accessible", 
                            IMessage.PriorityLevel.High));
                    }
                }
                else
                {
                    coordinator.PostMessage(this, 
                        new TextMessage($"Dungeon {DungeonList.GetDungeon(request.DungeonId)?.Name ?? "??"} is not at the player's location", 
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
                var battleState = m_player.GetComponent<BattlerComponent>()?.Battle?.State ?? Battle.BattleState.InProgress;
                if (battleState == Battle.BattleState.PlayerLost)
                {
                    locationComp.LeaveDungeon();
                    coordinator.PostMessage(this,
                        new AreaChangedMessage(m_player, locationComp.CurrentLocation, string.Empty, -1, AreaChangeType.LeftDungeon));
                    return;
                }

                if (battleState == Battle.BattleState.PlayerWon)
                {
                    if (world.AreaKingdom.GetDungeonFloorCount(locationComp.DungeonId) > locationComp.DungeonFloor + 1) // There are more floors
                    {
                        locationComp.DungeonFloor += 1;
                        coordinator.PostMessage(this,
                            new AreaChangedMessage(m_player, locationComp.CurrentLocation, 
                                locationComp.DungeonId, locationComp.DungeonFloor, AreaChangeType.FloorChange));
                    }
                    else // Dungeon cleared
                    {
                        bool first = !m_player.GetComponent<PlayerProgressComponent>()?.Data.DungeonTimes.ContainsKey(locationComp.DungeonId) ?? true;
                        coordinator.PostMessage(this, 
                            new DungeonCompletedMessage(locationComp.DungeonId, 
                                                        locationComp.DungeonLevel, 
                                                        first));
                        locationComp.EnterDungeon(locationComp.DungeonId, locationComp.DungeonLevel);
                        coordinator.PostMessage(this,
                            new AreaChangedMessage(m_player, locationComp.CurrentLocation, locationComp.DungeonId, 0, AreaChangeType.EnteredDungeon));
                        // Update available dungeons in case something opened up
                        UpdateAvailableDungeons(m_player, world, coordinator);
                    }
                }
            }
        }

        void UpdateAvailableDungeons(Entity player, World world, Coordinator coordinator)
        {
            var travelComp = player.GetComponent<TravellerComponent>();
            if (travelComp == null)
                return;
            Dictionary<string, int[]> nowAvailable = [];
            foreach (var dungeon in world.AreaKingdom.Dungeons)
            {
                var previouslyAvailable = travelComp.AvailableDungeons.GetValueOrDefault(dungeon.Id, []);
                var availableLevels = dungeon.AvailableLevels(player, world);
                if (availableLevels.Length == 0)
                {
                    continue;
                }
                nowAvailable[dungeon.Id] = availableLevels;

                // Skip messages on initial update or if there is nothing new to report
                // CornerCut: Theoretically a new dungeon could unlock while a previously available one closes
                if (m_firstUpdate || availableLevels.Length <= previouslyAvailable.Length)
                {
                    continue;
                }
                
                foreach (var level in availableLevels)
                {
                    if (!previouslyAvailable.Contains(level))
                    {
                        coordinator.PostMessage(this, new DungeonOpenedMessage(dungeon.Id, level));
                    }
                }
            }
            travelComp.AvailableDungeons = nowAvailable;
        }
    }

    record EnterDungeonRequest(string DungeonId, int Level) : IMessage
    {
        string IMessage.BuildMessage() => $"Request: Enter dungeon '{DungeonId}' (Level {Level})";

        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }

    class LeaveDungeonRequest : IMessage
    {
        string IMessage.BuildMessage() => "Request: Leave Dungeon";

        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }

    record DungeonOpenedMessage(string DungeonId, int Level) : IMessage
    {
        string IMessage.BuildMessage() => $"Dungeon '{DungeonList.GetDungeon(DungeonId)?.Name ?? "??"}' (Level {Level}) is now open";

        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.High;
    }

    record DungeonCompletedMessage(string DungeonId, int DungeonLevel, bool FirstCompletion) : IMessage
    {
        string IMessage.BuildMessage()
        {
            return $"Dungeon '{DungeonList.GetDungeon(DungeonId)?.Name ?? "??"}' (Level {DungeonLevel}) completed" 
                + ((FirstCompletion) ? " for the first time" : "");
        }

        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.High;
    }

    record LootTableEntry(string Item, double Weight);
}
