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
        public int AttacksPerformed { get; set; } = 0;
        public double DamageDealt { get; set; } = 0.0;

        public bool FirstStrike => AttacksPerformed == 0;
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
        public Entity? Mob { get; set; } = null;
        public int MobsRemaining { get; set; } = mobs;

        // Prevents time limit of final battle from being reset
        public bool CustomTimeLimit { get; set; } = false;

        public BattleState State { get; set; } = BattleState.Initialized;

        public bool IsFinished => State == BattleState.PlayerWon || State == BattleState.PlayerLost || State == BattleState.Cancelled;
        public bool NeedsMob => (State == BattleState.Initialized || State == BattleState.BetweenFights) && MobsRemaining > 0;
    }

    public record BattleData(double Duration, double EDPS, double TimeToKill, double TimeToDie, double DamagePotential)
    {
        public static BattleData FromBattle(Battle battle)
        {
            double dmgDealt = battle.Player.GetComponent<BattlerComponent>()!.DamageDealt;
            int mobHp = battle.Mob?.GetComponent<LifePoolComponent>()?.Maximum ?? 0;
            TimeShieldComponent timeLimit = battle.Player.GetComponent<TimeShieldComponent>()!;
            double timePctLost = timeLimit.Maximum > 0.0 ? 1.0 - (timeLimit.Remaining / timeLimit.Maximum) : 0.0;

            double edps = battle.Duration > 0.0 ? dmgDealt / battle.Duration : 0.0;
            double timeToKill = edps > 0.0 ? mobHp / edps : 0.0;
            double timeToDie = timePctLost > 0.0 ? battle.Duration / timePctLost : 0.0;
            double dmgPotential = timeToDie > 0.0 ? edps * timeToDie : 0.0;

            return new BattleData(battle.Duration, edps, timeToKill, timeToDie, dmgPotential);
        }
    }
}