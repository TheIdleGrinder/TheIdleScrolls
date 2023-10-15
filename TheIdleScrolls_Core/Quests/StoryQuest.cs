using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Systems;

using QuestStates = TheIdleScrolls_Core.Components.QuestStates;

namespace TheIdleScrolls_Core.Quests
{
    internal class StoryQuest : AbstractQuest
    {
        const double slopeDuration = 10.0;
        const double pauseDuration = 5.0;

        bool m_firstUpdate = true;

        public override QuestId GetId()
        {
            return QuestId.FinalFight;
        }

        public override void UpdateEntity(Entity entity, Coordinator coordinator, World world, double dt, Action<IMessage> postMessageCallback)
        {
            var storyComp = entity.GetComponent<QuestProgressComponent>();
            if (storyComp == null)
                return;

            var progress = (QuestStates.FinalFight)storyComp.GetQuestProgress(QuestId.FinalFight);
            // Reset state of unfinished final fight on first update. Necessary to avoid being 'trapped' upon restarting game during the final fight
            if (progress != QuestStates.FinalFight.Finished && m_firstUpdate)
            {
                progress = QuestStates.FinalFight.NotStarted;
                storyComp.SetQuestProgress(QuestId.FinalFight, progress);
                m_firstUpdate = false;
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

                    ScaleMobHpAndTimeLimit(entity, mob, world);

                    // Prevent player from fleeing
                    var travelComp = entity.GetComponent<TravellerComponent>();
                    if (travelComp != null)
                    {
                        travelComp.Active = false;
                        postMessageCallback(new QuestProgressMessage(QuestId.FinalFight, (int)QuestStates.FinalFight.Slowing));
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
                    ScaleMobHpAndTimeLimit(entity, mob, world);
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
                var progComp = entity.GetComponent<PlayerProgressComponent>();
                double playtime = (progComp != null) ? progComp.Data.Playtime : 0;
                bool first = !entity.GetComponent<PlayerProgressComponent>()?.Data.DungeonTimes.ContainsKey(world.DungeonId) ?? true;
                postMessageCallback(new ManualSaveRequest());
                postMessageCallback(new DungeonCompletedMessage(world.DungeonId, first));
                postMessageCallback(new TutorialMessage(TutorialStep.Finished,
                    Properties.LocalizedStrings.STORY_END_TITLE,
                    String.Format(Properties.LocalizedStrings.STORY_END_TEXT, playtime)));
                storyComp.SetQuestProgress(QuestId.FinalFight, QuestStates.FinalFight.Finished);
                world.GameOver = true;
            }
            m_firstUpdate = false;
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
}
