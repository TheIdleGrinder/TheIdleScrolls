using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
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
                int itemLevel = ItemFactory.GetItemDropLevel(item.GetComponent<ItemComponent>()?.Code
                    ?? throw new Exception($"{item.GetName()} is not an item"));
                // Check funds
                int cost = item.GetComponent<ItemReforgeableComponent>()?.Cost ?? throw new Exception($"{item.GetName()} is not reforgeable");
                CoinPurseComponent? purseComp = owner.GetComponent<CoinPurseComponent>();
                if (purseComp == null || purseComp.Coins < cost)
                {
                    coordinator.PostMessage(this, 
                        new TextMessage($"{owner.GetName()} does not have {cost} coins for reforging {item.GetName()}", 
                        IMessage.PriorityLevel.VeryHigh));
                    continue;
                }
                // Get ability value
                int abilityLevel = owner.GetComponent<AbilitiesComponent>()?.GetAbility(Properties.Constants.Key_Ability_Crafting)?.Level ?? 0;
                double rarityMulti = world.RarityMultiplier * (1.0 + Definitions.Stats.CraftingAbilityBonusPerLevel * abilityLevel);
                // Calculate craft level
                int craftLevel = itemLevel;
                if (abilityLevel > 0) 
                    craftLevel = Math.Min((abilityLevel + itemLevel) / 2, 100); // Craft level = average of item and ability level, capped at 100
                // Spend coins
                purseComp.RemoveCoins(cost);
                coordinator.PostMessage(this, new CoinsChangedMessage(owner, -cost));
                // Perform reforge

                int newRarity = ItemFactory.GetRandomRarity(craftLevel, rarityMulti);
                ItemFactory.SetItemRarity(item, newRarity);
                item.GetComponent<ItemReforgeableComponent>()!.Reforged = true;
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
            return IMessage.PriorityLevel.Medium;
        }
    }
}
