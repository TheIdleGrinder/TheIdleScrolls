using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.GameWorld
{
    public enum AreaChangeType
    {
        Travel,
        EnteredDungeon,
        LeftDungeon,
        FloorChange
    }

    public class AreaChangedMessage : IMessage
    {
        public Entity Entity { get; set; }
        public Location Location { get; set; }
        public string DungeonId { get; set; }
        public int DungeonFloor { get; set; }
        public AreaChangeType AreaChangeType { get; set; }

        public AreaChangedMessage(Entity entity, Location location, string dungeonId, int dungeonFloor, AreaChangeType areaChangeType)
        {
            Entity = entity;
            Location = location;
            DungeonId = dungeonId;
            DungeonFloor = dungeonFloor;
            AreaChangeType = areaChangeType;
        }

        public AreaChangedMessage(Entity entity, Location location) : this(entity, location, string.Empty, -1, AreaChangeType.Travel)
        { }

        string IMessage.BuildMessage()
        {
            return Entity.GetName() + " " + AreaChangeType switch
            {
                AreaChangeType.Travel => $"travelled to coordinates {Location}",
                AreaChangeType.EnteredDungeon => $"entered {DungeonList.GetDungeon(DungeonId)?.Name ?? "??"}",
                AreaChangeType.LeftDungeon => $"left {DungeonList.GetDungeon(DungeonId)?.Name ?? "??"}",
                AreaChangeType.FloorChange => $"moved to floor {DungeonFloor + 1} of {DungeonList.GetDungeon(DungeonId)?.Name ?? "??"}",
                _ => $"moved between areas"
            };
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.VeryLow;
        }
    }
}
