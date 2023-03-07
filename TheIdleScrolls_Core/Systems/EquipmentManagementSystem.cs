using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Systems
{
    /// <summary>
    /// Responsible for moving items between an InventoryComponent and an EquipmentComponent
    /// </summary>
    public class EquipmentManagementSystem : AbstractSystem
    {

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            HashSet<Entity> changedInventories = new();

            foreach (var itemMessage in coordinator.FetchMessagesByType<ItemReceivedMessage>())
            {
                var inventoryComp = itemMessage.Recipient.GetComponent<InventoryComponent>();
                if (inventoryComp == null)
                {
                    Debug.WriteLine($"{itemMessage.Recipient.GetName()} has no InventoryComponent");
                    continue;
                }
                inventoryComp.AddItem(itemMessage.Item);
                changedInventories.Add(itemMessage.Recipient);
            }

            foreach (var move in coordinator.FetchMessagesByType<ItemMoveRequest>())
            {
                Entity? owner = coordinator.GetEntity(move.Owner);
                if (owner == null)
                    continue;
                var inventoryComp = owner.GetComponent<InventoryComponent>();
                var equipmentComp = owner.GetComponent<EquipmentComponent>();
                if (inventoryComp == null || equipmentComp == null)
                    throw new Exception($"Entity {owner.GetName()} lacks either Inventory or Equipment component");
                Entity item = coordinator.GetEntity(move.ItemId) ?? throw new Exception($"No item with id {move.ItemId}");
                if (move.Equip)
                {
                    bool inInventory = inventoryComp.RemoveItem(item);
                    if (inInventory)
                    {
                        var equippableComp = item.GetComponent<EquippableComponent>();
                        if (equippableComp == null)
                            continue;


                        if (!equipmentComp.CanEquipItem(item))
                        {
                            // Remove previous item from slot
                            var previousItem = equipmentComp.GetItemInSlot(equippableComp.Slot);
                            if (previousItem != null)
                            {
                                if (equipmentComp.UnequipItem(previousItem))
                                    inventoryComp.AddItem(previousItem);
                            }
                        }

                        bool couldEquip = equipmentComp.EquipItem(item);
                        if (!couldEquip)
                        {
                            inventoryComp.AddItem(item); // Put item back in inventory
                        }
                        else
                        {
                            coordinator.PostMessage(this, new ItemMovedMessage(owner, item, move.Equip));
                        }
                    }
                }
                else
                {
                    bool unequipped = equipmentComp.UnequipItem(item);
                    if (unequipped)
                    {
                        inventoryComp.AddItem(item);
                        coordinator.PostMessage(this, new ItemMovedMessage(owner, item, move.Equip));
                    }
                }
                changedInventories.Add(owner);
            }

            foreach (var sale in coordinator.FetchMessagesByType<SellItemRequest>())
            {
                Entity? owner = coordinator.GetEntity(sale.SellerId);
                if (owner == null)
                    continue;
                var inventoryComp = owner.GetComponent<InventoryComponent>();
                if (inventoryComp == null)
                    throw new Exception($"Entity {owner.GetName()} has no Inventory component");
                
                Entity item = coordinator.GetEntity(sale.ItemId) ?? throw new Exception($"No item with id {sale.ItemId}");
                if (inventoryComp.RemoveItem(item))
                {
                    var purseComp = owner.GetComponent<CoinPurseComponent>();
                    if (purseComp != null)
                    {
                        int value = item.GetComponent<ItemValueComponent>()?.Value ?? 0;
                        owner.GetComponent<CoinPurseComponent>()?.AddCoins(value);
                        coordinator.PostMessage(this, new CoinsChangedMessage(owner, value));
                    }
                    changedInventories.Add(owner);
                }
                else 
                {
                    throw new Exception($"{owner.GetName()} does not own item '{item.GetName()}'");
                }
            }

            foreach (var entity in changedInventories)
            {
                coordinator.PostMessage(this, new InventoryChangedMessage(entity));
            }
        }
    }

    public class ItemReceivedMessage : IMessage
    {
        public Entity Recipient { get; set; }
        public Entity Item { get; set; }

        public ItemReceivedMessage(Entity recipient, Entity item)
        {
            Recipient = recipient;
            Item = item;
        }

        string IMessage.BuildMessage()
        {
            return $"{Recipient.GetName()} received {Item.GetName()}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.VeryHigh;
        }
    }

    public class ItemMoveRequest : IMessage
    {
        public uint Owner { get; set; }

        public uint ItemId { get; set; }

        public bool Equip { get; set; }

        public ItemMoveRequest(uint owner, uint itemId, bool equip)
        {
            Owner = owner;
            ItemId = itemId;
            Equip = equip;
        }

        string IMessage.BuildMessage()
        {
            return $"Request for #{Owner} to {(Equip ? "" : "un")}equip item #{ItemId}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }

    public class ItemMovedMessage : IMessage
    {
        public Entity Owner { get; set; }

        public Entity Item { get; set; }

        public bool Equipped { get; set; }

        public ItemMovedMessage(Entity owner, Entity item, bool equipped)
        {
            Owner = owner;
            Item = item;
            Equipped = equipped;
        }

        string IMessage.BuildMessage()
        {
            return $"{Owner.GetName()} {(Equipped ? "" : "un")}equipped {Item.GetName()}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Low;
        }
    }

    public class InventoryChangedMessage : IMessage
    {
        public Entity Entity { get; set; }

        public InventoryChangedMessage(Entity entity)
        {
            Entity = entity;
        }

        string IMessage.BuildMessage()
        {
            return $"Change to the inventory of {Entity.GetName()}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }

    public class SellItemRequest : IMessage
    {
        public uint SellerId { get; set; }
        public uint ItemId { get; set; }

        public SellItemRequest(uint seller, uint item)
        {
            SellerId = seller;
            ItemId = item;
        }

        string IMessage.BuildMessage()
        {
            return $"Request: Entity #{SellerId} to sell item #{ItemId}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }

    public class CoinsChangedMessage : IMessage
    {
        public Entity Owner { get; set; }
        public int Change { get; set; }

        public CoinsChangedMessage(Entity owner, int change)
        {
            Owner = owner;
            Change = change;
        }

        string IMessage.BuildMessage()
        {
            return $"{Owner.GetName()} " + ((Change >= 0) ? $"gained {Change} coins" : $"lost {-Change} coins");
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Medium;
        }
    }
}
