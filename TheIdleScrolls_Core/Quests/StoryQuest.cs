using MiniECS;
using System;
using System.Collections.Generic;
using System.Data;
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
        enum States
        {
            NotStarted,
            QuestReceived    = 01,
            LighthouseOpen   = 05, LighthouseFinished   = 10,
            TempleOpen       = 15, TempleFinished       = 20,
            CastleOpen       = 25, CastleFinished       = 30,
            LabyrinthOpen    = 35, LabyrinthFinished    = 40,
            Lighthouse2Open  = 45, Lighthouse2Finished  = 50,
            ThresholdOpen    = 55, ThresholdFinished    = 60
        }

        public enum FinalFightState { None = -1, NotStarted, Slowing, Pause, End, Finished }

        const int startLevel = 20;
        const double slopeDuration = 10.0;
        const double pauseDuration = 5.0;

        public override QuestId GetId()
        {
            return QuestId.Story;
        }

        public override void UpdateEntity(Entity entity, Coordinator coordinator, World world, double dt, Action<IMessage> postMessageCallback)
        {
            const string FightStateKey = "FinalFight_State";
            const string StartTimeKey = "FinalFight_StartTime";

            var storyComp = entity.GetComponent<QuestProgressComponent>();
            var locationComp = entity.GetComponent<LocationComponent>();
            if (storyComp == null || locationComp == null)
                return;

            var UpdateState = (States state, string message) =>
            {
                storyComp.SetQuestProgress(GetId(), state);
                postMessageCallback(new QuestProgressMessage(GetId(), (int)state, message == string.Empty ? null : message));
            };
            
            var progress = (States)storyComp.GetQuestProgress(GetId());
            var openDungeons = entity.GetComponent<TravellerComponent>()?.AvailableDungeons ?? new();
            var completedDungeons = entity.GetComponent<PlayerProgressComponent>()?.Data?.GetClearedDungeons() ?? new();

            // CornerCut: List of open dungeons is empty during the first frame, so some messages might be skipped when loading characters
            //  that were played before the current version of the story quest

            if (progress < States.QuestReceived && (entity.GetComponent<TravellerComponent>()?.MaxWilderness ?? 0) >= startLevel)
            {
                UpdateState(States.QuestReceived, Properties.Quests.Story_QuestReceived);
            }
            if (progress < States.LighthouseOpen && openDungeons.Contains(Definitions.DungeonIds.Lighthouse))
            {
                UpdateState(States.LighthouseOpen, Properties.Quests.Story_LighthouseOpen);
            }
            if (progress < States.LighthouseFinished && completedDungeons.Contains(Definitions.DungeonIds.Lighthouse))
            {
                UpdateState(States.LighthouseFinished, Properties.Quests.Story_LighthouseFinished);
            }

            if (progress < States.TempleOpen && openDungeons.Contains(Definitions.DungeonIds.Temple))
            {
                UpdateState(States.TempleOpen, Properties.Quests.Story_TempleOpen);
            }
            if (progress < States.TempleFinished && completedDungeons.Contains(Definitions.DungeonIds.Temple))
            {
                UpdateState(States.TempleFinished, Properties.Quests.Story_TempleFinished);
            }

            if (progress < States.CastleOpen && openDungeons.Contains(Definitions.DungeonIds.CultistCastle))
            {
                UpdateState(States.CastleOpen, Properties.Quests.Story_CastleOpen);
            }
            if (progress < States.CastleFinished && completedDungeons.Contains(Definitions.DungeonIds.CultistCastle))
            {
                UpdateState(States.CastleFinished, Properties.Quests.Story_CastleFinished);
            }

            if (progress < States.LabyrinthOpen && openDungeons.Contains(Definitions.DungeonIds.Labyrinth))
            {
                UpdateState(States.LabyrinthOpen, Properties.Quests.Story_LabyrinthOpen);
            }
            if (progress < States.LabyrinthFinished && completedDungeons.Contains(Definitions.DungeonIds.Labyrinth))
            {
                UpdateState(States.LabyrinthFinished, Properties.Quests.Story_LabyrinthFinished);
            }

            if (progress < States.Lighthouse2Open && openDungeons.Contains(Definitions.DungeonIds.ReturnToLighthouse))
            {
                UpdateState(States.Lighthouse2Open, Properties.Quests.Story_Lighthouse2Open);
            }
            if (progress < States.Lighthouse2Finished && completedDungeons.Contains(Definitions.DungeonIds.ReturnToLighthouse))
            {
                UpdateState(States.Lighthouse2Finished, Properties.Quests.Story_Lighthouse2Finished);
            }

            if (progress < States.ThresholdOpen && openDungeons.Contains(Definitions.DungeonIds.Threshold))
            {
                UpdateState(States.ThresholdOpen, Properties.Quests.Story_ThresholdOpen);
            }

            if (progress == States.ThresholdOpen)
            {
                FinalFightState ffState = storyComp.RetrieveTemporaryData<FinalFightState>(FightStateKey);
                if (ffState == FinalFightState.NotStarted
                    && locationComp.InDungeon
                    && locationComp.DungeonId == Definitions.DungeonIds.Threshold
                    && locationComp.RemainingEnemies == 1
                    && coordinator.GetEntities<MobComponent>().FirstOrDefault() != null)
                {
                    storyComp.StoreTemporaryData(FightStateKey, FinalFightState.Slowing);
                    storyComp.StoreTemporaryData(StartTimeKey, DateTime.Now);

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
                    }
                }
                else if (ffState == FinalFightState.Slowing)
                {
                    DateTime startTime = storyComp.RetrieveTemporaryData<DateTime>(StartTimeKey);
                    double duration = (DateTime.Now - startTime).Seconds;
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
                        storyComp.StoreTemporaryData(FightStateKey, FinalFightState.Pause);
                    }
                }
                else if (ffState == FinalFightState.Pause)
                {
                    DateTime startTime = storyComp.RetrieveTemporaryData<DateTime>(StartTimeKey);
                    double duration = (DateTime.Now - startTime).Seconds - slopeDuration;
                    if (duration >= pauseDuration)
                    {
                        storyComp.StoreTemporaryData(FightStateKey, FinalFightState.End);
                    }
                }
                else if (ffState == FinalFightState.End)
                {
                    var progComp = entity.GetComponent<PlayerProgressComponent>();
                    double dblTime = (progComp != null) ? progComp.Data.Playtime : 0;
                    var playtime = TimeSpan.FromSeconds(dblTime).ToString(@"hh\:mm\:ss");
                    bool first = !entity.GetComponent<PlayerProgressComponent>()?.Data.DungeonTimes.ContainsKey(locationComp.DungeonId) ?? true;
                    postMessageCallback(new ManualSaveRequest());
                    postMessageCallback(new DungeonCompletedMessage(locationComp.DungeonId, first));
                    postMessageCallback(new TutorialMessage(TutorialStep.Finished,
                        Properties.LocalizedStrings.STORY_END_TITLE,
                        String.Format(Properties.Quests.Story_ThresholdFinished, playtime)));
                    UpdateState(States.ThresholdFinished, String.Empty);
                    world.GameOver = true;                    
                }
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
}
