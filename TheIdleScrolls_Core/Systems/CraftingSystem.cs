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
        bool FirstUpdate = true;
		readonly Cooldown UpdateCrafts = new(0.5);
        List<Entity> ItemPrototypes = new();
        double UpdateCraftsTime = 0.0;

		readonly Random Rng = new();

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            var postMessageCallback = (IMessage message) =>
            {
                coordinator.PostMessage(this, message);
            };

            if (FirstUpdate || coordinator.MessageTypeIsOnBoard<DungeonCompletedMessage>())
            {
                foreach (var crafter in coordinator.GetEntities<CraftingBenchComponent>())
                {
                    var craftingBench = crafter.GetComponent<CraftingBenchComponent>()!;

                    int maxLevel = craftingBench.MaxCraftingLevel;
                    foreach (var message in coordinator.FetchMessagesByType<DungeonCompletedMessage>())
                    {
                        maxLevel = Math.Max(maxLevel, world.AreaKingdom.GetDungeon(message.DungeonId)?.Level ?? 0);
                    }

                    var progressComp = crafter.GetComponent<PlayerProgressComponent>();
                    if (progressComp != null)
                    {
                        maxLevel = Math.Max(maxLevel, progressComp.Data.GetClearedDungeons()
                            .Select(dId => world.Map.Dungeons.Where(d => d.Id == dId).FirstOrDefault()?.Level ?? 0).Max());
                    }
                    
                    if (maxLevel > craftingBench.MaxCraftingLevel)
                    {
                        craftingBench.MaxCraftingLevel = maxLevel;
                    }
                    craftingBench.AvailablePrototypes = GetPrototypes()
                        .Where(i => (i.GetComponent<LevelComponent>()?.Level ?? 0) <= craftingBench.MaxCraftingLevel)
                        .ToList();

                    if (!FirstUpdate)
                        coordinator.PostMessage(this, new AvailableCraftsChanged());
                }

                FirstUpdate = false;
            }

            foreach (var craftReq in coordinator.FetchMessagesByType<CraftItemRequest>())
            {
                var owner = coordinator.GetEntity(craftReq.OwnerId) ?? throw new Exception($"Invalid entity id: {craftReq.OwnerId}");
                HandleCraftRequest(owner, craftReq.ItemId, postMessageCallback);
            }

            foreach (var reforgeReq in coordinator.FetchMessagesByType<ReforgeItemRequest>())
            {
                // Fetch owner and item, check id validity
                var owner = coordinator.GetEntity(reforgeReq.OwnerId) ?? throw new Exception($"Invalid entity id: {reforgeReq.OwnerId}");
                var item = coordinator.GetEntity(reforgeReq.ItemId) ?? throw new Exception($"Invalid entity id: {reforgeReq.ItemId}");
                HandleReforgeRequest(owner, item, postMessageCallback);
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

        public void HandleCraftRequest(Entity owner, uint prototypeId, Action<IMessage> postMessage)
        {
            // Check for crafting bench
            var craftComp = GetBenchAndCheckSlots(owner, postMessage);
            if (craftComp == null)
            {
                return;
            }

            // Check for prototype
            Entity? prototype = craftComp.AvailablePrototypes.FirstOrDefault(i => i.Id == prototypeId)
                ?? throw new Exception($"Invalid crafting prototype id: #{prototypeId}");

            // Check funds, for now crafting cost = reforging cost
            int cost = prototype.GetComponent<ItemReforgeableComponent>()?.Cost ?? throw new Exception($"{prototype.GetName()} has no value");
            if (!CheckAndSpendCoins(owner, cost, postMessage))
            {
                return;
            }

            // Clone prototype
            Entity? newItem = ItemFactory.MakeItem(prototype.GetComponent<ItemComponent>()?.Code
                               ?? throw new Exception($"{prototype.GetName()} is not an item")) 
                ?? throw new Exception($"Failed to create item from prototype #{prototypeId}");

            // Start crafting
            double duration = Functions.CalculateReforgingDuration(newItem);
            craftComp.AddCraft(new(CraftingType.Craft, newItem, duration, 0.0));
            postMessage(new CraftingStartedMessage(owner, newItem, cost, CraftingType.Craft));
        }

        public void HandleReforgeRequest(Entity owner, Entity item, Action<IMessage> postMessage)
        {
            var inventoryComp = owner.GetComponent<InventoryComponent>() ?? throw new Exception($"{owner.GetName()} does not have an inventory");
            // Check ownership
            if (!inventoryComp.GetItems().Any(i => i.Id == item.Id))
            {
                throw new Exception($"{owner.GetName()} does have {item.GetName()} in inventory");
            }
            int itemLevel = ItemFactory.GetItemDropLevel(item.GetComponent<ItemComponent>()?.Code
                ?? throw new Exception($"{item.GetName()} is not an item"));
            
            // Check for crafting bench
            var craftComp = GetBenchAndCheckSlots(owner, postMessage);
            if (craftComp == null)
            {
                return;
            }

            // Check funds
            int cost = item.GetComponent<ItemReforgeableComponent>()?.Cost ?? throw new Exception($"{item.GetName()} is not reforgeable");
            if (!CheckAndSpendCoins(owner, cost, postMessage))
            {
                return;
            }

            // Start reforging
            double duration = Functions.CalculateReforgingDuration(item);
            double roll = Rng.NextDouble();
            craftComp.AddCraft(new(CraftingType.Reforge, item, duration, roll));
            inventoryComp.RemoveItem(item);
            postMessage(new CraftingStartedMessage(owner, item, cost, CraftingType.Reforge));
            postMessage(new InventoryChangedMessage(owner));
        }

        private static CraftingBenchComponent? GetBenchAndCheckSlots(Entity crafter, Action<IMessage> postMessage)
        {
            var craftComp = crafter.GetComponent<CraftingBenchComponent>()
                ?? throw new Exception($"{crafter.GetName()} is not able to craft items");
            if (!craftComp.HasFreeSlot)
            {
                postMessage(new TextMessage($"{crafter.GetName()} has no free crafting slots", IMessage.PriorityLevel.VeryHigh));
                return null;
            }
            return craftComp;
        }

        private static bool CheckAndSpendCoins(Entity owner, int cost, Action<IMessage> postMessage)
        {
            CoinPurseComponent? purseComp = owner.GetComponent<CoinPurseComponent>();
            if (purseComp == null || purseComp.Coins < cost)
            {
                postMessage(new TextMessage($"{owner.GetName()} does not have {cost} coins",
                    IMessage.PriorityLevel.VeryHigh));
                return false;
            }
            purseComp.RemoveCoins(cost);
            postMessage(new CoinsChangedMessage(owner, -cost));
            return true;
        }

        private List<Entity> GetPrototypes()
        {
            if (!ItemPrototypes.Any())
            {
                ItemPrototypes = LootTable.Generate(new(999, 0, 0, 0.0))
                                        .GetItemCodes()
                                        .Select(c => ItemFactory.MakeItem(new(c)))
                                        .Where(i => i is not null)
                                        .OfType<Entity>()
                                        .OrderBy(i => i.GetComponent<LevelComponent>()?.Level ?? 0)
                                        .ToList();
            }
            return ItemPrototypes;            
        }

        public void UpdateCrafting(World _, Coordinator coordinator, double dt)
        {
            bool updated = false;
            foreach (var crafter in coordinator.GetEntities<CraftingBenchComponent>())
            {
                var bench = UpdateCraftingBench(crafter)!; // has to exist due to filter above
                List<uint> finishedCrafts = new();
                foreach (var craft in bench.ActiveCrafts)
                {
                    updated = true;
                    craft.Update(dt);
                    if (craft.HasFinished)
                    {
                        if (craft.Type == CraftingType.Craft)
                        {
                            coordinator.PostMessage(this, new ItemCraftedMessage(crafter, craft.TargetItem));
                        }
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
                            coordinator.PostMessage(this, new ItemReforgedMessage(crafter, craft.TargetItem, newRarity > rarity));
						}

                        craft.TargetItem.GetComponent<ItemReforgeableComponent>()!.Reforged = true;
                        var invComp = crafter.GetComponent<InventoryComponent>()!; // CornerCut: technically not guaranteed
                        invComp.AddItem(craft.TargetItem);
                        coordinator.PostMessage(this, new InventoryChangedMessage(crafter));
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

        private static CraftingBenchComponent? UpdateCraftingBench(Entity crafter)
        {
            var bench = crafter.GetComponent<CraftingBenchComponent>();
            if (bench == null)
            {
                return null;
            }

            // Update number of slots
            var modComp = crafter.GetComponent<ModifierComponent>();
            if (modComp != null)
            {
                bench.CraftingSlots = (int)modComp.ApplyApplicableModifiers(1.0, new string[] { Definitions.Tags.CraftingSlot } );
            }

            return bench;
        }
    }

    public class CraftItemRequest : IMessage
    {
        public uint OwnerId { get; set; }
        public uint ItemId { get; set; }

        public CraftItemRequest(uint ownerId, uint itemId)
        {
            OwnerId = ownerId;
            ItemId = itemId;
        }

        string IMessage.BuildMessage()
        {
            return $"Request: #{OwnerId} to craft copy of prototype item #{ItemId}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
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

    public class AvailableCraftsChanged : IMessage
    {
        string IMessage.BuildMessage() => "New crafts unlocked!";

        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.High;
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

    public class ItemCraftedMessage : IMessage
    {
        public Entity Owner { get; set; }
        public Entity Item { get; set; }

        public ItemCraftedMessage(Entity owner, Entity item)
        {
            Owner = owner;
            Item = item;
        }

        string IMessage.BuildMessage()
        {
            return $"{Owner.GetName()} finished crafting {Item.GetName()}";
        }

        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.High;
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
