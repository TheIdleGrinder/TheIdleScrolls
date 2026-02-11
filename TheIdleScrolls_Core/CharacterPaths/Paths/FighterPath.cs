using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Achievements.Rewards;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Modifiers;

namespace TheIdleScrolls_Core.CharacterPaths.Paths
{
    public static class FighterPath
    {
        const string PathId = "fighter";
        const string RootId = "fighter_root";

        public static CharacterPath Path { get; } = new CharacterPath(PathId, Properties.Skills.PathFighter, Properties.Skills.PathFighter);
    
        readonly static Perk RootPerk = new(RootId, Properties.Skills.PathFighterRoot, "", [], 
            (l, e, w, c) => [
                new($"{RootId}_dmg", ModifierType.Increase, 0.25, [Tags.Damage, Tags.Melee], []),
                new($"{RootId}_arm", ModifierType.Increase, 0.25, [Tags.ArmorRating], [])
            ])
        {
            Permanent = true
        };

        static MultiReward StarterItems()
        {
            List<IAchievementReward> items = ItemFamilies.Weapons
                .Concat(ItemFamilies.Armors)
                .Select(w => new ItemBlueprint(w, 0, MaterialId.Simple))
                .Where(b => b.IsValid())
                .Select(b => new ItemReward(b) as IAchievementReward)
                .ToList();
            return new MultiReward(items, "Fighter training items");
        }

        static FighterPath()
        {
            Path.AddStep(new CharacterPathStep(RootId, Properties.Skills.PathFighterRoot, "")
            {
                Reward = new MultiReward([new PerkReward(RootPerk), StarterItems()]),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements("", 0)
            });
        }
    }
}
