using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public class ChanceChargeComponent : IComponent
    {
        Dictionary<string, double> Charges { get; } = [];

        public int AddCharge(string chargeId, double amount)
        {
            if (Charges.ContainsKey(chargeId))
            {
                Charges[chargeId] += amount;
            }
            else
            {
                Charges[chargeId] = 0.5 + amount; // Start at 0.5 to make things feel less "unlucky"
            }
            return GetFullChargeCount(chargeId);
        }

        public double GetCharge(string chargeId)
        {
            return Charges.TryGetValue(chargeId, out double amount) ? amount : 0.0;
        }

        public int GetFullChargeCount(string chargeId)
        {
            return (int)GetCharge(chargeId);
        }

        public bool RemoveCharge(string chargeId, double amount)
        {
            if (Charges.ContainsKey(chargeId) && Charges[chargeId] >= amount)
            {
                Charges[chargeId] -= amount;
                return true;
            }
            return false;
        }
    }
}
