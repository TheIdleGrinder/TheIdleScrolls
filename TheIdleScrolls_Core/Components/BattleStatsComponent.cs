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
        }

        public BattleStatsComponent(AttackVector baseAttack)
        {
            BaseAttack = baseAttack;
        }

        public AttackVector BaseAttack { get; set; }
        public List<AttackVector> AttackVectors { get; set; } = [];
        public double Armor { get; set; } = 0.0;
        public double Evasion { get; set; } = 0.0;
        public double Encumbrance { get; set; } = 0.0;
        public double MovementSpeed { get; set; } = Stats.BaseMovementSpeed;

        public bool CanAttack => AttackVectors.Count > 0;
        public DamageCluster AverageDamage => AttackVectors.Average();
        public double AverageCooldown => AttackVectors.Average(av => av.AttackTime);
        public double AverageDps => (AverageCooldown != 0) ? AverageDamage.TotalDamage / AverageCooldown : 0.0;
        public double AverageRange => AttackVectors.Average(av => av.Range);
        public double EncumbranceSlowdown => Functions.CalculateEncumbranceSlowdown(Encumbrance);

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
