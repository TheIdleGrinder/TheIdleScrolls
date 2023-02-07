using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core
{
    public class World
    {
        public string DungeonId = "";
        public int DungeonFloor = 0;

        public ZoneDescription Zone;
        public int RemainingEnemies = Int32.MaxValue;

        public double XpMultiplier = 1.0;
        public double SpeedMultiplier = 1.0;
        public double RarityMultiplier = 1.0;
        public double QuantityMultiplier = 1.0;

        public ItemKingdomDescription ItemKingdom = new();

        public AreaKingdomDescription AreaKingdom = new();

        public Entity GlobalEntity = new(); // Stores character-independent data, that needs to be saved regularly (e.g. achievement status)

        public Cooldown TimeLimit = new(10.0);

        public World()
        {
            Zone = new();
            TimeLimit.SingleShot = true;
        }

        public bool IsInDungeon()
        {
            return DungeonId != "";
        }

        public List<MobDescription> GetLocalMobs()
        {
            if (!IsInDungeon())
                return new();
            return AreaKingdom.GetLocalEnemies(DungeonId);
        }
    }
}
