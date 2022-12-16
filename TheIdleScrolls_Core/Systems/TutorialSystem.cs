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
    public enum TutorialProgress { Start, Inventory }

    public class TutorialSystem : AbstractSystem
    {
        TutorialProgress m_progress = TutorialProgress.Start;

        Entity? m_player = null;

        bool m_firstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == null)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;

            // Determine previous progress
            if (m_firstUpdate)
            {
                m_firstUpdate = false;

                if (m_player.HasComponent<InventoryComponent>())
                    m_progress = TutorialProgress.Inventory;
            }

            // Evaluate conditions for current tutorial stage
            if (m_progress == TutorialProgress.Start)
            {
                int lvl = m_player.GetComponent<LevelComponent>()?.Level ?? 0;
                if (lvl >= 2)
                {
                    InventoryComponent invComp = new();
                    List<string> weapons = new() { "SBL0", "LBL0", "AXE0", "BLN0", "POL0", "LAR0", "HAR0" };
                    ItemFactory factory = new();

                    m_player.AddComponent(invComp);
                    m_player.AddComponent(new EquipmentComponent());

                    foreach (var weaponCode in weapons)
                    {
                        Entity? weapon = factory.ExpandCode(weaponCode);
                        if (weapon != null)
                        {
                            coordinator.AddEntity(weapon);
                            coordinator.PostMessage(this, new ItemReceivedMessage(m_player, weapon));
                        }
                    }
                    
                    m_progress = TutorialProgress.Inventory;
                    coordinator.PostMessage(this,
                        new TutorialMessage(m_progress, "Level Up!", "You have unlocked the inventory. Time to gear up!"));
                }
            } else if (false)
            {
                // ...
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
