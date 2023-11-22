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
    public class TutorialSystem : AbstractSystem
    {
        const int ItemCountForSelling = 12;
        const int CoinsForReforging = 200;

        public const string FinalStoryDungeon = "THRESHOLD";
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

            var addTutorialProgress = (TutorialStep step, string title, string text, QuestProgressMessage questMessage) =>
            {
                if (!globalProgress.Data.TutorialProgress.Contains(step))
                {
                    coordinator.PostMessage(this, new TutorialMessage(step, title, text, questMessage));
                    globalProgress.Data.TutorialProgress.Add(step);
                }
            };

            foreach (var message in coordinator.FetchMessagesByType<QuestProgressMessage>())
            {
                if (message.Quest == QuestId.GettingStarted)
                {
                    switch ((Components.QuestStates.GettingStarted)message.Progress)
                    {
                        case Components.QuestStates.GettingStarted.Inventory:
                            addTutorialProgress(TutorialStep.Inventory, "Level Up!",
                                "You have unlocked the inventory. Double click on an item in your inventory to equip it.", message);
                            break;
                        case Components.QuestStates.GettingStarted.Abilities:
                            addTutorialProgress(TutorialStep.Abilities, "Live and Learn",
                                "The more you use weapons of one type, the better you will become at handling them. Watch your " +
                                "attack speed increase along with your ability level.", message);
                            break;
                        case Components.QuestStates.GettingStarted.Outside:
                            addTutorialProgress(TutorialStep.MobAttacks, "Training is Over",
                                "From this point on, mobs are going to fight back. Watch the countdown " +
                                "near the mob. If time runs out, you lose the fight.", message);
                            break;
                        case Components.QuestStates.GettingStarted.Armor:
                            addTutorialProgress(TutorialStep.Armor, "It's Dangerous to Go Alone",
                                "Wearing armor slows down the countdown during fights but also encumbers your character, reducing attack speed. " +
                                "Heavier armor means more encumbrance, but also better protection.", message);
                            break;
                        case Components.QuestStates.GettingStarted.Travel:
                            addTutorialProgress(TutorialStep.Travel, "Freedom of Movement", 
                                "Click the arrow buttons to move between zones. Higher level areas become accessible after defeating" +
                                "a mob in the previous zone. Checking 'Proceed when possible' will advance to the next zone as a mob has been defeated." +
                                "\nUpon losing a fight, your character will automatically move down one area and 'Go on after win' is deactivated.", 
                                message);
                            break;
                        default:
                            break;
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
                globalProgress.Data.TutorialProgress.Add(TutorialStep.DungeonOpen);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.DungeonOpen, "A New Challenge!",
                    "Progressing in the wilderness will give you access to dungeons. A dungeon consists of several floors, each containing " +
                    "monsters that have to be defeated to proceed. They are more challenging than wilderness areas " +
                    "of the same level, but completing dungeons will grant powerful rewards. Losing a fight in the dungeon will get you sent " +
                    "back to the wilderness." +
                    $"\n  - Unlocked dungeon '{world.AreaKingdom.Dungeons[0].Name.Localize()}'")); // CornerCut: Assumes first dungeon is first to unlock
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
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.Finished)
                && globalProgress.Data.GetClearedDungeons().Contains(FinalStoryDungeon))
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.Finished);
                var time = globalProgress.Data.Playtime;
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Finished, "",
                    "Congratulations you cleared the final dungeon and completed the game." +
                    $"\nFeel free to keep grinding and earning achievements or create a new character to try for a faster time!"));                    
            }
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.Evasion)
                && (m_player.GetComponent<DefenseComponent>()?.Evasion ?? 0) > 0)
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.Evasion);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Evasion, "Travelling Light",
                    $"You have proven your prowess in unarmored combat. Fighting with no armor now grants 0.5 " +
                    $"points to you evasion rating per level for each owned achievement from the 'unarmored' line. " +
                    $"\n  - Evasion increases the length of time limits by 1% per point."));
            }
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.Unarmed)
                && (achievementComp?.Achievements.Count(a => a.Id.Contains(UnarmedKey) 
                    && a.Status == Achievements.AchievementStatus.Awarded) > 0))
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.Unarmed);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Unarmed, "Iron Fists",
                    $"You have proven your prowess in unarmed combat. Fighting without a weapon now grants 0.05 " +
                    $"base damage per level for each owned achievement from the 'unarmed' line."));
            }
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
                    .Any(m => ItemIdentifier.ExtractGenusIndex(m.Item.GetItemCode()) > 0))
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
            if (!globalProgress.Data.TutorialProgress.Contains(TutorialStep.Reforging)
                && (m_player.GetComponent<PlayerProgressComponent>()?.Data.TotalCoins ?? 0) > CoinsForReforging)
            {
                globalProgress.Data.TutorialProgress.Add(TutorialStep.Reforging);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Reforging, "Let's put those coins to use",
                    $"You can unburden yourself of some cumbersome coinage by reforging your items. This will reroll their rarity to a random value." +
                    $"As your crafting ability improves, higher rarity levels become available and their probability increases.\n" +
                    $"\n  - You can now spend coins to reforge the rarity of items"));
            }
            // Enable feature for player if reforging has been unlocked globally
            if (globalProgress.Data.TutorialProgress.Contains(TutorialStep.Reforging)
                && !(m_player.GetComponent<PlayerComponent>()?.AvailableFeatures?.Contains(GameFeature.Crafting) ?? true)
                && (m_player.GetComponent<PlayerProgressComponent>()?.Data.TotalCoins ?? 0) > CoinsForReforging)
            {
                m_player.GetComponent<PlayerComponent>()?.SetFeatureState(GameFeature.Crafting, true);
                coordinator.PostMessage(this, new FeatureStateMessage(GameFeature.Crafting, true));
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

/*    public class Trigger
    {
        public enum TriggerState { Inactive, Ready, Triggered }

        public TriggerState State { get { return m_state; } }

        TriggerState m_state;
    }*/
}
