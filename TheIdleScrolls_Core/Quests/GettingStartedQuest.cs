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

namespace TheIdleScrolls_Core.Quests
{
    internal class GettingStartedQuest : AbstractQuest
    {

        [Flags]
        public enum StateFlags
        {
            Weapons     = 1 << 0,
            Armor       = 1 << 1,
            Abilities   = 1 << 2,
            MobAttacks  = 1 << 3,
            Perks       = 1 << 4,
            Travel      = 1 << 5,
            Dungeon     = 1 << 6,
            Bounties    = 1 << 7,
            Crafting    = 1 << 8
        }

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
            const int LvlCrafting = 20;
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

            void setQuestState(StateFlags progress, string message)
            {
                StateFlags previousState = (StateFlags)storyComp.GetQuestProgress(GetId());
                StateFlags newState = previousState | progress;
                storyComp.SetQuestProgress(GetId(), newState);
                postMessageCallback(new QuestProgressMessage(GetId(), (int)newState, message));
            }

            var progress = (StateFlags)storyComp.GetQuestProgress(QuestId.GettingStarted, 0);
            bool isStepDone(StateFlags step)
            {
                return (progress & step) != 0;
            }

            if (!isStepDone(StateFlags.Weapons) && level >= LvlInventory)
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

                    setQuestState(StateFlags.Weapons,
                        $"Here, take some weapons. Time to gear up!{itemString}");
                }
                else
                {
                    storyComp.SetQuestProgress(GetId(), StateFlags.Weapons); // CornerCut: Assumes that this is always the first step
                }
                setFeatureState(GameFeature.Inventory, true);
            }
            if (!(isStepDone(StateFlags.Perks) && playerComp.AvailableFeatures.Contains(GameFeature.Perks)) 
                && level >= LvlPerks)
            {
                if (!isStepDone(StateFlags.Perks))
                {
                    setQuestState(StateFlags.Perks, "");
                }
                setFeatureState(GameFeature.Perks, true);
            }   
            if (!isStepDone(StateFlags.MobAttacks) && level >= LvlMobAttacks)
            {
                setQuestState(StateFlags.MobAttacks,
                    "Alright, after beating up all those dummies you should be ready to take on an actual challenge.");
            }

            if (!isStepDone(StateFlags.Armor) && level >= LvlArmor)
            {
                var abilitiesComp = entity.GetComponent<AbilitiesComponent>();
                if (abilitiesComp == null)
                {
                    abilitiesComp = new();
                    entity.AddComponent(abilitiesComp);
                }
                Abilities.Armors.ForEach(ability => {
                    abilitiesComp.AddAbility(ability);
                    postMessageCallback(new AbilityAddedMessage(entity, ability));
                });

                string itemString = "";
                List<string> families = [ItemFamilies.HeavyChest, ItemFamilies.LightChest];
                foreach (var family in families)
                {
                    Entity? item = ItemFactory.MakeItem(new(family, 0, MaterialId.Simple));
                    if (item != null)
                    {
                        itemString += $"\n  - Received '{item.GetName()}'";
                        coordinator.AddEntity(item);
                        postMessageCallback(new ItemReceivedMessage(entity, item));
                    }
                    
                }

                setQuestState(StateFlags.Armor,
                    $"Those mobs are getting nasty, better put on some armor!" +
                    $"{itemString}");
                setFeatureState(GameFeature.Armor, true);
            }

            if (!isStepDone(StateFlags.Abilities) && level >= LvlAbilities)
            {
                setQuestState(StateFlags.Abilities, "");
                setFeatureState(GameFeature.Abilities, true);
                var abilitiesComp = entity.GetComponent<AbilitiesComponent>();
                if (abilitiesComp == null)
                {
                    abilitiesComp = new();
                    entity.AddComponent(abilitiesComp);
                }
                Abilities.Weapons.ForEach(ability => {
                    abilitiesComp.AddAbility(ability);
                    postMessageCallback(new AbilityAddedMessage(entity, ability));
                });
            }

            if (!isStepDone(StateFlags.Travel) && level >= LvlTravel)
            {
                if (!entity.HasComponent<TravellerComponent>())
                {
                    entity.AddComponent(new TravellerComponent());
                    setQuestState(StateFlags.Travel,
                        $"You can now travel between areas. Pick a spot to grind or push forward to unlock higher zones." +
                        $"\n  - Unlocked manual travel between areas");
                }
                else
                {
                    setQuestState(StateFlags.Travel, "");
                }
                setFeatureState(GameFeature.Travel, true);
            }

            if (!isStepDone(StateFlags.Dungeon))
            {
                if (rewardItems.Count == 0 && coordinator.MessageTypeIsOnBoard<DungeonCompletedMessage>())
                {
                    // Generate list of items, store in rewardItems list and send names as response options
                    // CornerCut: Assumes that only weapons have the target level
                    var blueprints = LootTable.GetAllBlueprints().Where(b => b.GetDropLevel() == Materials.LevelT1);
                    foreach (var blueprint in blueprints)
                    {
                        var item = ItemFactory.MakeItem(blueprint);
                        if (item != null)
                        {
                            rewardItems.Add(item);
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
                                setQuestState(StateFlags.Dungeon, "");
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

            if (!isStepDone(StateFlags.Bounties)
                && (entity.GetComponent<PlayerProgressComponent>()?.Data.HighestWildernessKill ?? 0) >= LvlBounties)
            {
                setFeatureState(GameFeature.Bounties, true);
                setQuestState(StateFlags.Bounties, 
                    "You have shown yourself capable of venturing far into the wilderness. From this point onwards, you will be " +
                    "eligible to receive coins as bounties for scouting ahead and defeating the dangerous denizens of the wild.");
                BountyHunterComponent bountyHunterComponent = new()
                {
                    HighestCollected = LvlBounties
                };
                entity.AddComponent(bountyHunterComponent);
            }

            if (!isStepDone(StateFlags.Crafting)
                && entity.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeons().Count >= 2)
            {
                setFeatureState(GameFeature.Crafting, true);
                setQuestState(StateFlags.Crafting,
                    $"While dungeon delving is more exciting, the wise adventurer knows that the most reliable way to acquire equipment " +
                    $"is to make it yourself. Give it a try if you have some coins and a little patience");

                entity.AddComponent(new CraftingBenchComponent());
                
                entity.GetComponent<AbilitiesComponent>()?.AddAbility(Abilities.Crafting);
                entity.GetComponent<AbilitiesComponent>()?.UpdateAbility(Abilities.Crafting, 10, 0);
                postMessageCallback(new AbilityAddedMessage(entity, Abilities.Crafting));
            }
        }
    }
}
