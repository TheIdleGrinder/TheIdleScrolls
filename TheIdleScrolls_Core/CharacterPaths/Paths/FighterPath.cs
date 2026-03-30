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
        const string PathId             = "fighter";
        const string RootId             = "fighter_root";
        const string DualWield1Id       = "dualwield1";
        const string DualWield2Id       = "dualwield2";
        const string DualWield3Id       = "dualwield3";
        const string DualWield4Id       = "dualwield4";
        const string Shield1Id          = "shielded1";
        const string Shield2Id          = "shielded2";
        const string Shield3Id          = "shielded3";
        const string Single1Id          = "singlehanded1";
        const string Single2Id          = "singlehanded2";
        const string Single3Id          = "singlehanded3";
        const string TwoHand1Id         = "twohanded1";
        const string TwoHand2Id         = "twohanded2";
        const string TwoHand3Id         = "twohanded3";

        const string OneHandDmgPerkId   = "fighter_onehanddmg";
        const string TwoHandDmgPerkId   = "fighter_twohanddmg";
        const string AtkSpeedPerkId     = "fighter_atkspeed";
        const string ShieldDefensePerkId= "fighter_shielddefense";

        public static CharacterPath Path { get; } = new CharacterPath(PathId, Properties.Skills.PathFighter, Properties.Skills.PathFighter);
    
        readonly static Perk RootPerk = new(RootId, Properties.Skills.PathFighterRoot, "", [], 
            (l, e, w, c) => [
                new($"{RootId}_dmg", ModifierType.Increase, 0.25, [Tags.Damage, Tags.Melee], []),
                new($"{RootId}_arm", ModifierType.Increase, 0.25, [Tags.ArmorRating], [])
            ])
        {
            Permanent = true
        };

        readonly static Perk OneHandDamagePerk = new(OneHandDmgPerkId, Properties.Skills.OneHandMeleeDamage, "", [],
            (l, e, w, c) => [
                new($"{OneHandDmgPerkId}_more", ModifierType.More, 0.1, 
                    [Tags.Damage, Tags.Melee, Tags.OneHandedWeapon], []),
                new($"{OneHandDmgPerkId}_dmg", ModifierType.Increase, l * Stats.BasicDamageIncrease * 1.5, 
                    [Tags.Damage, Tags.Melee, Tags.OneHandedWeapon], [])
            ])
            { MaxLevel = 10 };

        readonly static Perk TwoHandDamagePerk = new(TwoHandDmgPerkId, Properties.Skills.TwoHandMeleeDamage, "", [],
            (l, e, w, c) => [
                new($"{TwoHandDmgPerkId}_more", ModifierType.More, 0.1,
                    [Tags.Damage, Tags.Melee, Tags.TwoHandedWeapon], []),
                new($"{TwoHandDmgPerkId}_dmg", ModifierType.Increase, l * Stats.BasicDamageIncrease * 1.5,
                    [Tags.Damage, Tags.Melee, Tags.TwoHandedWeapon], [])
            ])
            { MaxLevel = 10 };

        readonly static Perk AttackSpeedPerk = new(AtkSpeedPerkId, Properties.Skills.MeleeAttackSpeed, "", [],
            (l, e, w, c) => [
                new($"{AtkSpeedPerkId}_more", ModifierType.More, 0.1,
                    [Tags.AttackSpeed, Tags.Melee], []),
                new($"{AtkSpeedPerkId}_speed", ModifierType.Increase, l * Stats.BasicAttackSpeedIncrease * 1.5,
                    [Tags.AttackSpeed, Tags.Melee], [])
            ])
            { MaxLevel = 10 };

        readonly static Perk ShieldDefensePerk = new(ShieldDefensePerkId, Properties.Skills.ShieldDefense, "", [],
            (l, e, w, c) => [
                new($"{ShieldDefensePerkId}_more", ModifierType.More, 0.1,
                    [Tags.Defense], []),
                new($"{ShieldDefensePerkId}_shield", ModifierType.Increase, l * Stats.BasicDefenseIncrease * 2.5,
                    [Tags.Defense, Tags.Shield], [])
            ])
            { MaxLevel = 10 };

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
                Reward = new MultiReward([new PerkReward(RootPerk), new PerkReward(HeavyAttack.BasePerk), StarterItems()]),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements("", 0)
            });

            Path.AddStep(new CharacterPathStep(DualWield1Id, Properties.Skills.DualWield1, "")
            {
                Reward = new AbilityReward(Abilities.DualWield),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(RootId, 1)
            });
            Path.AddStep(new CharacterPathStep(DualWield2Id, Properties.Skills.DualWield2, "")
            {
                Reward = new PerkReward(OneHandDamagePerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(DualWield1Id, 1)
            });
            Path.AddStep(new CharacterPathStep(DualWield3Id, Properties.Skills.DualWield3, "")
            {
                Reward = new PerkReward(DoubleStrike.BasePerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(DualWield2Id, 1)
            });

            Path.AddStep(new CharacterPathStep(Shield1Id, Properties.Skills.Shield1, "")
            {
                Reward = new AbilityReward(Abilities.Shielded),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(RootId, 1)
            });
            Path.AddStep(new CharacterPathStep(Shield2Id, Properties.Skills.Shield2, "")
            {
                Reward = new PerkReward(ShieldDefensePerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(Shield1Id, 1)
            });
            Path.AddStep(new CharacterPathStep(Shield3Id, Properties.Skills.Shield3, "")
            {
                Reward = new PerkReward(ShieldCharge.BasePerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(Shield2Id, 1)
            });

            Path.AddStep(new CharacterPathStep(Single1Id, Properties.Skills.SingleHanded1, "")
            {
                Reward = new AbilityReward(Abilities.SingleHanded),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(RootId, 1)
            });
            Path.AddStep(new CharacterPathStep(Single2Id, Properties.Skills.SingleHanded2, "")
            {
                Reward = new PerkReward(AttackSpeedPerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(Single1Id, 1)
            });
            Path.AddStep(new CharacterPathStep(Single3Id, Properties.Skills.SingleHanded3, "")
            {
                Reward = new PerkReward(VitalStrike.BasePerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(Single2Id, 1)
            });

            Path.AddStep(new CharacterPathStep(TwoHand1Id, Properties.Skills.TwoHanded1, "")
            {
                Reward = new AbilityReward(Abilities.TwoHanded),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(RootId, 1)
            });
            Path.AddStep(new CharacterPathStep(TwoHand2Id, Properties.Skills.TwoHanded2, "")
            {
                Reward = new PerkReward(TwoHandDamagePerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(TwoHand1Id, 1)
            });
            Path.AddStep(new CharacterPathStep(TwoHand3Id, Properties.Skills.TwoHanded3, "")
            {
                Reward = new PerkReward(CrushingBlow.BasePerk),
                SpecificAccess = CharacterPathStep.MakeStandardRequirements(TwoHand2Id, 1)
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
