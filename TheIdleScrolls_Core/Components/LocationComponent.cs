using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Components
{
    public class LocationComponent : IComponent
    {
        public Location CurrentLocation { get; set; } = new();

        public string DungeonId { get; set; } = string.Empty;

        public int DungeonFloor { get; set; } = 0;

        /// <summary>
        /// Corner cut: Store the data pertaining to the current zone in the component. 
        /// This is only used for player characters to keep track of remaining monsters.
        /// </summary>
        public ZoneDescription CurrentZone { get; set; } = new();

        public bool InDungeon => DungeonId != string.Empty;
    }
}
