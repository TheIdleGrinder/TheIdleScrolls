using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public class BattlerComponent : IComponent
    {
        public Battle Battle { get; set; }
        public int AttacksPerformed { get; set; } = 0;

        public bool FirstStrike => AttacksPerformed == 0;

        public BattlerComponent(Battle battle)
        {
            Battle = battle;
        }
    }

    public class Battle
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

        public Battle(Entity player, int mobs) 
        {
            Player = player;
            MobsRemaining = mobs;
        }
        public Entity Player { get; set; }
        public Entity? Mob { get; set; } = null;
        public int MobsRemaining { get; set; } = 1;

        // Prevents time limit of final battle from being reset
        public bool CustomTimeLimit { get; set; } = false;

        public BattleState State { get; set; } = BattleState.Initialized;

        public bool IsFinished => State == BattleState.PlayerWon || State == BattleState.PlayerLost || State == BattleState.Cancelled;
        public bool NeedsMob => Mob == null && MobsRemaining > 0;
    }
}
