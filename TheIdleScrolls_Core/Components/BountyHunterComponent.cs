using MiniECS;

namespace TheIdleScrolls_Core.Components
{
    public class BountyHunterComponent : IComponent
    {
        public int HighestCollected { get; set; } = 0;

        public int CurrentHuntCount { get; set; } = 0;

        public int CurrentHuntLevel { get; set; } = 0;
    }
}