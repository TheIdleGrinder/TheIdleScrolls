using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Systems
{
    internal class PlayerProgressSystem : AbstractSystem
    {
        Entity? m_player = null;
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            m_player ??= coordinator.GetEntities<PlayerComponent, PlayerProgressComponent>().FirstOrDefault();
            if (m_player == null)
                return;

            var progComp = m_player.GetComponent<PlayerProgressComponent>();
            if (progComp == null)
                return;
            var locationComp = m_player.GetComponent<LocationComponent>();
            if (locationComp == null)
                return;

            // Update playtime
            progComp.Data.Playtime += dt;

            // Update kills and highest area
            var kills = coordinator.FetchMessagesByType<DeathMessage>();
            progComp.Data.Kills += kills.Count;
            if (kills.Count > 0 && !locationComp.InDungeon)
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
            var dungeons = coordinator.FetchMessagesByType<DungeonCompletedMessage>();
            foreach (var dungeon in dungeons)
            {
                if (!progComp.Data.DungeonTimes.ContainsKey(dungeon.DungeonId))
                {
                    progComp.Data.DungeonTimes[dungeon.DungeonId] = progComp.Data.Playtime;
                }
            }

            // Update coins
            var coinMsgs = coordinator.FetchMessagesByType<CoinsChangedMessage>();
            foreach (var coinMsg in coinMsgs)
            {
                if (coinMsg.Change > 0)
                {
                    progComp.Data.TotalCoins += coinMsg.Change;
                    int coins = m_player.GetComponent<CoinPurseComponent>()?.Coins ?? 0;
                    if (coins > progComp.Data.MaxCoins)
                        progComp.Data.MaxCoins = coins;
                }
                if (progComp.Data.MaxCoins > progComp.Data.TotalCoins) // Fixes old save games from before total coins were tracked
                    progComp.Data.TotalCoins = progComp.Data.MaxCoins;
            }

            // Update crafting
            var craftMsgs = coordinator.FetchMessagesByType<CraftingStartedMessage>();
            foreach (var craftMsg in craftMsgs.Where(m => m.Owner == m_player))
            {
                progComp.Data.CoinsSpentOnForging += craftMsg.CoinsPaid;
                
            }
            var forgeMsgs = coordinator.FetchMessagesByType<ItemReforgedMessage>();
            foreach (var forgeMsg in forgeMsgs.Where(m => m.Owner == m_player))
            {
                int rarity = forgeMsg.Item.GetComponent<ItemRarityComponent>()?.RarityLevel ?? 0;
				if (rarity > progComp.Data.BestReforge)
				{
					progComp.Data.BestReforge = rarity;
				}
				if ((forgeMsg.Item.GetItemId()?.GenusIndex ?? -1) == 0
					&& rarity > progComp.Data.BestG0Reforge)
				{
					progComp.Data.BestG0Reforge = rarity;
				}
			}
        }
    }
}
