using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public enum AdventurerState
    {
        Idle,
        Resting,
        Fighting
    }

    public class AdventurerComponent : IComponent
    {
        public AdventurerState State { get; set; } = AdventurerState.Idle;

        public void SetState(AdventurerState newState)
        {
            State = newState;
        }
    }
}
