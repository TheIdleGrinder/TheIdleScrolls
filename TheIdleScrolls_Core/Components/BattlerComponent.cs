using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public class BattlerComponent(Battle battle) : IComponent
    {
        public Battle Battle { get; set; } = battle;
        public BattlePosition Position { get; set; } = new(0.0, 0.0);
        public int SkillsUsed { get; set; } = 0;
        public double DamageDealt { get; set; } = 0.0;

        public bool FirstStrike => DamageDealt == 0.0;
    }

    public class Battle(Entity player, int mobs)
    {
        public enum BattleState
        {
            Initialized,
            BetweenFights,
            InProgress,
            Cancelled,
            PlayerWon,
            PlayerLost
        }

        public double Duration { get; set; } = 0.0;
        public Entity Player { get; set; } = player;
        public List<Entity> Mobs { get; set; } = [];
        public int MobsRemaining { get; set; } = mobs;

        // Prevents time limit of final battle from being reset
        public bool CustomTimeLimit { get; set; } = false;

        public BattleState State { get; set; } = BattleState.Initialized;

        public bool IsFinished => State == BattleState.PlayerWon || State == BattleState.PlayerLost || State == BattleState.Cancelled;
        public bool CanAddMob => (State == BattleState.Initialized || State == BattleState.BetweenFights) 
                                    && MobsRemaining > 0;
    }

    public class BattlePosition(double x, double y)
    {
        public double X { get; set; } = x;
        public double Y { get; set; } = y;
        public double DistanceTo(BattlePosition other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public record BattleData(double Duration, double EDPS, double TimeToKill, double TimeToDie, double DamagePotential)
    {
        public static BattleData FromBattle(Battle battle)
        {
            double dmgDealt = battle.Player.GetComponent<BattlerComponent>()!.DamageDealt;
            int mobHp = battle.Mobs.Sum(mob => mob.GetComponent<LifePoolComponent>()?.Maximum ?? 0);
            LifePoolComponent hpComp = battle.Player.GetComponent<LifePoolComponent>()!;
            double hpPctLost = hpComp.Maximum > 0.0 ? 1.0 - (1.0 * hpComp.Current / hpComp.Maximum) : 0.0;

            double edps = battle.Duration > 0.0 ? dmgDealt / battle.Duration : 0.0;
            double timeToKill = edps > 0.0 ? mobHp / edps : 0.0;
            double timeToDie = hpPctLost > 0.0 ? battle.Duration / hpPctLost : 0.0;
            double dmgPotential = timeToDie > 0.0 ? edps * timeToDie : 0.0;

            return new BattleData(battle.Duration, edps, timeToKill, timeToDie, dmgPotential);
        }
    }
}