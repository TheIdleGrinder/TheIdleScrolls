using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public class TimeShieldComponent : IComponent
    {
        public double Maximum { get; set; }
        public double Remaining { get; set; }

        public bool IsDepleted => Remaining <= 0;
        public bool IsFull => Remaining >= Maximum;

        public TimeShieldComponent(double max)
        {
            Maximum = max;
            Refill();
        }

        public void Refill()
        {
            Remaining = Maximum;
        }

        public void Drain(double amount)
        {
            Remaining -= amount;
            if (Remaining < 0)
            {
                Remaining = 0;
            }
        }

        // Update maximum and rescale remaining time
        public void Rescale(double newMaximum)
        {
            double previousMax = Maximum;
            Maximum = newMaximum;
            Remaining = (previousMax > 0) ? Remaining * Maximum / previousMax : Maximum;
        }
    }
}
