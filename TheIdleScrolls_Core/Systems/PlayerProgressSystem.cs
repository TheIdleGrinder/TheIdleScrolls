using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
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
            foreach (var kill in coordinator.FetchMessagesByType<DeathMessage>())
            {
                progComp.Data.Kills++;
                var mobId = kill.Victim.GetComponent<MobComponent>()?.Id;
                if (mobId != null)
                {
                    if (!progComp.Data.DefeatedMobs.ContainsKey(mobId))
                        progComp.Data.DefeatedMobs[mobId] = 0;
                    progComp.Data.DefeatedMobs[mobId]++;
                }
                if (!locationComp.InDungeon)
                {
                    progComp.Data.HighestWildernessKill = Math.Max(progComp.Data.HighestWildernessKill, kill.Victim.GetLevel());
                }
                List<string> tagsOfInterest = [Tags.DualWield, Tags.Shielded, Tags.SingleHanded, Tags.TwoHanded, 
                    Tags.Unarmed, Tags.Unarmored, 
                    Abilities.Axe, Abilities.Blunt, Abilities.LongBlade, Abilities.Polearm, Abilities.ShortBlade];
                var playerTags = m_player.GetTags().ToHashSet();
                foreach (var tag in tagsOfInterest)
                {
                    if (playerTags.Contains(tag))
                    {
                        if (!progComp.Data.ConditionalKills.ContainsKey(tag))
                            progComp.Data.ConditionalKills[tag] = 0;
                        progComp.Data.ConditionalKills[tag]++;
                    }
                }
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
                if (!progComp.Data.DungeonTimes.TryGetValue(dungeon.DungeonId, out Dictionary<int, double>? levelTimes) ||
                    !levelTimes.ContainsKey(dungeon.DungeonLevel))
                {
                    if (!progComp.Data.DungeonTimes.ContainsKey(dungeon.DungeonId))
                        progComp.Data.DungeonTimes[dungeon.DungeonId] = [];
                    progComp.Data.DungeonTimes[dungeon.DungeonId][dungeon.DungeonLevel] = progComp.Data.Playtime;
                }
                if (!progComp.Data.DungeonCompletions.TryGetValue(dungeon.DungeonId, out Dictionary<int, int>? levelCounts) ||
                    !levelCounts.ContainsKey(dungeon.DungeonLevel))
                {
                    if (!progComp.Data.DungeonCompletions.ContainsKey(dungeon.DungeonId))
                        progComp.Data.DungeonCompletions[dungeon.DungeonId] = [];
                    progComp.Data.DungeonCompletions[dungeon.DungeonId][dungeon.DungeonLevel] = 1;
                }
                else
                {
                    levelCounts[dungeon.DungeonLevel]++;
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
                progComp.Data.CoinsSpentOnCrafting += craftMsg.CoinsPaid;                
            }
            var forgeMsgs = coordinator.FetchMessagesByType<CraftingProcessFinished>();
            foreach (var forgeMsg in forgeMsgs.Where(m => m.Owner == m_player))
            {
                int rarity = forgeMsg.Craft.TargetItem.GetComponent<ItemRarityComponent>()?.RarityLevel ?? 0;
				if (rarity > progComp.Data.BestRefine)
				{
					progComp.Data.BestRefine = rarity;
				}
				if ((forgeMsg.Craft.TargetItem.GetBlueprint()?.MaterialId ?? MaterialId.Wood3) == MaterialId.Simple
					&& rarity > progComp.Data.BestG0Refine)
				{
					progComp.Data.BestG0Refine = rarity;
				}
			}
        }
    }
}
