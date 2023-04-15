using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;
using QuestStates = TheIdleScrolls_Core.Components.QuestStates;

namespace TheIdleScrolls_Core.Systems
{

    public class QuestSystem : AbstractSystem
    {
        const double slopeDuration = 10.0;
        const double pauseDuration = 5.0;

        Entity? m_player = null;

        bool m_firstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();

            HandleGettingStarted(world, coordinator);
            HandleFinalFight(world, coordinator);

            m_firstUpdate = false;
        }

        void HandleGettingStarted(World world, Coordinator coordinator)
        {
            const int LvlInventory = 2;
            const int LvlMobAttacks = 6;
            const int LvlArmor = 8;
            const int LvlAbilities = 4;
            const int LvlTravel = 10;

            

            if (m_player == null)
                return;

            int level = m_player.GetLevel();

            var playerComp = m_player.GetComponent<PlayerComponent>();
            var storyComp = m_player.GetComponent<QuestProgressComponent>();
            if (playerComp == null || storyComp == null)
                return;

            var setFeatureState = (GameFeature feature, bool state) => 
            {
                playerComp.SetFeatureState(feature, state);
                coordinator.PostMessage(this, new FeatureStateMessage(feature, state));
            };

            var setQuestState = (QuestId quest, int progress, string message) =>
            {
                storyComp.SetQuestProgress(quest, progress);
                coordinator.PostMessage(this, new QuestProgressMessage(quest, progress, message));
            };

            var progress = (QuestStates.GettingStarted)storyComp.GetQuestProgress(QuestId.GettingStarted);
            if (progress < QuestStates.GettingStarted.Inventory && level >= LvlInventory)
            {
                if (!m_player.HasComponent<InventoryComponent>())
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

                    setQuestState(QuestId.GettingStarted, (int)QuestStates.GettingStarted.Inventory,
                        $"You have unlocked the inventory. Time to gear up!" +
                        $"\nDouble click on an item in your inventory to equip it." +
                        $"\n  - Unlocked inventory{itemString}");
                }
                storyComp.SetQuestProgress(QuestId.GettingStarted, QuestStates.GettingStarted.Inventory);
                setFeatureState(GameFeature.Inventory, true);
            }

            if (progress < QuestStates.GettingStarted.Outside && level >= LvlMobAttacks)
            {
                setQuestState(QuestId.GettingStarted, (int)QuestStates.GettingStarted.Outside,
                    $"From this point on, mobs are going to fight back. Watch the countdown near the mob. If time runs out, you lose the fight.");
            }

            if (progress < QuestStates.GettingStarted.Armor && level >= LvlArmor)
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

