using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Storage;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Systems
{
    public class SaveSystem : AbstractSystem
    {
        readonly DataAccessHandler m_dataAccessHandler;

        readonly Cooldown m_cooldown = new(30.0);

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            bool trigger = m_cooldown.Update(dt) > 0 
                || coordinator.MessageTypeIsOnBoard<ManualSaveRequest>()
                || coordinator.FetchMessagesByType<BattleStateChangedMessage>().Any(bm => bm.Battle.IsFinished);
            if (trigger)
            {
                Entity? player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault();
                if (player != null)
                {
                    if (player.HasComponent<ROMComponent>())
                    {
                        return;
                    }
                    m_dataAccessHandler.StoreEntity(player);
                    coordinator.PostMessage(this, new TextMessage("Game saved"));
                    m_cooldown.Reset();
                }
            }

            if (coordinator.MessageTypeIsOnBoard<AchievementStatusMessage>() 
                || coordinator.MessageTypeIsOnBoard<TutorialMessage>())
            {
                Entity? globalEntity = coordinator.GetEntities<AchievementsComponent>().FirstOrDefault();
                if (globalEntity != null)
                {
                    if (globalEntity.HasComponent<ROMComponent>())
                    {
                        return;
                    }
                    m_dataAccessHandler.StoreEntity(globalEntity);
                }
            }
        }

        public SaveSystem(DataAccessHandler dataHandler, double cooldown = 10.0)
        {
            m_dataAccessHandler = dataHandler;
            m_cooldown = new(cooldown);
        }
    }

    public class ManualSaveRequest : IMessage
    {
        string IMessage.BuildMessage()
        {
            return $"Request: Save game";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }
}
