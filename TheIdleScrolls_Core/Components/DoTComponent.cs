using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Components
{
    public class DoTComponent : IComponent
    {
        readonly Dictionary<DamageType, List<DoT>> DoTs = [];

        public int DamageTypeCount => DoTs.Count;
        public double TotalDps => DoTs.Values.Select(d => d.Sum(dot => dot.Dps)).Sum();

        public void Add(DoT dot)
        {
            if (!DoTs.TryGetValue(dot.DamageType, out List<DoT>? value))
            {
                DoTs[dot.DamageType] = [dot];
            }
            else
            {
                value.Add(dot);
            }
        }

        public double Update(double dt)
        {
            double totalDamage = 0.0;
            foreach (DamageType type in DoTs.Keys)
            {
                bool cleanList = false;
                foreach (DoT dot in DoTs[type])
                {
                    totalDamage += dot.Update(dt);
                    cleanList |= dot.IsExpired;
                }
                if (cleanList)
                {
                    DoTs[type] = DoTs[type].Where(dot => !dot.IsExpired).ToList();
                    if (DoTs[type].Count == 0)
                    {
                        DoTs.Remove(type);
                    }
                }
            }
            return totalDamage;
        }

        public List<DamageType> ActiveTypes => [.. DoTs.Keys];

        public List<DoT> EffectsOfType(DamageType type)
        {
            return DoTs.GetValueOrDefault(type) ?? [];
        }

        public int EffectCountForType(DamageType type)
        {
            return DoTs.GetValueOrDefault(type)?.Count ?? 0;
        }

        public double DpsForType(DamageType type)
        {
            return DoTs.GetValueOrDefault(type)?.Sum(x => x.Dps) ?? 0.0;
        }
    }

    public class DoT(DamageType damageType, double dps, double duration)
    {
        public DamageType DamageType { get; set; } = damageType;
        public double Dps { get; set; } = dps;
        public Cooldown Timer { get; init; } = new(duration) { SingleShot = true };

        public bool IsExpired => Timer.HasFinished;
        public double RemainingDamage => Dps * Timer.Remaining;
        
        /// <summary>
        /// Updates the timer by dt seconds and calculates the amount of damage done
        /// (might be less than expected if remaining time was less than dt)
        /// </summary>
        /// <param name="dt">Elapsed time</param>
        /// <returns>Damage done during dt</returns>
        public double Update(double dt)
        {
            if (Timer.HasFinished) 
            {
                return 0.0;
            }
            double realDt = Math.Min(dt, Timer.Remaining);
            Timer.Update(realDt);
            return realDt * Dps;
        }
    }
}
