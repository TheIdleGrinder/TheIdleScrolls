using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class EquipmentAchievementPack : IContentPack
	{
		public string Id => "CP_EquipmentAchievements";
		public string Name => "Equipment";
		public string Description => "This pack contains achievements for accomplishing various tasks related to equipment";
		public List<IContentPiece> ContentPieces => [
			new AchievementContent(new(
                "DifferentQualities",
                "Happy Pride",
                "Wear items with six different quality levels above 0 at the same time",
                (e, w) => true,
                (e, w) => (e.GetComponent<EquipmentComponent>()
                            ?.GetItems()
                            ?.Select(i => i.GetBlueprint()?.Quality ?? 0)
                            ?.Distinct()
                            ?.Count(i => i > 0) ?? 0) >= 6)
                {
                    Reward = new PerkReward(new Perk("WellDressed", "Well Dressed", 
                        $"Gain {0.005:0.#%} increased damage and defense per level of quality on your gear", 
                        [UpdateTrigger.EquipmentChanged],
                        (_, e, w, c) =>
                        {
                            int total = e.GetComponent<EquipmentComponent>()?.GetItems()
                                ?.Sum(i => i.GetBlueprint()?.Quality ?? 0)
                                ?? 0;
                            return [ 
                                new($"WellDressed_dmg", ModifierType.Increase, 0.005 * total, [Tags.Damage],  []),
                                new($"WellDressed_def", ModifierType.Increase, 0.005 * total, [Tags.Defense], [])
                            ];
                        }))
                })
		];
	}
}
