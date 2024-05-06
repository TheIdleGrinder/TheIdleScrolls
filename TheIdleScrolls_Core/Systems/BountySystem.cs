using System.Reflection.Metadata;
using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Systems
{
    /// <summary>
    /// Bounties are supposed to provide a source of steady income that does not rely on grinding dungeons.
    /// The guaranteed item drops at the end of dungeons tend to make grinding low level dungeons more profitable than hunting in the wilderness.
    /// This system is supposed to balance that and give an incentive to grind higher areas to earn XP without feeling like one is 
    /// missing out on too.much income.
    /// </summary>
    public class BountySystem : AbstractSystem
    {
        public static readonly int FirstBountyLevel = 25;
        public static readonly int EnemiesPerHunt = 20;
        public static readonly double MalusPerLevel = 0.05;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            // Bounties require a progress component for tracking highest kills and location to check if the player is in the wilderness
            foreach (var hunter in coordinator.GetEntities<BountyHunterComponent, PlayerProgressComponent, LocationComponent>())
            {
                var hunterComp = hunter.GetComponent<BountyHunterComponent>()!;
                var progressComp = hunter.GetComponent<PlayerProgressComponent>()!;
                var locationComp = hunter.GetComponent<LocationComponent>()!;

                // Setup fresh component
                if (hunterComp.HighestCollected == 0)
                {
                    hunterComp.HighestCollected = progressComp.Data.HighestWildernessKill; 
                }
                
                if (locationComp.InDungeon)
                    continue;
                
                foreach (var victim in coordinator.GetEntities<KilledComponent>().Where(v => v.GetComponent<KilledComponent>()?.Killer == hunter.Id))
                {
                    int level = victim.GetLevel();
                    if (level < FirstBountyLevel)
                        continue;

                    // Award bounty for each new level if level is above previously collected
                    // In practice this should only ever be 1 level at a time
                    while (hunterComp.HighestCollected < level)
                    {
                        AwardBounty(hunter, ++hunterComp.HighestCollected, BountyType.NewLevel, coordinator);
                    }

                    // Award bounty for every Xth kill in the wilderness
                    hunterComp.CurrentHuntCount++;
                    hunterComp.CurrentHuntLevel = (hunterComp.CurrentHuntCount == 1) 
                                                    ? level 
                                                    : Math.Min(hunterComp.CurrentHuntLevel, level);
                    if (hunterComp.CurrentHuntCount >= EnemiesPerHunt)
                    {
                        int value = CalculateBountyReward(hunterComp.CurrentHuntLevel, hunterComp.HighestCollected);
                        AwardBounty(hunter, hunterComp.CurrentHuntLevel, BountyType.Hunt, coordinator);
                        hunterComp.CurrentHuntCount = 0;
                        hunterComp.CurrentHuntLevel = 0;
                    }
                }
            }
        }

        private void AwardBounty(Entity hunter, int amount, BountyType type, Coordinator coordinator)
        {
            hunter.GetComponent<CoinPurseComponent>()?.AddCoins(amount);
            coordinator.PostMessage(this, new CoinsChangedMessage(hunter, amount));
            coordinator.PostMessage(this, new BountyMessage(hunter, type, amount));
        }

        public static int CalculateBountyReward(int level, int highestLevel)
        {
            double levelMalus = Math.Pow(1.0 - MalusPerLevel, Math.Max(0, highestLevel - level));
            return (int)Math.Ceiling(levelMalus * level);
        }
    }

    public enum BountyType { NewLevel, Hunt }

    public class BountyMessage : IMessage
    {
        public Entity Collector { get; set; }
        public BountyType Type { get; set; }

        public int Value { get; set; }

        public BountyMessage(Entity collector, BountyType type, int value)
        {
            Collector = collector;
            Type = type;
            Value = value;
        }

        string IMessage.BuildMessage()
        {
            string typeString = Type switch
            {
                BountyType.NewLevel => "unlocking a new area",
                BountyType.Hunt => "a successful hunt",
                _ => "an unknown event"
            };
            return $"{Collector.GetName()} collected a bounty of {Value}c for {typeString}";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.High;
        }
    }
}