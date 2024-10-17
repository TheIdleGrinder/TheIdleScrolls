using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;

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
            HashSet<Entity> changedEquipments = new();

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
                            //Try finding a single(!) item to replace
                            List<EquipmentSlot> missing = equipmentComp.GetMissingEquipmentSlotsForItem(item);
                            // Shields replace shields, weapons replace weapons first
                            Entity? prevItem = FindBlockingItem(equipmentComp, missing, item.IsWeapon(), item.IsShield());
                            // If no shield or weapon to replace is found, try the other type next
                            // CornerCut: This does not work for items that are both weapons and shields
                            if (prevItem == null && (item.IsWeapon() || item.IsShield()))
                            {
                                prevItem = FindBlockingItem(equipmentComp, missing, item.IsShield(), item.IsWeapon());
                            }
                                
                            if (prevItem != null)
                            {
                                if (equipmentComp.UnequipItem(prevItem))
                                    inventoryComp.AddItem(prevItem);
                            }
                            else
                            {
                                coordinator.PostMessage(this,
                                    new TextMessage($"Too many occupied equipment slots to equip {item.GetName()}", IMessage.PriorityLevel.VeryHigh));
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
                changedEquipments.Add(owner);
                changedInventories.Add(owner);
            }

            foreach (var sale in coordinator.FetchMessagesByType<SellItemRequest>())
            {
                Entity? owner = coordinator.GetEntity(sale.SellerId);
                if (owner == null)
                    continue;
                var inventoryComp = owner.GetComponent<InventoryComponent>() 
                    ?? throw new Exception($"Entity {owner.GetName()} has no Inventory component");
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
                    coordinator.RemoveEntity(sale.ItemId);
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

            foreach (var entity in changedEquipments)
            {
                var equipComp = entity.GetComponent<EquipmentComponent>();
                if (equipComp == null)
                    continue; // Can't happen
                // CornerCut: The whole handling of encumbrance is not very elegant. How do we inform the app in a smoother way?
                coordinator.PostMessage(this, new EncumbranceChangedMessage(entity, equipComp.TotalEncumbrance)); 
            }
        }

        /// <summary>
        /// Attempts to find a single item occupying a list of equipment slots
        /// </summary>
        /// <param name="equipment"></param>
        /// <param name="slots"></param>
        /// <param name="onlyWeapons"></param>
        /// <param name="onlyShields"></param>
        /// <returns></returns>
        private static Entity? FindBlockingItem(EquipmentComponent equipment, List<EquipmentSlot> slots, bool onlyWeapons, bool onlyShields)
        {
            foreach (var item in equipment.GetItems().Where(i => (!onlyWeapons || i.IsWeapon()) && (!onlyShields || i.IsShield())))
            {
                if (AIsSubsetOfB(slots, item.GetComponent<EquippableComponent>()?.Slots ?? new()))
                    return item;
            }
            return null;
        }

        private static bool AIsSubsetOfB(List<EquipmentSlot> a, List<EquipmentSlot> b)
        {
            foreach (var slot in a)
            {
                if (a.Count(s => s == slot) > b.Count(s => s == slot))
                    return false;
            }
            return true;
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

    public class EncumbranceChangedMessage : IMessage
    {
        public Entity Entity { get; set; }
        public double Encumbrance { get; set; }

        public EncumbranceChangedMessage(Entity entity, double encumbrance)
        {
            Entity = entity;
            Encumbrance = encumbrance;
        }

        string IMessage.BuildMessage()
        {
            return $"Encumbrance on entity {Entity.GetName()} changed to {Encumbrance}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Low;
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
