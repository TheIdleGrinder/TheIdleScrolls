using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
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
            const int LvlBounties = 24;

            int level = entity.GetLevel();

            var playerComp = entity.GetComponent<PlayerComponent>();
            var storyComp = entity.GetComponent<QuestProgressComponent>();
            if (playerComp == null || storyComp == null)
                return;

            void setFeatureState(GameFeature feature, bool state)
            {
                playerComp.SetFeatureState(feature, state);
                postMessageCallback(new FeatureStateMessage(feature, state));
            }

            void setQuestState(QuestId quest, QuestStates.GettingStarted progress, string message)
            {
                storyComp.SetQuestProgress(quest, progress);
                postMessageCallback(new QuestProgressMessage(quest, (int)progress, message));
            }

            var progress = (QuestStates.GettingStarted)storyComp.GetQuestProgress(QuestId.GettingStarted);
            bool isStepDone(QuestStates.GettingStarted step)
            {
                return progress >= step;
            }

            if (!isStepDone(QuestStates.GettingStarted.Inventory) && level >= LvlInventory)
            {
                if (!entity.HasComponent<InventoryComponent>())
                {
                    InventoryComponent invComp = new();
                    List<ItemBlueprint> weapons = ItemFamilies.Weapons
                        .Select(i => new ItemBlueprint(i, 0, MaterialId.Simple)).ToList();

                    entity.AddComponent(invComp);
                    entity.AddComponent(new EquipmentComponent());
                    string itemString = "";
                    var names = new List<string>();
                    foreach (var weaponCode in weapons)
                    {
                        Entity? weapon = ItemFactory.ExpandCode(weaponCode);
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
                string itemString = "";
                foreach (var family in ItemFamilies.Armors)
                {
                    Entity? item = ItemFactory.MakeItem(new(family, 0, MaterialId.Simple));
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
                    foreach (var family in ItemFamilies.Weapons)
                    {
                        var item = ItemFactory.MakeItem(ItemBlueprint.WithLocalMaterialIndex(family, 1, 0, 1));
                        if (item != null)
                        {
                            rewardItems.Add(item);
                        }
                        else
                        {
                            throw new Exception($"Invalid item family: {family}");
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

            if (!isStepDone(QuestStates.GettingStarted.Bounties)
                && (entity.GetComponent<PlayerProgressComponent>()?.Data.HighestWildernessKill ?? 0) >= LvlBounties)
            {
                setFeatureState(GameFeature.Bounties, true);
                setQuestState(QuestId.GettingStarted, QuestStates.GettingStarted.Bounties, 
                    "You have shown yourself capable of venturing far into the wilderness. From this point onwards, you will be " +
                    "eligible to receive coins as bounties for scouting ahead and defeating the dangerous denizens of the wild.");
                BountyHunterComponent bountyHunterComponent = new()
                {
                    HighestCollected = LvlBounties
                };
                entity.AddComponent(bountyHunterComponent);
            }
        }
    }
}
