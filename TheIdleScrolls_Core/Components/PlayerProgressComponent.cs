using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public enum TutorialProgress { Start, Inventory, MobAttacks, Armor, Abilities, Travel }

    internal class PlayerProgressComponent : IComponent
    {
        public ProgressData Data { get; set; } = new();
    }

    public class ProgressData
    {
        public TutorialProgress Progress { get; set; } = TutorialProgress.Start;
    }
}
