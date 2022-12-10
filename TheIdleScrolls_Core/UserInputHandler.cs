using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;

namespace TheIdleScrollsApp
{
    public interface IUserInputHandler
    {
        public void EquipItem(uint playerId, uint itemId);

        public void UnequipItem(uint playerId, uint itemId);

        public void TravelToArea(int areaLevel);

        public void SetAutoProceed(bool autoProceed);
    }
}
