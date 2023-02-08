using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core.Systems
{
    public class CraftingSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            foreach (var reforgeReq in coordinator.FetchMessagesByType<ReforgeItemRequest>())
            {
                // Fetch owner and item, check id validity
                var owner = coordinator.GetEntity(reforgeReq.OwnerId) ?? throw new Exception($"Invalid entity id: {reforgeReq.OwnerId}");
                var item = coordinator.GetEntity(reforgeReq.ItemId) ?? throw new Exception($"Invalid entity id: {reforgeReq.ItemId}");
                // Check ownership
                if (!owner.GetComponent<InventoryComponent>()?.GetItems().Any(i => i.Id == item.Id) ?? false)
                {
                    throw new Exception($"{owner.GetName()} does have {item.GetName()} in inventory");
                }
                // Check funds
                int cost = item.GetComponent<ItemReforgeableComponent>()?.Cost ?? throw new Exception($"{item.GetName()} is not reforgeable");
                CoinPurseComponent? purseComp = owner.GetComponent<CoinPurseComponent>();
                if (purseComp == null || purseComp.Coins < cost)
                {
                    throw new Exception($"{owner.GetName()} does not have {cost} coins for reforging {item.GetName()}");
                }
                // Spend coins
                purseComp.RemoveCoins(cost);
                // Perform reforge
                int itemLevel = item.GetComponent<ItemComponent>()?.Code.GetGenusDescription().DropLevel 
                    ?? throw new Exception($"{item.GetName()} is not an item");
                int newRarity = ItemFactory.GetRandomRarity(itemLevel, 1.0);
                ItemFactory.SetItemRarity(item, newRarity);
                // Send result message
                coordinator.PostMessage(this, new ItemReforgedMessage(owner, item, cost, newRarity));
            }
        }
    }

    public class ReforgeItemRequest : IMessage
    {
        public uint OwnerId { get; set; }
        public uint ItemId { get; set; }

        public ReforgeItemRequest(uint ownerId, uint itemId)
        {
            OwnerId = ownerId;
            ItemId = itemId;
        }

        string IMessage.BuildMessage()
        {
            return $"Request: #{OwnerId} to reforge item #{ItemId}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }

    public class ItemReforgedMessage : IMessage
    {
        public Entity Owner { get; set; }
        public Entity Item { get; set; }
        public int CoinsPaid { get; set; }
        public int RarityResult { get; set; }

        public ItemReforgedMessage(Entity owner, Entity item, int coinsPaid, int rarityResult)
        {
            Owner = owner;
            Item = item;
            CoinsPaid = coinsPaid;
            RarityResult = rarityResult;
        }

        string IMessage.BuildMessage()
        {
            return $"{Owner.GetName()} spent {CoinsPaid} to craft {Item.GetName()}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.High;
        }
    }
}
