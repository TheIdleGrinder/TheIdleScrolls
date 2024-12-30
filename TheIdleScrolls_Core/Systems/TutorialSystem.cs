using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Quests;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.Systems
{
    public class TutorialSystem : AbstractSystem
    {
        const int ItemCountForSelling = 12;

        public const string FinalStoryDungeon = Definitions.DungeonIds.Threshold;
        const string UnarmoredKey = "NOARMOR";
        const string UnarmedKey = "NOWEAPON";

        Entity? m_player = null;

        bool m_firstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            var playerProgress = m_player?.GetComponent<PlayerProgressComponent>();
            var globalProgress = world.GlobalEntity.GetComponent<PlayerProgressComponent>();
            
            if (m_player == null || playerProgress == null || globalProgress == null)
                return;

            int lvl = m_player.GetComponent<LevelComponent>()?.Level ?? 0;

            // Determine previous progress
            if (m_firstUpdate)
            {
                m_firstUpdate = false;
            }

            var achievementComp = coordinator.GetEntities<AchievementsComponent>().FirstOrDefault()?.GetComponent<AchievementsComponent>();

            void addTutorialProgress(TutorialStep step, string title, string text, QuestProgressMessage questMessage)
            {
                if (!globalProgress.Data.TutorialProgress.Contains(step))
                {
                    coordinator.PostMessage(this, new TutorialMessage(step, title, text, questMessage));
                    globalProgress.Data.TutorialProgress.Add(step);
                }
            }

            foreach (var message in coordinator.FetchMessagesByType<QuestProgressMessage>())
            {
                if (message.Quest == QuestId.GettingStarted)
                {
                    var progress = (GettingStartedQuest.StateFlags)message.Progress;
                    if ((progress & GettingStartedQuest.StateFlags.Weapons) != 0)
                    {
                        addTutorialProgress(TutorialStep.Inventory, "Level Up!",
                            "You have unlocked the inventory. Double click on an item in your inventory to equip it.", message);
                    }
                    if ((progress & GettingStartedQuest.StateFlags.Abilities) != 0)
                    {
                        addTutorialProgress(TutorialStep.Abilities, "Live and Learn",
                            "The more you use weapons of one type, the better you will become at handling them. Watch your " +
                            "damage and attack speed increase along with your ability level.", message);
                    }
                    if ((progress & GettingStartedQuest.StateFlags.Perks) != 0)
                    {
                        addTutorialProgress(TutorialStep.Perks, "Perks of the Trade",
                            "You can see the exact values of your ability bonus by checking the 'Perks' tab. As you continue playing " +
                            "and earning achievements, you will also unlock additional perks.", message);
                    }
                    if ((progress & GettingStartedQuest.StateFlags.MobAttacks) != 0)
                    {
                        addTutorialProgress(TutorialStep.MobAttacks, "Training is Over",
                            "From this point on, mobs are going to fight back. Watch the countdown " +
                            "near the mob. If time runs out, you lose the fight.", message);
                    }
                    if ((progress & GettingStartedQuest.StateFlags.Armor) != 0)
                    {
                        addTutorialProgress(TutorialStep.Armor, "It's Dangerous to Go Alone",
                            "Wearing armor slows down the countdown during fights but also encumbers your character, reducing attack speed. " +
                            "Heavier armor means more encumbrance, but also better protection.", message);
                    }
                    if ((progress & GettingStartedQuest.StateFlags.Travel) != 0)
                    {
                        addTutorialProgress(TutorialStep.Travel, "Freedom of Movement",
                            "Click the arrow buttons to move between zones. Higher level areas become accessible after defeating" +
                            "a mob in the previous zone." +
                            "\nUpon losing a fight, your character will automatically move down one area.",
                            message);
                    }
                }
            }

            // Evaluate conditions for current tutorial stage
            // Defeated is tracked locally until introduction of the hardcore quest
            if (!playerProgress.Data.TutorialProgress.Contains(TutorialStep.Defeated)
                && playerProgress.Data.Losses == 1)
            {
                playerProgress.Data.TutorialProgress.Add(TutorialStep.Defeated);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Defeated, "There's Always a Bigger Fish...",
                    "Time ran out and you lost this fight. Don't worry, though, a little more training should get you over the hump."));
            }
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.DungeonOpen) 
                && coordinator.MessageTypeIsOnBoard<DungeonOpenedMessage>())
            {
                var message = coordinator.FetchMessagesByType<DungeonOpenedMessage>().FirstOrDefault();
                var dungeon = DungeonList.GetDungeon(message!.DungeonId)!;
                globalProgress.Data.TutorialProgress.Add(TutorialStep.DungeonOpen);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.DungeonOpen, "A New Challenge!",
                    "Progressing in the wilderness will give you access to dungeons. A dungeon consists of several floors, each containing " +
                    "monsters that have to be defeated to proceed. They are more challenging than wilderness areas " +
                    "of the same level, but completing dungeons will grant powerful rewards. Losing a fight in the dungeon will get you sent " +
                    "back to the wilderness." +
                    $"\n  - Unlocked dungeon '{dungeon.Name}'"));
            }
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.DungeonComplete)
                && coordinator.MessageTypeIsOnBoard<DungeonCompletedMessage>())
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.DungeonComplete);
                var itemName = coordinator.FetchMessagesByType<ItemReceivedMessage>().LastOrDefault()?.Item.GetName() ?? "??";
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.DungeonComplete, "Dungeon completed!",
                    "Good job, you completed your first dungeon and obtained a reward:" +
                    $"\n  - Received '{itemName}'"));
            }
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.Evasion)
                && (m_player.GetComponent<DefenseComponent>()?.Evasion ?? 0) > 0)
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.Evasion);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Evasion, "Travelling Light",
                    $"You have proven your prowess in unarmored combat. While fighting without armor, you will now be able to " +
                    $"use your evasiness to gain additional time to defeat your enemies." +
                    $"\n  - Evasion periodically stops the fight timer"));
            }
            //if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.Unarmed)
            //    && (achievementComp?.Achievements.Count(a => a.Id.Contains(UnarmedKey) 
            //        && a.Status == Achievements.AchievementStatus.Awarded) > 0))
            //{
            //    globalProgress.Data.TutorialProgress.Add(TutorialStep.Unarmed);
            //    coordinator.PostMessage(this,
            //        new TutorialMessage(TutorialStep.Unarmed, "Iron Fists",
            //        $"You have proven your prowess in unarmed combat. Fighting without a weapon now grants 0.05 " +
            //        $"base damage per level for each owned achievement from the 'unarmed' line."));
            //}
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.FlatCircle)
                && (achievementComp?.Achievements.Count(a => a.Id.Contains(UnarmedKey)
                    && a.Id.Contains(UnarmoredKey)
                    && a.Status == Achievements.AchievementStatus.Awarded) > 0))
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.FlatCircle);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.FlatCircle, "A Flat Circle",
                    "Ironic, all this grinding just to get to a point where you use none of your gear or abilities."));
            }
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.ItemFound)
                && coordinator.MessageTypeIsOnBoard<ItemReceivedMessage>())
            {
                if (coordinator.FetchMessagesByType<ItemReceivedMessage>()
                    .Any(m => (m.Item.GetBlueprint()?.GenusIndex ?? 0) > 0))
                {
                    globalProgress.Data.TutorialProgress.Add(TutorialStep.ItemFound);
                    coordinator.PostMessage(this,
                        new TutorialMessage(TutorialStep.ItemFound, "Loot!",
                        $"You just found your first item. Defeated monsters will occasionally drop equipment which gets " +
                        $"stronger as you progress to higher zones."));
                }
            }
            // Skip tutorial for selling if the players sells an item before getting to it
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.Selling) 
                && coordinator.MessageTypeIsOnBoard<CoinsChangedMessage>())
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.Selling);
            }
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.Selling)
                && (m_player.GetComponent<InventoryComponent>()?.ItemCount ?? 0) > ItemCountForSelling)
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.Selling);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Selling, "I'm not a... pack animal",
                    $"Items are piling up in your inventory. Selling them will make it less cluttered and also earn you some pretty coins.\n" +
                    $"\n  - You can sell items from you inventory to gain coins"));
            }
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.Crafting)
                && m_player.HasComponent<CraftingBenchComponent>())
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.Crafting);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Crafting, "Let's put those coins to use",
                    $"You can unburden yourself of some cumbersome coinage by crafting items. You can either create new items or " +
                    $"attempt to refine the ones you already have. Doing so will raise your crafting ability, which improves your " +
                    $"chances of successfully refining items." +
                    $"\n  - You can now spend coins to craft and refine items"));
            }

            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.Bounties)
                && coordinator.MessageTypeIsOnBoard<BountyMessage>())
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.Bounties);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Bounties, "Bounty Hunter",
                    $"The main way to earn bounties is by defeating " +
                    $"an enemy at a higher level than you had before in the wilderness. You will also be awarded a bounty every " +
                    $"time you defeat {BountySystem.EnemiesPerHunt} enemies in the wilderness. The value of bounties " +
                    $"depends on the level of the defeated enemies."));
            }
        }
    }

    public class TutorialMessage : IMessage
    {
        public TutorialStep Progress { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public QuestProgressMessage? QuestMessage { get; set; } // Optionally attach to a quest progress message for combined displaying

        public TutorialMessage(TutorialStep progress, string title, string text, QuestProgressMessage? questMessage = null)
        {
            Progress = progress;
            Title = title;
            Text = text;
            QuestMessage = questMessage;
        }

        string IMessage.BuildMessage()
        {
            return $"Tutorial: {Title} - {Text}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug; // Tutorial messages are displayed directly
        }
    }
}
