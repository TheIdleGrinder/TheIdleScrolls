using MiniECS;

namespace TheIdleScrolls_Core.Components
{
    public class BountyHunterComponent : IComponent
    {
        public int HighestCollected { get; set; } = 0;

        public int CurrentStreak { get; set; } = 0;

        public int CurrentStreakLevel { get; set; } = 0;
    }
}