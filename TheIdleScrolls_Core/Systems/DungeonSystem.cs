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
        bool m_firstUpdate = true;

        Entity? m_player = null;

        int m_wildernessLevel = 1;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == null)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;

            // Check accessible dungeons
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<DeathMessage>())
            {
                var progComp = m_player.GetComponent<PlayerProgressComponent>();
                var travelComp = m_player.GetComponent<TravellerComponent>();
                if (progComp != null && travelComp != null)
                {
                    int progLevel = progComp.Data.HighestWildernessKill;
                    foreach (var dungeon in world.AreaKingdom.Dungeons)
                    {
                        if (progLevel >= dungeon.Level && !travelComp.AvailableDungeons.Contains(dungeon.Id))
                        {
                            travelComp.AvailableDungeons.Add(dungeon.Id);
                            coordinator.PostMessage(this, new DungeonOpenedMessage(dungeon.Id));
                        }
                    }
                    m_firstUpdate = false;
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
                        coordinator.PostMessage(this, new DungeonClearedMessage(world.DungeonId));
                        coordinator.PostMessage(this, new TravelRequest("", m_wildernessLevel));
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
            List<string> ownedItems = invItems.Concat(equipItems).Select(i => i.GetComponent<ItemComponent>()?.GenusName ?? "").ToList(); // CornerCut: Only looks at genus, ignoring quality etc.

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

    record LootTableEntry(string Item, double Weight);
}
