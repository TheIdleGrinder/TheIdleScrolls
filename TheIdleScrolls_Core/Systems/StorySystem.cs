using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Systems
{
    public class StorySystem : AbstractSystem
    {
        const double slopeDuration = 10.0;
        const double pauseDuration = 5.0;

        Entity? m_player = null;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;
            var storyComp = m_player.GetComponent<StoryProgressComponent>();
            if (storyComp == null)
                return;

            if (storyComp.FinalFight.State != FinalFight.Status.NotStarted && world.DungeonId != TutorialSystem.FinalStoryDungeon)
            {
                // Reset game speed
                world.GameOver = false;
                world.SpeedMultiplier = 1.0;
                storyComp.FinalFight.State = FinalFight.Status.NotStarted;                
            }

            if (storyComp.FinalFight.State == FinalFight.Status.NotStarted)
            {
                if (world.IsInDungeon()
                    && world.DungeonId == TutorialSystem.FinalStoryDungeon
                    && world.RemainingEnemies == 1
                    && coordinator.GetEntities<MobComponent>().FirstOrDefault() != null)
                {
                    var mob = coordinator.GetEntities<MobComponent>().FirstOrDefault();
                    if (mob == null)
                        throw new Exception("Final mob was not found");

                    storyComp.FinalFight.State = FinalFight.Status.Slowing;
                    storyComp.FinalFight.StartTime = DateTime.Now;

                    // Transform mob into final boss
                    mob.AddComponent(new NameComponent(Properties.LocalizedStrings.BOSS_FINAL_DEMON));
                    mob.AddComponent(new MobDamageComponent(1.0));

                    // Set HP high enought to prevent deafeating the boss
                    var attackComp = m_player.GetComponent<AttackComponent>();
                    if (attackComp != null)
                    {
                        double dps = attackComp.RawDamage / attackComp.Cooldown.Duration;
                        mob.AddComponent(new LifePoolComponent((int)(dps * slopeDuration)));
                    }

                    var defenseComp = m_player.GetComponent<DefenseComponent>();
                    if (defenseComp != null)
                    {
                        double multi = 1.0 + (defenseComp.Armor / 100.0); // CornerCut: Have central function for bonus
                        world.TimeLimit.Reset(1.0 * slopeDuration / multi);
                    }
                }
            } 
            else if (storyComp.FinalFight.State == FinalFight.Status.Slowing)
            {
                double duration = (DateTime.Now - storyComp.FinalFight.StartTime).Seconds;
                world.SpeedMultiplier = 1.0 - Math.Min(Math.Pow(duration, 0.25) / Math.Pow(slopeDuration, 0.25), 1.0);
                if (duration >= slopeDuration)
                {
                    storyComp.FinalFight.State = FinalFight.Status.Pause;
                }
            }
            else if (storyComp.FinalFight.State == FinalFight.Status.Pause)
            {
                double duration = (DateTime.Now - storyComp.FinalFight.StartTime).Seconds - slopeDuration;
                if (duration >= pauseDuration)
                {
                    storyComp.FinalFight.State = FinalFight.Status.End;
                }
            }
            else if (storyComp.FinalFight.State == FinalFight.Status.End)
            {
                var progComp = m_player.GetComponent<PlayerProgressComponent>();
                double playtime = (progComp != null) ? progComp.Data.Playtime : 0;
                bool first = !m_player.GetComponent<PlayerProgressComponent>()?.Data.DungeonTimes.ContainsKey(world.DungeonId) ?? true;
                coordinator.PostMessage(this, new ManualSaveRequest());
                coordinator.PostMessage(this, new DungeonCompletedMessage(world.DungeonId.Localize(), first));
                coordinator.PostMessage(this, new TutorialMessage(TutorialStep.Finished, 
                    Properties.LocalizedStrings.STORY_END_TITLE, 
                    String.Format(Properties.LocalizedStrings.STORY_END_TEXT, playtime)));
                storyComp.FinalFight.State = FinalFight.Status.Finished;
                world.GameOver = true;
            }
        }
    }
}
