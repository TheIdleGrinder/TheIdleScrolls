using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Systems
{
    internal class DungeonSystem : AbstractSystem
    {
        Entity? m_player = null;

        int m_wildernessLevel = 1;

        int m_highestWilderness = 0; // Used to determine whether accessible dungeons need to be checked again

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == null)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;

            var progLevel = m_player.GetComponent<PlayerProgressComponent>()?.Data.HighestWildernessKill ?? 0;
            if (progLevel > m_highestWilderness)
            {
                var travelComp = m_player.GetComponent<TravellerComponent>();
                if (travelComp != null)
                {
                    m_highestWilderness = progLevel;
                    foreach (var dungeon in world.AreaKingdom.Dungeons)
                    {
                        if (progLevel >= dungeon.Level && !travelComp.AvailableDungeons.Contains(dungeon.Id))
                        {
                            travelComp.AvailableDungeons.Add(dungeon.Id);
                            coordinator.PostMessage(this, new DungeonOpenedMessage(dungeon.Id));
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
                        GiveDungeonReward(world, coordinator);
                        coordinator.PostMessage(this, new DungeonCompletedMessage(world.DungeonId));
                        //coordinator.PostMessage(this, new TravelRequest("", m_wildernessLevel));
                        coordinator.PostMessage(this, new TravelRequest(world.DungeonId, 0));
                    }
                }
            }
        }

        void GiveDungeonReward(World world, Coordinator coordinator)
        {
            // Build loot table
            // Get rewards from dungeon description
            List<LootTableEntry> lootTable = new();
            var rewards = world.AreaKingdom.GetDungeon(world.DungeonId)?.Rewards ?? new();
            if (rewards.Any())
            {
                foreach (var reward in rewards)
                {
                    lootTable.Add(new LootTableEntry(reward, 1.0));
                }
            }
            else
            {
                return;
            }

            // Select random reward
            double weightSum = lootTable.Sum(e => e.Weight);
            double pointer = new Random().NextDouble() * weightSum;
            string selection = "";
            foreach (var reward in lootTable)
            {
                if (reward.Weight > pointer)
                {
                    selection = reward.Item;
                    break;
                }
                pointer -= reward.Weight;
            }

            // Give item to player, if they don't have it already
            // TODO: Give some other kind of reward instead of duplicate items
            
            // Find already owned items
            var invItems = m_player?.GetComponent<InventoryComponent>()?.GetItems() ?? new();
            var equipItems = m_player?.GetComponent<EquipmentComponent>()?.GetItems() ?? new();
            List<string> ownedItems = invItems.Concat(equipItems).Select(i => i.GetComponent<ItemComponent>()?.Code.Code ?? "").ToList();

            if (!ownedItems.Contains(selection))
            {
                Entity item = new ItemFactory().ExpandCode(selection) ?? throw new Exception($"Invalid item code: {selection}");
                coordinator.AddEntity(item);
                coordinator.PostMessage(this, new ItemReceivedMessage(m_player!, item));
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
