using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrollsApp
{
    internal class UserInputSystem : AbstractSystem, IUserInputHandler
    {

        List<IMessage> m_requests = new();

        public void EnterDungeon(string dungeonId)
        {
            m_requests.Add(new EnterDungeonRequest(dungeonId));
        }

        public void EquipItem(uint playerId, uint itemId)
        {
            m_requests.Add(new ItemMoveRequest(playerId, itemId, true));
        }

        public void SellItem(uint playerId, uint itemId)
        {
            m_requests.Add(new SellItemRequest(playerId, itemId));
        }

        public void ReforgeItem(uint playerId, uint itemId)
        {
            m_requests.Add(new ReforgeItemRequest(playerId, itemId));
        }

        public void LeaveDungeon()
        {
            m_requests.Add(new LeaveDungeonRequest());
        }

        public void SetAutoProceed(bool autoProceed)
        {
            m_requests.Add(new AutoProceedRequest(autoProceed));
        }

        public void TravelIntoWilderness(int areaLevel)
        {
            m_requests.Add(new TravelRequest("", areaLevel));
        }

        public void UnequipItem(uint playerId, uint itemId)
        {
            m_requests.Add(new ItemMoveRequest(playerId, itemId, false));
        }

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            List<IMessage> processed = new();
            foreach (var request in m_requests)
            {
                coordinator.PostMessage(this, request as dynamic);
                processed.Add(request);
            }
            processed.ForEach(m => m_requests.Remove(m)); // Don't use Clear to prevent (unlikely) timing issues
        }

    }
}
