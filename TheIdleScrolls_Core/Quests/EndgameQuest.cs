using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrolls_Core.Quests
{
    internal class EndgameQuest : AbstractQuest
    {
        enum States { NotStarted, Void, Pinnacle, Finished }

        bool FirstUpdate = true; // The quest first starts after starting the game for the first time after finishing the story

        public override QuestId GetId()
        {
            return QuestId.Endgame;
        }

        public override void UpdateEntity(Entity entity, Coordinator coordinator, World world, double dt, Action<IMessage> postMessageCallback)
        {
            var questComp = entity.GetComponent<QuestProgressComponent>();
            if (questComp == null)
                return;

            States state = (States)questComp.GetQuestProgress(GetId());
            if (!FirstUpdate && state == States.NotStarted)
            {
                return;
            }

            if (FirstUpdate)
            {
                FirstUpdate = false;
                var dungeons = entity.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeons() ?? [];
                if (dungeons.Contains(Definitions.DungeonIds.Threshold))
                {
                    questComp.SetQuestProgress(GetId(), (int)States.Void);
                    postMessageCallback(new QuestProgressMessage(GetId(), (int)States.Void, Properties.Quests.Endgame_Start));
                    postMessageCallback(new QuestProgressMessage(GetId(), (int)States.Void, Properties.Quests.Endgame_Void));
                }
                else
                {
                    return; // Story is not over
                }
            }

            if (state == States.Void)
            {
                var dungeons = entity.GetComponent<PlayerProgressComponent>()?.Data.GetClearedDungeonLevels() ?? [];
                if (dungeons.Contains((Definitions.DungeonIds.Void, Definitions.DungeonLevels.LevelVoidMax)))
                {
                    questComp.SetQuestProgress(GetId(), (int)States.Pinnacle);
                    postMessageCallback(new QuestProgressMessage(GetId(), (int)States.Pinnacle, Properties.Quests.Endgame_Pinnacle));
                }
            }

            if (state == States.Pinnacle)
            {

            }
        }
    }
}
