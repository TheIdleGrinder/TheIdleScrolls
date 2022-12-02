using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Utility
{
    public class Cooldown
    {
        double m_duration;
        double m_remaining;

        public double Duration { get { return m_duration; } }
        public double Remaining { get { return m_remaining; } }

        public Cooldown(double duration, bool startReady = false)
        {
            m_duration = duration;
            m_remaining = startReady ? 0.0 : duration;
        }

        public int Update(double dt)
        {
            int triggers = 0;
            m_remaining -= dt;
            while (m_remaining <= 0.0)
            {
                triggers++;
                m_remaining += m_duration;
            }
            return triggers;
        }
    }
}
