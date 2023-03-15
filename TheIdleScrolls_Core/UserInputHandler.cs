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

        public void ReforgeItem(uint playerId, uint itemId);

        public void TravelIntoWilderness(int areaLevel);

        public void EnterDungeon(string dungeonId);

        public void LeaveDungeon();

        public void SetAutoProceed(bool autoProceed);
    }

    public class UserInputErrorMessage : IMessage
    {
        public string Message { get; set; }
        public UserInputErrorMessage(string message) { Message = message; }
        string IMessage.BuildMessage() { return $"Error: {Message}"; }
        IMessage.PriorityLevel IMessage.GetPriority() { return IMessage.PriorityLevel.VeryHigh; }
    }
}
