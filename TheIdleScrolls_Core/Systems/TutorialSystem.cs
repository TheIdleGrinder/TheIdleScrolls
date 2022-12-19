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
        static int LvlInventory = 2;
        static int LvlMobAttacks = 6;
        static int LvlArmor = 10;
        static int LvlAbilities = 15;
        static int LvlTravel = 20;

        Entity? m_player = null;

        bool m_firstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == null)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            var progComp = m_player?.GetComponent<PlayerProgressComponent>();
            
            if (m_player == null || progComp == null)
                return;

            int lvl = m_player.GetComponent<LevelComponent>()?.Level ?? 0;

            // Determine previous progress
            if (m_firstUpdate)
            {
                m_firstUpdate = false;

                var progress = TutorialProgress.Start;
                if (m_player.HasComponent<InventoryComponent>())
                    progress = TutorialProgress.Inventory;
                if (progress == TutorialProgress.Inventory && lvl >= LvlMobAttacks)
                    progress = TutorialProgress.MobAttacks;
                if (progress == TutorialProgress.MobAttacks && lvl >= LvlArmor)
                    progress = TutorialProgress.Armor;
                if (progress == TutorialProgress.Armor && lvl >= LvlAbilities)
                    progress = TutorialProgress.Abilities;
                if (progress == TutorialProgress.Abilities && lvl >= LvlTravel)
                    progress = TutorialProgress.Travel;

                if (progress > progComp.Data.Progress)
                {
                    progComp.Data.Progress = progress;
                }
            }

            // Evaluate conditions for current tutorial stage
            if (progComp.Data.Progress == TutorialProgress.Start && lvl >= LvlInventory)
            {
                InventoryComponent invComp = new();
                List<string> weapons = new() { "SBL0", "LBL0", "AXE0", "BLN0", "POL0" };
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

                progComp.Data.Progress = TutorialProgress.Inventory;
                coordinator.PostMessage(this,
                    new TutorialMessage(progComp.Data.Progress, "Level Up!", 
                    $"You have unlocked the inventory. Time to gear up!\n  - Unlocked inventory{itemString}"));
            }
            else if (progComp.Data.Progress == TutorialProgress.Inventory && lvl >= LvlMobAttacks)
            {
                progComp.Data.Progress = TutorialProgress.MobAttacks;
                coordinator.PostMessage(this,
                    new TutorialMessage(progComp.Data.Progress, "Training is over",
                    $"From this point on, mobs are going to fight back. Watch the countdown near the mob. If time runs out, you lose the fight."));
            
            }
            else if (progComp.Data.Progress == TutorialProgress.MobAttacks && lvl >= LvlArmor)
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

                progComp.Data.Progress = TutorialProgress.Armor;
                coordinator.PostMessage(this,
                    new TutorialMessage(progComp.Data.Progress, "It's dangerous to go alone", 
                    $"Those mobs are getting nasty. Use armor to slow down the countdown during fights.{itemString}"));
            }
            else if (progComp.Data.Progress == TutorialProgress.Armor && lvl >= LvlAbilities)
            {
                progComp.Data.Progress = TutorialProgress.Abilities;
                coordinator.PostMessage(this,
                    new TutorialMessage(progComp.Data.Progress, "Live and learn",
                    $"\n  - Unlocked abilities"));

            }
            else if (progComp.Data.Progress == TutorialProgress.Abilities && lvl >= LvlTravel)
            {
                progComp.Data.Progress = TutorialProgress.Travel;
                coordinator.PostMessage(this,
                    new TutorialMessage(progComp.Data.Progress, "Freedom of movement",
                    $"\n  - Unlocked abilities"));

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
