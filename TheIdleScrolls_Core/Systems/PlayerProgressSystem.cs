using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Systems
{
    internal class PlayerProgressSystem : AbstractSystem
    {
        Entity? m_player = null;
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == null)
                m_player = coordinator.GetEntities<PlayerProgressComponent>().FirstOrDefault();
            if (m_player == null)
                return;

            var progComp = m_player.GetComponent<PlayerProgressComponent>();
            if (progComp == null)
                return;

            // Update playtime
            progComp.Data.Playtime += dt;

            // Update kills and highest area
            var kills = coordinator.FetchMessagesByType<DeathMessage>();
            progComp.Data.Kills += kills.Count;
            if (kills.Count > 0) // TODO: Check if in wilderness
            {
                var lvl = kills.First().Victim.GetComponent<LevelComponent>()?.Level ?? 0;
                progComp.Data.HighestWildernessKill = Math.Max(lvl, progComp.Data.HighestWildernessKill);
            }

            // Update losses
            progComp.Data.Losses += coordinator.FetchMessagesByType<BattleLostMessage>().Count;

            // Update items
            var newItems = coordinator.FetchMessagesByType<ItemReceivedMessage>();
            foreach (var item in newItems)
            {
                if (item.Recipient == m_player)
                {
                    // TODO: do something
                }
            }

            // Update cleared dungeons
            var dungeons = coordinator.FetchMessagesByType<DungeonClearedMessage>();
            foreach (var dungeon in dungeons)
            {
                progComp.Data.ClearedDungeons.Add(dungeon.DungeonId);
            }
        }
    }
}
