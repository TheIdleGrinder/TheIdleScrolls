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

                        var previousItem = equipmentComp.GetItemInSlot(equippableComp.Slot);
                        if (previousItem != null)
                        {
                            if (equipmentComp.UnequipItem(previousItem))
                                inventoryComp.AddItem(previousItem);
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
    }
}
