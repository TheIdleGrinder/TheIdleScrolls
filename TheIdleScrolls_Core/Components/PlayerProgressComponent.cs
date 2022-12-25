using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public enum TutorialStep { Start, Inventory, MobAttacks, Armor, Abilities, Travel }

    public class PlayerProgressComponent : IComponent
    {
        public ProgressData Data { get; set; } = new();
    }

    public class ProgressData
    {
        public HashSet<TutorialStep> TutorialProgress { get; set; } = new();
    }
}
