using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Crafting;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Systems
{
    public class CraftingSystem : AbstractSystem
    {
		readonly Cooldown UpdateCrafts = new(0.5);
        double UpdateCraftsTime = 0.0;

		readonly Random Rng = new();

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            foreach (var reforgeReq in coordinator.FetchMessagesByType<ReforgeItemRequest>())
            {
                // Fetch owner and item, check id validity
                var owner = coordinator.GetEntity(reforgeReq.OwnerId) ?? throw new Exception($"Invalid entity id: {reforgeReq.OwnerId}");
                var item = coordinator.GetEntity(reforgeReq.ItemId) ?? throw new Exception($"Invalid entity id: {reforgeReq.ItemId}");
                var inventoryComp = owner.GetComponent<InventoryComponent>() ?? throw new Exception($"{owner.GetName()} does not have an inventory");
                // Check ownership
                if (!inventoryComp.GetItems().Any(i => i.Id == item.Id))
                {
                    throw new Exception($"{owner.GetName()} does have {item.GetName()} in inventory");
                }
                int itemLevel = ItemFactory.GetItemDropLevel(item.GetComponent<ItemComponent>()?.Code
                    ?? throw new Exception($"{item.GetName()} is not an item"));
                // Check for crafting bench
                var craftComp = owner.GetComponent<CraftingBenchComponent>() 
                    ?? throw new Exception($"{owner.GetName()} is not able to craft items");
                // Check for free crafting slots
                if (!craftComp.HasFreeSlot)
                {
					coordinator.PostMessage(this, new TextMessage($"{owner.GetName()} has no free crafting slots", IMessage.PriorityLevel.VeryHigh));
					continue;
				}

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
				// Spend coins
				purseComp.RemoveCoins(cost);
				coordinator.PostMessage(this, new CoinsChangedMessage(owner, -cost));

                // Start reforging
                double duration = Functions.CalculateReforgingDuration(item); // TODO: Use item level?
                double roll = Rng.NextDouble();
                craftComp.AddCraft(new(CraftingType.Reforge, item, duration, roll));
                inventoryComp.RemoveItem(item);
                coordinator.PostMessage(this, new CraftingStartedMessage(owner, item, cost, CraftingType.Reforge));
                coordinator.PostMessage(this, new InventoryChangedMessage(owner));
            }

            // Updating twice per second is enough
            UpdateCraftsTime += dt;
            bool doUpdate = UpdateCrafts.Update(dt) > 0;
            if (doUpdate)
            {
                UpdateCrafting(world, coordinator, UpdateCraftsTime);
                UpdateCraftsTime = 0.0;
            }
        }

        public void UpdateCrafting(World _, Coordinator coordinator, double dt)
        {
            bool updated = false;
            foreach (var crafter in coordinator.GetEntities<CraftingBenchComponent>())
            {
                var bench = crafter.GetComponent<CraftingBenchComponent>()!; // has to exist due to filter above
                List<uint> finishedCrafts = new();
                foreach (var craft in bench.ActiveCrafts)
                {
                    updated = true;
                    craft.Update(dt);
                    if (craft.HasFinished)
                    {
                        if (craft.Type == CraftingType.Reforge)
                        {
							int abilityLevel = crafter.GetComponent<AbilitiesComponent>()
                                ?.GetAbility(Properties.Constants.Key_Ability_Crafting)?.Level ?? 1;
							int rarity = craft.TargetItem.GetComponent<ItemRarityComponent>()?.RarityLevel ?? 0;
                            double percentage = Functions.CalculateReforgingSuccessRate(abilityLevel, rarity);

                            int newRarity = rarity + (percentage >= craft.Roll ? 1 : -1);
                            if (newRarity >= 0)
                            {
                                ItemFactory.SetItemRarity(craft.TargetItem, newRarity);
                            }
                            craft.TargetItem.GetComponent<ItemReforgeableComponent>()!.Reforged = true;
                            coordinator.PostMessage(this, new ItemReforgedMessage(crafter, craft.TargetItem, newRarity > rarity));
						}
                        
                        var invComp = crafter.GetComponent<InventoryComponent>()!; // CornerCut: technically not guaranteed
                        invComp.AddItem(craft.TargetItem);
                        finishedCrafts.Add(craft.ID);
                    }
                }
                foreach (var id in finishedCrafts)
                {
					bench.RemoveCraft(id);
				}
            }
            if (updated)
            {
                coordinator.PostMessage(this, new CraftingUpdateMessage());
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

    public class CraftingStartedMessage : IMessage
    {
		public Entity Owner { get; set; }
		public Entity Item { get; set; }
		public int CoinsPaid { get; set; }

        public CraftingType Type { get; set; }

		public CraftingStartedMessage(Entity owner, Entity item, int cost, CraftingType type)
        {
			Owner = owner;
			Item = item;
            CoinsPaid = cost;
			Type = type;
		}

		string IMessage.BuildMessage()
        {
			return $"{Owner.GetName()} spent {CoinsPaid}c to start {(Type == CraftingType.Reforge ? "reforging" : "crafting")} {Item.GetName()}";
		}

		IMessage.PriorityLevel IMessage.GetPriority()
        {
			return IMessage.PriorityLevel.High;
		}
	}

    public class ItemReforgedMessage : IMessage
    {
        public Entity Owner { get; set; }
        public Entity Item { get; set; }
        public bool Success { get; set; }

        public ItemReforgedMessage(Entity owner, Entity item, bool success)
        {
            Owner = owner;
            Item = item;
            Success = success;
        }

        string IMessage.BuildMessage()
        {
            return $"{Owner.GetName()} finished reforging {Item.GetName()}: Rarity {(Success ? "increased" : "reduced")}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.High;
        }
    }

    public class CraftingUpdateMessage : IMessage
    {
        string IMessage.BuildMessage() => $"Crafting progress has been updated";

        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }
}
