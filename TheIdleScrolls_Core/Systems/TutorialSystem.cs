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
    public enum TutorialProgress { Start, Inventory, MobAttacks, Armor, Abilities, Travel }

    public class TutorialSystem : AbstractSystem
    {
        static int LvlInventory = 2;
        static int LvlMobAttacks = 5;
        static int LvlArmor = 10;
        static int LvlAbilities = 15;
        static int LvlTravel = 20;

        TutorialProgress m_progress = TutorialProgress.Start;

        Entity? m_player = null;

        bool m_firstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == null)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;

            int lvl = m_player.GetComponent<LevelComponent>()?.Level ?? 0;

            // Determine previous progress
            if (m_firstUpdate)
            {
                m_firstUpdate = false;

                if (m_player.HasComponent<InventoryComponent>())
                    m_progress = TutorialProgress.Inventory;
                if (m_progress == TutorialProgress.Inventory && lvl >= LvlMobAttacks)
                    m_progress = TutorialProgress.MobAttacks;
                if (m_progress == TutorialProgress.MobAttacks && lvl >= LvlArmor)
                    m_progress = TutorialProgress.Armor;
                if (m_progress == TutorialProgress.Armor && lvl >= LvlAbilities)
                    m_progress = TutorialProgress.Abilities;
                if (m_progress == TutorialProgress.Abilities && lvl >= LvlTravel)
                    m_progress = TutorialProgress.Travel;
            }

            // Evaluate conditions for current tutorial stage
            if (m_progress == TutorialProgress.Start && lvl >= LvlInventory)
            {
                InventoryComponent invComp = new();
                List<string> weapons = new() { "SBL0", "LBL0", "AXE0", "BLN0", "POL0", "LAR0", "HAR0" };
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
                    
                m_progress = TutorialProgress.Inventory;
                coordinator.PostMessage(this,
                    new TutorialMessage(m_progress, "Level Up!", 
                    $"You have unlocked the inventory. Time to gear up!\n  - Unlocked inventory{itemString}"));
            }
            else if (m_progress == TutorialProgress.Inventory && lvl >= LvlMobAttacks)
            {
                m_progress = TutorialProgress.MobAttacks;
                coordinator.PostMessage(this,
                    new TutorialMessage(m_progress, "Training is over",
                    $"From this point on, mobs are going to fight back. Watch the countdown near the mob. If time runs out, you lose the fight."));
            
            }
            else if (m_progress == TutorialProgress.MobAttacks && lvl >= LvlArmor)
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

                m_progress = TutorialProgress.Armor;
                coordinator.PostMessage(this,
                    new TutorialMessage(m_progress, "It's dangerous to go alone", 
                    $"Those mobs are getting nasty. Use armor to slow down the countdown during fights.{itemString}"));
            }
            else if (m_progress == TutorialProgress.Armor && lvl >= LvlAbilities)
            {
                m_progress = TutorialProgress.Abilities;
                coordinator.PostMessage(this,
                    new TutorialMessage(m_progress, "Live and learn",
                    $""));

            }
        }
    }

    public class TutorialMessage : IMessage
    {
        public TutorialProgress Progress { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public TutorialMessage(TutorialProgress progress, string title, string text)
        {
            Progress = progress;
            Title = title;
            Text = text;
        }

        string IMessage.BuildMessage()
        {
            return $"Tutorial: {Title} - {Text}";
        }
    }

/*    public class Trigger
    {
        public enum TriggerState { Inactive, Ready, Triggered }

        public TriggerState State { get { return m_state; } }

        TriggerState m_state;
    }*/
}
