using MiniECS;
using TheIdleScrolls_Core.Crafting;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Core
{
    public interface IApplicationModel
    {
        public HashSet<IMessage.PriorityLevel> GetRelevantMessagePriorties();
    }

    public enum GameFeature { Inventory, Armor, Abilities, Travel, Crafting, Perks, Bounties, DungeonGrinding }

    public record CharacterRepresentation(uint Id, string Name, string Class, int Level);
    public record AbilityRepresentation(string Key, string Name, int Level, int XP, int TargetXP);
    public record PerkRepresentation(string Id, string Name, string Description, List<string> Modifiers, int Level);
    public record MobRepresentation(uint EntityId, string Id, string Name, int Level, int HP, int HpMax, double Damage);
    public record DungeonRepresentation(string Id, string Name, int[] Levels, string Description, int Rarity);
    public record AchievementRepresentation(string Title, string Description, bool Earned, string Reward = "");
    public record AreaRepresentation(string Name, int Level, bool IsDungeon);
    public record CraftingBenchRepresentation(int MaxCraftingLevel, int Slots, int MaxActive, List<CraftingProcessRepresentation> Crafts);
    public record CraftingProcessRepresentation(CraftingType Type, IItemEntity Item, double Duration, double Remaining, int CoinsSpent);
    public record BountyStateRepresentation(int HighestEarned, 
                                            int CurrentHuntLevel,
                                            int CurrentHuntAnchor,
                                            int CurrentHuntCount, 
                                            int CurrentHuntLength, 
                                            int ExpectedReward);
}
