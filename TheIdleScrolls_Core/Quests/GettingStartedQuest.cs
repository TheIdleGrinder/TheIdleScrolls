﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Messages;
using TheIdleScrolls_Core.Systems;

using QuestStates = TheIdleScrolls_Core.Components.QuestStates;

namespace TheIdleScrolls_Core.Quests
{
    internal class GettingStartedQuest : AbstractQuest
    {
        readonly List<Entity> rewardItems = new();

        public override QuestId GetId()
        {
            return QuestId.GettingStarted;
        }

        public override void UpdateEntity(Entity entity, Coordinator coordinator, World world, double dt, Action<IMessage> postMessageCallback)
        {
            const int LvlInventory = 2;
            const int LvlMobAttacks = 6;
            const int LvlArmor = 8;
            const int LvlAbilities = 4;
            const int LvlPerks = 5;
            const int LvlTravel = 10;

            int level = entity.GetLevel();

            var playerComp = entity.GetComponent<PlayerComponent>();
            var storyComp = entity.GetComponent<QuestProgressComponent>();
            if (playerComp == null || storyComp == null)
                return;

            var setFeatureState = (GameFeature feature, bool state) =>
            {
                playerComp.SetFeatureState(feature, state);
                postMessageCallback(new FeatureStateMessage(feature, state));
            };

            var setQuestState = (QuestId quest, QuestStates.GettingStarted progress, string message) =>
            {
                storyComp.SetQuestProgress(quest, progress);
                postMessageCallback(new QuestProgressMessage(quest, (int)progress, message));
            };

            var progress = (QuestStates.GettingStarted)storyComp.GetQuestProgress(QuestId.GettingStarted);
            var isStepDone = (QuestStates.GettingStarted step) =>
            {
                return progress >= step;
            };

            if (!isStepDone(QuestStates.GettingStarted.Inventory) && level >= LvlInventory)
            {
                if (!entity.HasComponent<InventoryComponent>())
                {
                    InventoryComponent invComp = new();
                    List<ItemIdentifier> weapons = (new List<string>() { "SBL0", "LBL0", "AXE0", "BLN0", "POL0" })
                        .Select(i => new ItemIdentifier(i)).ToList();
                    ItemFactory factory = new();

                    entity.AddComponent(invComp);
                    entity.AddComponent(new EquipmentComponent());
                    string itemString = "";
                    var names = new List<string>();
                    foreach (var weaponCode in weapons)
                    {
                        Entity? weapon = factory.ExpandCode(weaponCode);
                        if (weapon != null)
                        {
                            itemString += $"\n  - Received '{weapon.GetName()}'";
                            coordinator.AddEntity(weapon);
                            postMessageCallback(new ItemReceivedMessage(entity, weapon));
                            names.Add(weapon.GetName());
                        }
                    }

                    setQuestState(QuestId.GettingStarted, QuestStates.GettingStarted.Inventory,
                        $"Here, take some weapons. Time to gear up!{itemString}");
                }
                storyComp.SetQuestProgress(QuestId.GettingStarted, QuestStates.GettingStarted.Inventory);
                setFeatureState(GameFeature.Inventory, true);
            }
            if (!(isStepDone(QuestStates.GettingStarted.Perks) && playerComp.AvailableFeatures.Contains(GameFeature.Perks)) 
                && level >= LvlPerks)
            {
                if (!isStepDone(QuestStates.GettingStarted.Perks))
                {
                    setQuestState(QuestId.GettingStarted, QuestStates.GettingStarted.Perks, "");
                }
                setFeatureState(GameFeature.Perks, true);
            }   
            if (!isStepDone(QuestStates.GettingStarted.Outside) && level >= LvlMobAttacks)
            {
                setQuestState(QuestId.GettingStarted, QuestStates.GettingStarted.Outside,
                    "Alright, after beating up all those dummies you should be ready to take on an actual challenge.");
            }

            if (!isStepDone(QuestStates.GettingStarted.Armor) && level >= LvlArmor)
            {
                List<string> items = new() { "LAR0", "HAR0" };
                ItemFactory factory = new();

                string itemString = "";
                foreach (var itemCode in items)
                {
                    Entity? item = factory.ExpandCode(itemCode);
                    if (item != null)
                    {
                        itemString += $"\n  - Received '{item.GetName()}'";
                        coordinator.AddEntity(item);
                        postMessageCallback(new ItemReceivedMessage(entity, item));
                    }
                }

                setQuestState(QuestId.GettingStarted, QuestStates.GettingStarted.Armor,
                    $"Those mobs are getting nasty, better put on some armor!" +
                    $"{itemString}");
                setFeatureState(GameFeature.Armor, true);
            }

            if (!isStepDone(QuestStates.GettingStarted.Abilities) && level >= LvlAbilities)
            {
                setQuestState(QuestId.GettingStarted, QuestStates.GettingStarted.Abilities,
                    "");
                setFeatureState(GameFeature.Abilities, true);
            }

            if (!isStepDone(QuestStates.GettingStarted.Travel) && level >= LvlTravel)
            {
                if (!entity.HasComponent<TravellerComponent>())
                {
                    entity.AddComponent(new TravellerComponent());
                    setQuestState(QuestId.GettingStarted, QuestStates.GettingStarted.Travel,
                        $"You can now travel between areas. Pick a spot to grind or push forward to unlock higher zones." +
                        $"\n  - Unlocked manual travel between areas");
                }
                storyComp.SetQuestProgress(QuestId.GettingStarted, QuestStates.GettingStarted.Travel);
                setFeatureState(GameFeature.Travel, true);
            }

            if (!isStepDone(QuestStates.GettingStarted.Dungeon))
            {
                if (rewardItems.Count == 0 && coordinator.MessageTypeIsOnBoard<DungeonCompletedMessage>())
                {
                    // Generate list of items, store in rewardItems listk send names as response options
                    ItemFactory factory = new();
                    var itemCodes = new List<string>() { "SBL1", "LBL1", "AXE1", "BLN1", "POL1" };
                    foreach (var itemCode in itemCodes)
                    {
                        var item = factory.ExpandCode(itemCode + "+1");
                        if (item != null)
                        {
                            rewardItems.Add(item);
                        }
                        else
                        {
                            throw new Exception($"Invalid item code: {itemCode}");
                        }
                    }
                    postMessageCallback(new DialogueMessage(GetId().ToString(), "", "", 
                        "Good job clearing your first dungeon. The people of the nearby village are impressed by your heroism" +
                        " and offer you a reward of your choice:", 
                        rewardItems.Select(i => i.GetName()).ToList())
                    );
                }
                else if (rewardItems.Count > 0 && coordinator.MessageTypeIsOnBoard<DialogueResponseMessage>())
                {
                    var response = coordinator.FetchMessagesByType<DialogueResponseMessage>()
                        .FirstOrDefault(m => m.ResponseId == GetId().ToString());
                    if (response != null)
                    {
                        bool success = false;
                        foreach (Entity item in rewardItems)
                        {
                            if (item.GetName() == response.Response)
                            {
                                coordinator.AddEntity(item);
                                rewardItems.Clear();
                                postMessageCallback(new ItemReceivedMessage(entity, item));
                                setQuestState(QuestId.GettingStarted, QuestStates.GettingStarted.Dungeon, "");
                                success = true;
                                break;
                            }
                        }
                        if (!success)
                        {
                            throw new Exception($"Invalid reward choice: {response.Response}");
                        }
                    }
                }
            }
        }
    }
}
