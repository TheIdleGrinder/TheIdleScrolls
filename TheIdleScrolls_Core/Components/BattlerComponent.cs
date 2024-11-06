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

        public Entity Player { get; set; } = player;
        public Entity? Mob { get; set; } = null;
        public int MobsRemaining { get; set; } = mobs;

        // Prevents time limit of final battle from being reset
        public bool CustomTimeLimit { get; set; } = false;

        public BattleState State { get; set; } = BattleState.Initialized;

        public bool IsFinished => State == BattleState.PlayerWon || State == BattleState.PlayerLost || State == BattleState.Cancelled;
        public bool NeedsMob => (State == BattleState.Initialized || State == BattleState.BetweenFights) && MobsRemaining > 0;
    }
}
