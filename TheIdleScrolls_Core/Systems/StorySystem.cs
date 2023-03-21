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
        Entity? m_player = null;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
            if (m_player == null)
                return;
            var storyComp = m_player.GetComponent<StoryProgressComponent>();
            if (storyComp == null)
                return;

            if (storyComp.FinalFight.Started)
            {

            }
            else
            {
                if (world.IsInDungeon() 
                    && world.DungeonId == TutorialSystem.FinalStoryDungeon 
                    && world.RemainingEnemies == 1
                    && coordinator.GetEntities<MobComponent>().FirstOrDefault() != null)
                {
                    var mob = coordinator.GetEntities<MobComponent>().FirstOrDefault();
                    if (mob == null)
                        throw new Exception("Final mob was not found");

                    const double slopeDuration = 20.0;
                    storyComp.FinalFight.Started = true;
                    storyComp.FinalFight.StartTime = DateTime.Now;

                    // Transform mob into final boss
                    mob.AddComponent(new NameComponent(Properties.LocalizedStrings.BOSS_FINAL_DEMON));

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
                        world.TimeLimit.Reset(2.0 * slopeDuration / multi);
                    }
                }
            }
        }
    }
}
