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
        public bool SingleShot { get; set; } = false;
        public bool HasFinished { get { return Remaining <= 0.0; } }

        public Cooldown(double duration, bool startReady = false)
        {
            m_duration = duration;
            m_remaining = startReady ? 0.0 : duration;
        }

        public int Update(double dt)
        {
            if (SingleShot && HasFinished)
            {
                m_remaining = 0.0;
                return 0;
            }
            m_remaining -= dt;
            if (!HasFinished)
                return 0;

            if (SingleShot)
            {
                m_remaining = 0.0;
                return 1;
            }
            else
            {
                int triggers = 0;
                while (HasFinished)
                {
                   triggers++;
                   m_remaining += m_duration;
                }
                return triggers;
            }
        }

        public void Reset()
        {
            m_remaining = m_duration;
        }

        public void Reset(double newDuration)
        {
            m_duration = newDuration;
            Reset();
        }
    }
}
