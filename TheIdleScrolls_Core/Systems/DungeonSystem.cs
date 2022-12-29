using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Systems
{
    internal class DungeonSystem : AbstractSystem
    {
        Entity? m_player = null;

        int m_wildernessLevel = 1;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == null)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;

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
                        coordinator.PostMessage(this, new DungeonClearedMessage(world.DungeonId));
                        coordinator.PostMessage(this, new TravelRequest("", m_wildernessLevel));
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
    }

    class DungeonClearedMessage : IMessage
    {
        public string DungeonId { get; set; }

        public DungeonClearedMessage(string dungeonId)
        {
            DungeonId = dungeonId;
        }

        string IMessage.BuildMessage()
        {
            return $"Dungeon '{DungeonId}' cleared";
        }
    }
}
