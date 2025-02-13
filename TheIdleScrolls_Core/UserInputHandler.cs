using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;

namespace TheIdleScrolls_Core
{
    public interface IUserInputHandler
    {
        public void EquipItem(uint playerId, uint itemId);

        public void UnequipItem(uint playerId, uint itemId);

        public void SellItem(uint playerId, uint itemId);

        public void CraftItem(uint playerId, uint itemId); // Requires item to be picked from the list of available prototypes

        public void RefineItem(uint playerId, uint itemId);

        public void CancelCraft(uint playerId, uint itemId);

        public void TravelIntoWilderness(int areaLevel);

        public void TravelToLocation(int x, int y);

        public void TravelToNextLocation();

        public void TravelToPreviousLocation();

        public void EnterDungeon(string dungeonId, int level);

        public void LeaveDungeon();

        public void SetAutoProceed(bool autoProceed);

        public void SetGrindDungeon(bool grind);

        public void SendDialogueResponse(string id, string response);

        public void SetPerkLevel(uint playerId, string perkId, int level);
    }

}
