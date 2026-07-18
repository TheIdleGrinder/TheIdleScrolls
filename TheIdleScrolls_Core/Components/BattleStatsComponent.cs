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
    public class BattleStatsComponent : IComponent
    {
        public class AttackVector(DamageCluster dmg, double cd, double range)
        {
            public DamageCluster RawDamage = dmg;
            public double AttackTime = cd;
            public double Range = range;

            public double Dps => (AttackTime != 0) ? RawDamage.TotalDamage / AttackTime : 0.0;
        }

        public BattleStatsComponent(AttackVector baseAttack)
        {
            BaseAttack = baseAttack;
        }

        public AttackVector BaseAttack { get; set; }
        public List<AttackVector> AttackVectors { get; set; } = [];
        public int CurrentHand { get; set; } = 0; // 0 for main hand, 1 for off hand, etc.

        public double Armor { get; set; } = 0.0;
        public double Evasion { get; set; } = 0.0;
        public double Encumbrance { get; set; } = 0.0;
        public double MovementSpeed { get; set; } = Stats.BaseMovementSpeed;

        public bool CanAttack => AttackVectors.Count > 0;
        public AttackVector CurrentAttack => AttackVectors.Count == 0 ? BaseAttack
            : (AttackVectors[CurrentHand < AttackVectors.Count ? CurrentHand : 0]);
        public DamageCluster AverageDamage => AttackVectors.Average();
        public double AverageCooldown => (AttackVectors.Count != 0) ? AttackVectors.Average(av => av.AttackTime) : BaseAttack.AttackTime;
        public double AverageDps => (AverageCooldown != 0) ? AverageDamage.TotalDamage / AverageCooldown : 0.0;
        public double AverageRange => (AttackVectors.Count != 0) ? AttackVectors.Average(av => av.Range) : BaseAttack.Range;
        public double EncumbranceSlowdown => Functions.CalculateEncumbranceSlowdown(Encumbrance);

        public void SwitchHand()
        {
            if (AttackVectors.Count > 1)
            {
                CurrentHand = (CurrentHand + 1) % AttackVectors.Count;
            }
            else
            {
                CurrentHand = 0; // Only one attack vector, so reset to main hand
            }
        }

        public void ResetAttacks()
        {
            AttackVectors.Clear();
        }

        public void AddAttackVector(DamageCluster rawDamage, double cooldown, double range)
        {
            AttackVectors.Add(new AttackVector(rawDamage, cooldown, range));
        }
    }
}