                setQuestState(QuestId.GettingStarted, (int)QuestStates.GettingStarted.Armor,
                    $"Those mobs are getting nasty. Use armor to slow down the countdown during fights. " +
                    $"Wearing armor encumbers your character, reducing attack speed. " +
                    $"Heavier armor means more encumbrance, but also better protection." +
                    $"{itemString}");
                setFeatureState(GameFeature.Armor, true);
            }

            if (progress < QuestStates.GettingStarted.Abilities && level >= LvlAbilities)
            {
                setQuestState(QuestId.GettingStarted, (int)QuestStates.GettingStarted.Abilities,
                    $"The more you use weapons of one type, the better you will become at handling them. Watch your " +
                    $"attack speed increase along with your ability level.");
                setFeatureState(GameFeature.Abilities, true);
            }

            if (progress < QuestStates.GettingStarted.Travel && level >= LvlTravel)
            {
                if (m_player.HasComponent<TravellerComponent>())
                {
                    m_player.AddComponent(new TravellerComponent());
                    setQuestState(QuestId.GettingStarted, (int)QuestStates.GettingStarted.Travel,
                        $"You can now travel between areas. Pick a spot to grind or push forward to unlock higher zones." +
                        $"\n  - Unlocked manual travel between areas");
                }
                storyComp.SetQuestProgress(QuestId.GettingStarted, QuestStates.GettingStarted.Travel);
                setFeatureState(GameFeature.Travel, true);
            }
        }

        void HandleFinalFight(World world, Coordinator coordinator)
        {
            if (m_player == null)
                return;

            var storyComp = m_player.GetComponent<QuestProgressComponent>();
            if (storyComp == null)
                return;

            var progress = (QuestStates.FinalFight)storyComp.GetQuestProgress(QuestId.FinalFight);
            // Reset state of unfinished final fight on first update. Necessary to avoid being 'trapped' upon restarting game during the final fight
            if (progress != QuestStates.FinalFight.Finished && m_firstUpdate)
            {
                progress = QuestStates.FinalFight.NotStarted;
                storyComp.SetQuestProgress(QuestId.FinalFight, progress);
            }
            if (progress < 0 || progress == QuestStates.FinalFight.Finished)
                return;
            
            if (progress == QuestStates.FinalFight.NotStarted)
            {
                if (world.IsInDungeon()
                    && world.DungeonId == TutorialSystem.FinalStoryDungeon
                    && world.RemainingEnemies == 1
                    && coordinator.GetEntities<MobComponent>().FirstOrDefault() != null)
                {
                    storyComp.SetQuestProgress(QuestId.FinalFight, QuestStates.FinalFight.Slowing);
                    storyComp.FinalFight.StartTime = DateTime.Now;

                    // Transform mob into final boss
                    var mob = coordinator.GetEntities<MobComponent>().FirstOrDefault();
                    if (mob == null)
                        throw new Exception("Final mob was not found");
                    mob.AddComponent(new NameComponent(Properties.LocalizedStrings.BOSS_FINAL_DEMON));
                    mob.AddComponent(new MobDamageComponent(1.0));
                    world.TimeLimit.Reset();

                    ScaleMobHpAndTimeLimit(m_player, mob, world);

                    // Prevent player from fleeing
                    var travelComp = m_player.GetComponent<TravellerComponent>();
                    if (travelComp != null)
                    {
                        travelComp.Active = false;
                        coordinator.PostMessage(this, new QuestProgressMessage(QuestId.FinalFight, (int)QuestStates.FinalFight.Slowing));
                    }
                }
            }
            else if (progress == QuestStates.FinalFight.Slowing)
            {
                double duration = (DateTime.Now - storyComp.FinalFight.StartTime).Seconds;
                world.SpeedMultiplier = 1.0 - Math.Min(Math.Pow(duration, 0.25) / Math.Pow(slopeDuration, 0.25), 1.0);

                // Rescale HP and time on gear change
                if (coordinator.MessageTypeIsOnBoard<ItemMovedMessage>())
                {
                    var mob = coordinator.GetEntities<MobComponent>().FirstOrDefault();
                    if (mob == null)
                        throw new Exception("Final mob was not found");
                    ScaleMobHpAndTimeLimit(m_player, mob, world);
                }

                if (duration >= slopeDuration)
                {
                    storyComp.SetQuestProgress(QuestId.FinalFight, QuestStates.FinalFight.Pause);
                }
            }
            else if (progress == QuestStates.FinalFight.Pause)
            {
                double duration = (DateTime.Now - storyComp.FinalFight.StartTime).Seconds - slopeDuration;
                if (duration >= pauseDuration)
                {
                    storyComp.SetQuestProgress(QuestId.FinalFight, QuestStates.FinalFight.End);
                }
            }
            else if (progress == QuestStates.FinalFight.End)
            {
                var progComp = m_player.GetComponent<PlayerProgressComponent>();
                double playtime = (progComp != null) ? progComp.Data.Playtime : 0;
                bool first = !m_player.GetComponent<PlayerProgressComponent>()?.Data.DungeonTimes.ContainsKey(world.DungeonId) ?? true;
                coordinator.PostMessage(this, new ManualSaveRequest());
                coordinator.PostMessage(this, new DungeonCompletedMessage(world.DungeonId.Localize(), first));
                coordinator.PostMessage(this, new TutorialMessage(TutorialStep.Finished,
                    Properties.LocalizedStrings.STORY_END_TITLE,
                    String.Format(Properties.LocalizedStrings.STORY_END_TEXT, playtime)));
                storyComp.SetQuestProgress(QuestId.FinalFight, QuestStates.FinalFight.Finished);
                world.GameOver = true;
            }
        }

        static void ScaleMobHpAndTimeLimit(Entity player, Entity mob, World world)
        {
            double baseMultiplier = 0.35;
            // Set HP high enough to prevent deafeating the boss
            var attackComp = player.GetComponent<AttackComponent>();
            if (attackComp != null)
            {
                var hpComp = mob.GetComponent<LifePoolComponent>() ?? new LifePoolComponent();
                double remaining = 1.0 * hpComp.Current / hpComp.Maximum;
                double dps = attackComp.RawDamage / attackComp.Cooldown.Duration;
                hpComp.Maximum = (int)(baseMultiplier * dps * slopeDuration);
                hpComp.Current = (int)(remaining * hpComp.Maximum);
                mob.AddComponent(hpComp);
            }

            // Set time limit to prevent the player from losing
            var defenseComp = player.GetComponent<DefenseComponent>();
            if (defenseComp != null)
            {
                double multi = 1.0 + (defenseComp.Armor / 100.0); // CornerCut: Have central function for bonus
                double targetDuration = slopeDuration / multi;
                world.TimeLimit.ChangeDuration(baseMultiplier * targetDuration);
            }
        }
    }

    public class StoryMessage : IMessage
    {
        readonly string content = "";

        public StoryMessage(string content)
        {
            this.content = content;
        }

        string IMessage.BuildMessage()
        {
            return content;
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.VeryHigh;
        }
    }

    /// <summary>
    /// Potentially placeholder. Used to notify other systems of the fact that a quest state has changed.
    /// </summary>
    public class QuestProgressMessage : IMessage
    {
        public QuestId Quest { get; }
        public int Progress { get; }
        public string? QuestMessage { get; }

        string IMessage.BuildMessage()
        {
            if (QuestMessage != null)
            {
                return $"{Quest}|{Progress}: {QuestMessage}";
            }
            else
            {
                return $"Quest progressed: {Quest}|{Progress}";
            }
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return (QuestMessage != null) 
                ? IMessage.PriorityLevel.VeryHigh 
                : IMessage.PriorityLevel.Debug;
        }

        public QuestProgressMessage(QuestId quest, int progress, string? message = null)
        {
            Quest = quest;
            Progress = progress;
            QuestMessage = message;
        }
    }

    public class FeatureStateMessage : IMessage
    {
        public GameFeature Feature { get; }
        public bool Enabled { get; }

        string IMessage.BuildMessage()
        {
            return $"Feature '{Feature}' has been {(Enabled ? "en" : "dis")}abled";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }

        public FeatureStateMessage(GameFeature feature, bool enabled)
        {
            Feature = feature;
            Enabled = enabled;
        }
    }

}
