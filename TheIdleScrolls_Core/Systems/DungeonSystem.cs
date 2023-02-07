using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Systems
{
    public class DungeonSystem : AbstractSystem
    {
        Entity? m_player = null;

        int m_wildernessLevel = 1;

        int m_highestWilderness = 0; // Used to determine whether accessible dungeons need to be checked again
        int m_dungeonsDone = 0;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == null)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
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
                            coordinator.PostMessage(this, new DungeonOpenedMessage(dungeon.Name.Localize()));
                        }
                    }
                }
            }

            // Handle requests
            // Note current zone, if in wilderness
            // Send travel request
            var request = coordinator.FetchMessagesByType<EnterDungeonRequest>().LastOrDefault();
            if (request != null)
            {
                if (!world.IsInDungeon())
                {
                    m_wildernessLevel = world.Zone.Level;
                }
                coordinator.PostMessage(this, new TravelRequest(request.DungeonId, 0));
                return;
            }

            // Leave dungeon if requested
            if (coordinator.MessageTypeIsOnBoard<LeaveDungeonRequest>())
            {
                coordinator.PostMessage(this, new TravelRequest("", m_wildernessLevel));
            }

            // In dungeon?
                // Time expired => return to wilderness
                // Mob defeated => check remaining
                    // No mobs remaining => check floor
                        // Next floor exists => send travel request
                        // No next floor
                            // Update PlayerProgress
                            // Give reward
                            // return to wilderness
            if (world.IsInDungeon())
            {
                if (coordinator.MessageTypeIsOnBoard<BattleLostMessage>())
                {
                    coordinator.PostMessage(this, new TravelRequest("", m_wildernessLevel));
                    return;
                }

                var kills = coordinator.FetchMessagesByType<DeathMessage>().Count;
                world.RemainingEnemies -= kills;
                if (world.RemainingEnemies <= 0)
                {
                    if (world.AreaKingdom.GetDungeonFloorCount(world.DungeonId) > world.DungeonFloor + 1) // There are more floors
                    {
                        coordinator.PostMessage(this, new TravelRequest(world.DungeonId, world.DungeonFloor + 1));
                    }
                    else // Dungeon cleared
                    {
                        coordinator.PostMessage(this, new DungeonCompletedMessage(world.DungeonId));
                        coordinator.PostMessage(this, new TravelRequest(world.DungeonId, 0));
                    }
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

        public DungeonCompletedMessage(string dungeonId)
        {
            DungeonId = dungeonId;
        }

        string IMessage.BuildMessage()
        {
            return $"Dungeon '{DungeonId}' completed";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.High;
        }
    }

    record LootTableEntry(string Item, double Weight);
}
