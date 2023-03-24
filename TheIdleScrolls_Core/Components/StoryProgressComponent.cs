using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public class StoryProgressComponent : IComponent
    {
        public FinalFight FinalFight = new();
    }

    public struct FinalFight
    {
        public enum Status { NotStarted, Slowing, Pause, Done, Inactive }
        public Status State = FinalFight.Status.NotStarted;
        public DateTime StartTime = DateTime.MinValue;

        public FinalFight() {}
    }
}
