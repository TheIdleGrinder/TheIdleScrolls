using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrolls_Core.Quests
{
    internal class EndgameQuest : AbstractQuest
    {
        enum States { NotStarted, Void, Endgame, UberEndgame, Finished }

        bool FirstUpdate = true;

        public override QuestId GetId()
        {
            return QuestId.Endgame;
        }

        public override void UpdateEntity(Entity entity, Coordinator coordinator, World world, double dt, Action<IMessage> postMessageCallback)
        {
            var questComp = entity.GetComponent<QuestProgressComponent>();
            if (questComp == null)
                return;

            States state = (States)questComp.GetQuestProgress(GetId(), 0);
            
            if (state == States.NotStarted)
            {
                if (!FirstUpdate)
                {
                    return; // Quest begins immediately after starting the game for the first time after finishing the story
                }
                FirstUpdate = false;
                var dungeons = entity.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeons() ?? [];
                if (dungeons.Contains(DungeonIds.Threshold))
                {
                    questComp.SetQuestProgress(GetId(), (int)States.Void);
                    postMessageCallback(new QuestProgressMessage(GetId(), (int)States.Void, 
                        Properties.Quests.Endgame_Start, 
                        Properties.Quests.Endgame_Title));
                    postMessageCallback(new QuestProgressMessage(GetId(), (int)States.Void, 
                        Properties.Quests.Endgame_Void,
                        Properties.Quests.Endgame_Title));
                }
                else
                {
                    return; // Story is not over
                }
            }

            if (state == States.Void)
            {
                var dungeons = entity.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeonLevels() ?? [];
                if (dungeons.Contains((DungeonIds.Void, DungeonLevels.LevelVoidMax)))
                {
                    questComp.SetQuestProgress(GetId(), (int)States.Endgame);
                    postMessageCallback(new QuestProgressMessage(GetId(), (int)States.Endgame, 
                        Properties.Quests.Endgame_Pinnacle,
                        Properties.Quests.Endgame_Title));
                }
            }

            if (state == States.Endgame)
            {
                string[] pinnacleIds = [DungeonIds.EndgameAges, 
                                        DungeonIds.EndgameMagic, 
                                        DungeonIds.EndgamePyramid];
                var dungeons = entity.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeons() ?? [];
                if (pinnacleIds.All(dungeons.Contains))
                {
                    questComp.SetQuestProgress(GetId(), (int)States.UberEndgame);
                    postMessageCallback(new QuestProgressMessage(GetId(), (int)States.UberEndgame, 
                        Properties.Quests.Endgame_Finished,
                        Properties.Quests.Endgame_Title));

                    // Give reward title
                    var titleComp = entity.GetComponent<TitleBearerComponent>();
                    if (titleComp != null)
                    {
                        int deaths = entity.GetComponent<PlayerProgressComponent>()?.Data.Losses ?? 1;
                        titleComp.AddTitle(deaths == 0 ? Titles.ConquerorHC : Titles.Conqueror);
                    }
                }
            }

            if (state == States.UberEndgame)
            {
                questComp.SetQuestProgress(GetId(), (int)States.Finished);
                postMessageCallback(new QuestProgressMessage(GetId(), (int)States.Finished,
                    Properties.Quests.Endgame_UberEndgame,
                    "Just one more thing..."));
            }
        }
    }
}
