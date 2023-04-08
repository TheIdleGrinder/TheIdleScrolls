using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using QuestStates = TheIdleScrolls_Core.Components.QuestStates;

namespace TheIdleScrolls_Core.Systems
{
    public class StorySystem : AbstractSystem
    {
        const double slopeDuration = 10.0;
        const double pauseDuration = 5.0;

        Entity? m_player = null;

        bool m_firstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            
            HandleFinalFight(world, coordinator);

            m_firstUpdate = false;
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
                progress = QuestStates.FinalFight.NotStarted;
            if (progress < 0 || progress == QuestStates.FinalFight.Finished)
                return;
            
            //if (progress != QuestStates.FinalFight.NotStarted && world.DungeonId != TutorialSystem.FinalStoryDungeon)
            //{
            //    // Reset game speed
            //    world.GameOver = false;
            //    world.SpeedMultiplier = 1.0;
            //    storyComp.SetQuestProgress(QuestId.FinalFight, (int)QuestStates.FinalFight.NotStarted);
            //    var travelComp = m_player.GetComponent<TravellerComponent>();
            //    if (travelComp != null)
            //    {
            //        travelComp.Active = true;
            //    }
            //}

            if (progress == QuestStates.FinalFight.NotStarted)
            {
                if (world.IsInDungeon()
                    && world.DungeonId == TutorialSystem.FinalStoryDungeon
                    && world.RemainingEnemies == 1
                    && coordinator.GetEntities<MobComponent>().FirstOrDefault() != null)
                {
                    storyComp.SetQuestProgress(QuestId.FinalFight, (int)QuestStates.FinalFight.Slowing);
                    storyComp.FinalFight.StartTime = DateTime.Now;

                    // Transform mob into final boss
                    var mob = coordinator.GetEntities<MobComponent>().FirstOrDefault();
                    if (mob == null)
                        throw new Exception("Final mob was not found");
                    mob.AddComponent(new NameComponent(Properties.LocalizedStrings.BOSS_FINAL_DEMON));
                    mob.AddComponent(new MobDamageComponent(1.0));
                    world.TimeLimit.Reset();

                    ScaleMobHpAndTimeLimit(m_player, mob, world);
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

                // Prevent player from fleeing
                var travelComp = m_player.GetComponent<TravellerComponent>();
                if (travelComp != null)
                {
                    travelComp.Active = false;
                    coordinator.PostMessage(this, new StoryProgressMessage());
                }

                if (duration >= slopeDuration)
                {
                    storyComp.SetQuestProgress(QuestId.FinalFight, (int)QuestStates.FinalFight.Pause);
                }
            }
            else if (progress == QuestStates.FinalFight.Pause)
            {
                double duration = (DateTime.Now - storyComp.FinalFight.StartTime).Seconds - slopeDuration;
                if (duration >= pauseDuration)
                {
                    storyComp.SetQuestProgress(QuestId.FinalFight, (int)QuestStates.FinalFight.End);
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
                storyComp.SetQuestProgress(QuestId.FinalFight, (int)QuestStates.FinalFight.Finished);
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
    public class StoryProgressMessage : IMessage
    {
        string IMessage.BuildMessage()
        {
            return "Story was progressed";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }

}
