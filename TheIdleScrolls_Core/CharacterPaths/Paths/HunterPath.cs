using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Achievements.Rewards;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Perks;

namespace TheIdleScrolls_Core.CharacterPaths.Paths
{
    public static class HunterPath
    {
        const string PathId = "ranger";
        const string RootId = PathId + "_root";
        const string Life1Id = PathId + "_hp1";

        const string MomentumPerkId = PathId + "_momentum";

        public static CharacterPath Path { get; } = new CharacterPath(PathId, 
                                                                      Properties.Skills.PathHunter, 
                                                                      Properties.Skills.PathHunter_Description);

        readonly static Perk RootPerk = new(RootId, Properties.Skills.PathHunterRoot,
            "Grants increased melee damage and armor for each step taken on this path", [],
            (l, e, w, c) =>
            {
                int steps = e.GetComponent<CharacterPathComponent>()!.StepsTakenOnPath(PathId).Count;
                return [
                    new($"{RootId}_dmg", ModifierType.Increase, 0.1 * steps, [Tags.Damage], []),
                    new($"{RootId}_eva", ModifierType.Increase, 0.1 * steps, [Tags.EvasionRating], [])
                ];
            }
        )
        {
            Permanent = true,
            Categories = [Properties.Skills.PathHunter]
        };

        readonly static Perk LifeRegPerLevelPerk = new(Life1Id, Properties.Skills.FighterHP1,
            "Regenerate one hit point per character level", [],
            (l, e, w, c) => [
                new($"{Life1Id}", ModifierType.AddBase, e.GetLevel(), [Tags.LifeRegeneration], [])
            ])
        { Permanent = true };

        readonly static Perk MomentumPerk = new(MomentumPerkId, Properties.Skills.FighterMomentum_Name, Properties.Skills.FighterMomentum_Description,
            [],
            (l, e, w, c) => [
                new($"{MomentumPerkId}_limit", ModifierType.AddBase, l + 1, [Tags.MomentumLimit], [])
            ])
        {
            MaxLevel = 9,
            Categories = [Properties.Skills.FighterMomentum_Name],
            ConditionFunc = PerkExtensions.FlatPerkLevelCondition(PerkIds.Strength, 5)
        };

        static MultiReward StarterItems()
        {
            List<string> families = [ItemFamilies.Dagger, ItemFamilies.ShortSword,
                                     ItemFamilies.Spear, ItemFamilies.Polearm,
                                     ItemFamilies.ShortBow, ItemFamilies.LongBow,
                                     ItemFamilies.LightChest];
            List<ItemReward> items = families
                .Select(w => new ItemBlueprint(w, 0, MaterialId.Simple))
                .Where(b => b.IsValid())
                .Select(b => new ItemReward(b))
                .ToList();
            List<IAchievementReward> rewards = [];
            foreach (var item in items)
            {
                rewards.Add(item);
            }
            return new MultiReward(rewards, "Hunter training items");
        }
        static HunterPath()
        {
            Path.AddStep(new CharacterPathStep(RootId, Properties.Skills.PathFighterRoot, "")
            {
                Reward = new MultiReward([new PerkReward(RootPerk),
                    new PerkReward(ExposeWeaknessPerks.BasePerk
                        .WithCategories(Properties.Skills.PathHunter, "Expose Weakness")),
                    StarterItems()]),
                StepNumber = 0
            });

            Path.AddSimplePerkStep(LifeRegPerLevelPerk, 1);

            Path.AddSimplePerkStep(MomentumPerk, 1);
        }
    }
}
