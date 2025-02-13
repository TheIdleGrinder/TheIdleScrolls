using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Messages;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrollsApp
{
    internal class UserInputSystem : AbstractSystem, IUserInputHandler
    {

        List<IMessage> m_requests = new();

        public void EnterDungeon(string dungeonId, int level)
        {
            m_requests.Add(new EnterDungeonRequest(dungeonId, level));
        }

        public void EquipItem(uint playerId, uint itemId)
        {
            m_requests.Add(new ItemMoveRequest(playerId, itemId, true));
        }

        public void UnequipItem(uint playerId, uint itemId)
        {
            m_requests.Add(new ItemMoveRequest(playerId, itemId, false));
        }

        public void SellItem(uint playerId, uint itemId)
        {
            m_requests.Add(new SellItemRequest(playerId, itemId));
        }

        public void CraftItem(uint playerId, uint itemId)
        {
            m_requests.Add(new CraftItemRequest(playerId, itemId));
        }

        public void RefineItem(uint playerId, uint itemId)
        {
            m_requests.Add(new RefineItemRequest(playerId, itemId));
        }

        public void CancelCraft(uint playerId, uint itemId)
        {
            m_requests.Add(new CancelCraftRequest(playerId, itemId));
        }

        public void LeaveDungeon()
        {
            m_requests.Add(new LeaveDungeonRequest());
        }

        public void SetAutoProceed(bool autoProceed)
        {
            m_requests.Add(new AutoProceedRequest(autoProceed));
        }

        public void SetGrindDungeon(bool grind)
        {
            m_requests.Add(new AutoGrindDungeonsRequest(grind));
        }

        public void TravelIntoWilderness(int areaLevel)
        {
            m_requests.Add(new TravelRequest(areaLevel, 0)); // CornerCut: for now this works, this function will be removed later
        }

        public void TravelToLocation(int x, int y)
        {
            m_requests.Add(new TravelRequest(x, y));
        }

        public void TravelToNextLocation()
        {
            m_requests.Add(new SingleStepTravelRequest(true));
        }

        public void TravelToPreviousLocation()
        {
            m_requests.Add(new SingleStepTravelRequest(false));
        }

        public void SendDialogueResponse(string id, string response)
        {
            m_requests.Add(new DialogueResponseMessage(id, response));
        }

        public void SetPerkLevel(uint playerId, string perkId, int level)
        {
            m_requests.Add(new SetPerkLevelRequest(playerId, perkId, level));
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
