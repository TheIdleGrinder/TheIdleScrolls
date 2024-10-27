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

        public BattlerComponent(Battle battle)
        {
            Battle = battle;
        }
    }

    public class Battle
    {
        public enum BattleState
        {
            WaitingForMob,
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

        public BattleState State { get; set; } = BattleState.WaitingForMob;

        public bool IsFinished => State == BattleState.PlayerWon || State == BattleState.PlayerLost || State == BattleState.Cancelled;
        public bool NeedsMob => State == BattleState.WaitingForMob && MobsRemaining > 0;
    }
}
