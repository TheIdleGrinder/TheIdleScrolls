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
    public class TutorialSystem : AbstractSystem
    {
        const int LvlInventory = 2;
        const int LvlMobAttacks = 6;
        const int LvlArmor = 8;
        const int LvlAbilities = 4;
        const int LvlTravel = 10;
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
            var progComp = m_player?.GetComponent<PlayerProgressComponent>();
            
            if (m_player == null || progComp == null)
                return;

            int lvl = m_player.GetComponent<LevelComponent>()?.Level ?? 0;

            // Determine previous progress
            if (m_firstUpdate)
            {
                m_firstUpdate = false;
            }

            var achievementComp = coordinator.GetEntities<AchievementsComponent>().FirstOrDefault()?.GetComponent<AchievementsComponent>();

            // Evaluate conditions for current tutorial stage
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Inventory) && lvl >= LvlInventory)
            {
                InventoryComponent invComp = new();
                List<ItemIdentifier> weapons = (new List<string>() { "SBL0", "LBL0", "AXE0", "BLN0", "POL0" })
                    .Select(i => new ItemIdentifier(i)).ToList();
                ItemFactory factory = new();

                m_player.AddComponent(invComp);
                m_player.AddComponent(new EquipmentComponent());
                string itemString = "";
                foreach (var weaponCode in weapons)
                {
                    Entity? weapon = factory.ExpandCode(weaponCode);
                    if (weapon != null)
                    {
                        itemString += $"\n  - Received '{weapon.GetName()}'";
                        coordinator.AddEntity(weapon);
                        coordinator.PostMessage(this, new ItemReceivedMessage(m_player, weapon));
                    }
                }

                progComp.Data.TutorialProgress.Add(TutorialStep.Inventory);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Inventory, "Level Up!", 
                    $"You have unlocked the inventory. Time to gear up!" +
                    $"\nDouble click on an item in your inventory to equip it." +
                    $"\n  - Unlocked inventory{itemString}"));
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.MobAttacks) && lvl >= LvlMobAttacks)
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.MobAttacks);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.MobAttacks, "Training is Over",
                    $"From this point on, mobs are going to fight back. Watch the countdown near the mob. If time runs out, you lose the fight."));
            
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Armor) && lvl >= LvlArmor)
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
                        coordinator.PostMessage(this, new ItemReceivedMessage(m_player, item));
                    }
                }

                progComp.Data.TutorialProgress.Add(TutorialStep.Armor);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Armor, "It's Dangerous to Go Alone", 
                    $"Those mobs are getting nasty. Use armor to slow down the countdown during fights. " +
                    $"Wearing armor encumbers your character, reducing attack speed. " +
                    $"Heavier armor means more encumbrance, but also better protection." +
                    $"{itemString}"));
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Abilities) && lvl >= LvlAbilities)
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.Abilities);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Abilities, "Live and Learn",
                    $"The more you use weapons of one type, the better you will become at handling them. Watch your " +
                    $"attack speed increase along with your ability level."));
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Travel) && lvl >= LvlTravel)
            {
                m_player.AddComponent(new TravellerComponent());
                progComp.Data.TutorialProgress.Add(TutorialStep.Travel);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Travel, "Freedom of Movement",
                    $"You can now travel between areas. Pick a spot to grind or push forward to unlock higher zones." +
                    $"\n  - Unlocked manual travel between areas"));
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Defeated)
                && progComp.Data.Losses == 1)
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.Defeated);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Defeated, "There's Always a Bigger Fish...",
                    "Time ran out and you lost this fight. Don't worry, though, a little more training should get you over the hump."));
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.DungeonOpen) 
                && coordinator.MessageTypeIsOnBoard<DungeonOpenedMessage>())
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.DungeonOpen);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.DungeonOpen, "A New Challenge!",
                    "Progressing in the wilderness will give you access to dungeons. A dungeon consists of several floors, each containing " +
                    "monsters that have to be defeated to proceed. They are more challenging than wilderness areas " +
                    "of the same level, but completing dungeons will grant powerful rewards. Losing a fight in the dungeon will get you sent " +
                    "back to the wilderness." +
                    $"\n  - Unlocked dungeon '{world.AreaKingdom.Dungeons[0].Name.Localize()}'")); // CornerCut: Assumes first dungeon is first to unlock
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.DungeonComplete)
                && coordinator.MessageTypeIsOnBoard<DungeonCompletedMessage>())
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.DungeonComplete);
                var itemName = coordinator.FetchMessagesByType<ItemReceivedMessage>().LastOrDefault()?.Item.GetName() ?? "??";
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.DungeonComplete, "Dungeon completed!",
                    "Good job, you completed your first dungeon and obtained a reward:" +
                    $"\n  - Received '{itemName}'"));
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Finished)
                && progComp.Data.GetClearedDungeons().Contains(FinalStoryDungeon))
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.Finished);
                var time = progComp.Data.Playtime;
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Finished, "",
                    "Congratulations you cleared the final dungeon and completed the game." +
                    $"\n  Playtime: {time:0} seconds" +
                    $"\n\nFeel free to keep grinding and earning achievements or reset your character to try for a faster time!"));
                    
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Evasion)
                && (m_player.GetComponent<DefenseComponent>()?.Evasion ?? 0) > 0)
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.Evasion);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Evasion, "Travelling Light",
                    $"You have proven your prowess in unarmored combat. Fighting with no armor now grants 0.5 " +
                    $"points to you evasion rating per level for each owned achievement from the 'unarmored' line. " +
                    $"\n  - Evasion increases the length of time limits by 1% per point."));
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Unarmed)
                && (achievementComp?.Achievements.Count(a => a.Id.Contains(UnarmedKey) 
                    && a.Status == Achievements.AchievementStatus.Awarded) > 0))
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.Unarmed);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Unarmed, "Iron Fists",
                    $"You have proven your prowess in unarmed combat. Fighting without a weapon now grants 0.05 " +
                    $"base damage per level for each owned achievement from the 'unarmed' line."));
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.FlatCircle)
                && (achievementComp?.Achievements.Count(a => a.Id.Contains(UnarmedKey)
                    && a.Id.Contains(UnarmoredKey)
                    && a.Status == Achievements.AchievementStatus.Awarded) > 0))
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.FlatCircle);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.FlatCircle, "A Flat Circle",
                    "Ironic, all this grinding just to get to a point where you use none of your gear or abilities."));
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.ItemFound)
                && coordinator.MessageTypeIsOnBoard<ItemReceivedMessage>())
            {
                if (coordinator.FetchMessagesByType<ItemReceivedMessage>()
                    .Any(m => ItemIdentifier.ExtractGenusIndex(m.Item.GetItemCode()) > 0))
                {
                    progComp.Data.TutorialProgress.Add(TutorialStep.ItemFound);
                    coordinator.PostMessage(this,
                        new TutorialMessage(TutorialStep.ItemFound, "Loot!",
                        $"You just found your first item. Defeated monsters will occasionally drop equipment which gets " +
                        $"stronger as you progress to higher zones."));
                }
            }
            // Skip tutorial for selling if the players sells an item before getting to it
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Selling) 
                && coordinator.MessageTypeIsOnBoard<CoinsChangedMessage>())
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.Selling);
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Selling)
                && (m_player.GetComponent<InventoryComponent>()?.ItemCount ?? 0) > ItemCountForSelling)
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.Selling);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Selling, "I'm not a... pack animal",
                    $"Items are piling up in your inventory. Selling them will make it less cluttered and also earn you some pretty coins.\n" +
                    $"\n  - You can sell items from you inventory using the context menu\n" +
                    $"\n  - Selecting one or more items and pressing DEL works as well"));
            }
            if (!progComp.Data.TutorialProgress.Contains(TutorialStep.Reforging)
                && (m_player.GetComponent<PlayerProgressComponent>()?.Data.TotalCoins ?? 0) > CoinsForReforging)
            {
                progComp.Data.TutorialProgress.Add(TutorialStep.Reforging);
                coordinator.PostMessage(this,
                    new TutorialMessage(TutorialStep.Reforging, "Let's put those coins to use",
                    $"You can unburden yourself of some cumbersome coinage by reforging your items. This will reroll their rarity to a random value." +
                    $"As your crafting ability improves, higher rarity levels become available and their probability increases.\n" +
                    $"\n  - You can reforge the rarity of items using the context menu\n" +
                    $"\n  - Selecting an items and pressing CTRL+F works as well"));
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
