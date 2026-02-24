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
using TheIdleScrolls_Core.Perks;

namespace TheIdleScrolls_Core.CharacterPaths.Paths
{
    public static class FighterPath
    {
        const string PathId         = "fighter";
        const string RootId         = "fighter_root";
        const string DualWield1Id   = "dualwield1";
        const string DualWield2Id   = "dualwield2";
        const string DualWield3Id   = "dualwield3";
        const string DualWield4Id   = "dualwield4";
        const string Shield1Id      = "shielded1";
        const string Single1Id      = "singlehanded1";
        const string TwoHand1Id     = "twohanded1";

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
            List<ItemReward> items = ItemFamilies.Weapons
                .Concat(ItemFamilies.Armors)
                .Select(w => new ItemBlueprint(w, 0, MaterialId.Simple))
                .Where(b => b.IsValid())
                .Select(b => new ItemReward(b))
                .ToList();
            List<IAchievementReward> rewards = [];
            foreach (var item in items)
            {
                rewards.Add(item);
            }
            return new MultiReward(rewards, "Fighter training items");
        }

        static FighterPath()
        {
            Path.AddStep(new CharacterPathStep(RootId, Properties.Skills.PathFighterRoot, "")
            {
                Reward = new MultiReward([new PerkReward(RootPerk), new PerkReward(HeavyAttackPerks.BasePerk), StarterItems()]),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements("", 0)
            });
            Path.AddStep(new CharacterPathStep(DualWield1Id, Properties.Skills.DualWield1, "")
            {
                Reward = new AbilityReward(Abilities.DualWield),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements("", 1)
            });
            Path.AddStep(new CharacterPathStep(Shield1Id, Properties.Skills.Shield1, "")
            {
                Reward = new AbilityReward(Abilities.Shielded),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements("", 1)
            });
            Path.AddStep(new CharacterPathStep(Single1Id, Properties.Skills.SingleHanded1, "")
            {
                Reward = new AbilityReward(Abilities.SingleHanded),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements("", 1)
            });
            Path.AddStep(new CharacterPathStep(TwoHand1Id, Properties.Skills.TwoHanded1, "")
            {
                Reward = new AbilityReward(Abilities.TwoHanded),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements("", 1)
            });
            Path.AddStep(new CharacterPathStep(EnvenomWeapon.BasePerkId, EnvenomWeapon.BasePerk.Name, EnvenomWeapon.BasePerk.Description)
            {
                Reward = new PerkReward(EnvenomWeapon.BasePerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements("", 2)
            }); 
            Path.AddStep(new CharacterPathStep(EnvenomWeapon.DurationPerkId, EnvenomWeapon.SupportPerk.Name, EnvenomWeapon.SupportPerk.Description)
            {
                Reward = new PerkReward(EnvenomWeapon.SupportPerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(EnvenomWeapon.BasePerkId, 3)
            }); 
            Path.AddStep(new CharacterPathStep(BlazingWeapon.BasePerkId, BlazingWeapon.BasePerk.Name, BlazingWeapon.BasePerk.Description)
            {
                Reward = new PerkReward(BlazingWeapon.BasePerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements("", 2)
            });
            Path.AddStep(new CharacterPathStep(BlazingWeapon.SupportPerkId, BlazingWeapon.SupportPerk.Name, BlazingWeapon.SupportPerk.Description)
            {
                Reward = new PerkReward(BlazingWeapon.SupportPerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(BlazingWeapon.BasePerkId, 3)
            });
            Path.AddStep(new CharacterPathStep(BattleCry.BasePerkId, BattleCry.BasePerk.Name, BattleCry.BasePerk.Description)
            {
                Reward = new PerkReward(BattleCry.BasePerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements("", 3)
            });
            Path.AddStep(new CharacterPathStep(BattleCry.BuffPerkId, BattleCry.BuffPerk.Name, BattleCry.BuffPerk.Description)
            {
                Reward = new PerkReward(BattleCry.BuffPerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(BattleCry.BasePerkId, 4)
            });
            Path.AddStep(new CharacterPathStep(BattleCry.DebuffPerkId, BattleCry.DebuffPerk.Name, BattleCry.DebuffPerk.Description)
            {
                Reward = new PerkReward(BattleCry.DebuffPerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(BattleCry.BasePerkId, 5)
            });
        }
    }
}
