using System;
using System.Collections.Generic;
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
using TheIdleScrolls_Core.Skills.Skills;
using TheIdleScrolls_Core.Properties;

namespace TheIdleScrolls_Core.CharacterPaths.Paths
{
    public static class FighterPath
    {
        const string PathId = "fighter";
        public const string RootId = "fighter_root";
        public const string Life1Id = "fighter_hp1";
        public const string Life2Id = "fighter_hp2";
        public const string Life3Id = "fighter_hp3";
        public const string Life4Id = "fighter_hp4";
        public const string HeavyWeaponsId = "heavyWeapons";
        const string DualWield1Id = "dualwield1";
        const string DualWield2Id = "dualwield2";
        const string DualWield3Id = "dualwield3";
        const string DualWield4Id = "dualwield4";
        const string Shield1Id = "shielded1";
        const string Shield2Id = "shielded2";
        const string Shield3Id = "shielded3";
        const string Single1Id = "singlehanded1";
        const string Single2Id = "singlehanded2";
        const string Single3Id = "singlehanded3";
        const string TwoHand1Id = "twohanded1";
        const string TwoHand2Id = "twohanded2";
        const string TwoHand3Id = "twohanded3";

        const string OneHandDmgPerkId = "fighter_onehanddmg";
        const string TwoHandDmgPerkId = "fighter_twohanddmg";
        const string AtkSpeedPerkId = "fighter_atkspeed";
        const string ShieldDefensePerkId = "fighter_shielddefense";

        const string MomentumPerkId = "fighter_momentum";

        public static CharacterPath Path { get; } = new CharacterPath(PathId, Properties.Skills.PathFighter, Properties.Skills.PathFighter);

        readonly static Perk RootPerk = new(RootId, Properties.Skills.PathFighterRoot,
            "Grants increased melee damage and armor for each step taken on this path", [],
            (l, e, w, c) =>
            {
                int steps = e.GetComponent<CharacterPathComponent>()!.StepsTakenOnPath(PathId).Count;
                return [
                new($"{RootId}_dmg", ModifierType.Increase, 0.1 * steps, [Tags.Damage, Tags.Melee], []),
                new($"{RootId}_arm", ModifierType.Increase, 0.1 * steps, [Tags.ArmorRating], [])];
            }
        )
        {
            Permanent = true,
            Categories = [Properties.Skills.PathFighter]
        };

        readonly static Perk LifePerLevelPerk = new(Life1Id, Properties.Skills.FighterHP1,
            "Gain 2 additional hit points per character level", [],
            (l, e, w, c) => [
                new($"{Life1Id}", ModifierType.AddBase, 2 * e.GetLevel(), [Tags.HitPoints], [])
            ])
        { Permanent = true, Categories = [Properties.Skills.PathFighter] };
        readonly static Perk FighterIncLife = new(Life2Id, Properties.Skills.FighterHP2,
            "Gain increased hit points for each level and an additional multiplier at maximum level", [],
            (l, e, w, c) => 
            {
                double perLevel = Math.Round(Stats.BasicTimeIncrease * 1.25, 2);
                List<Modifier> result = [new($"{Life2Id}_inc", ModifierType.Increase, perLevel * l, [Tags.HitPoints], [])];
                if (l == 5)
                {
                    result.Add(new($"{Life2Id}_more", ModifierType.More, 0.05, [Tags.HitPoints], []));
                }
                return result;
            })
        { MaxLevel = 5, Categories = [Properties.Skills.PathFighter] };
        readonly static Perk FighterIncLifeAndReg = new(Life3Id, Properties.Skills.FighterHP3,
            "Gain increased hit points and life regeneration for each level", 
            [UpdateTrigger.BattleStarted, UpdateTrigger.BattleFinished, UpdateTrigger.LevelUp, UpdateTrigger.EquipmentChanged],
            (l, e, w, c) =>
            {
                int totalHp = e.GetComponent<LifePoolComponent>()!.Maximum;
                double incPerLevel = Math.Round(Stats.BasicTimeIncrease * 1.25, 2);
                double reg = 0.003 * l;
                if (l == 5)
                    reg += 0.005;
                return [
                    new($"{Life3Id}_inc", ModifierType.Increase, incPerLevel * l, [Tags.HitPoints], []),
                    new($"{Life3Id}_reg", ModifierType.AddBase, reg * totalHp, [Tags.LifeRegeneration], [])
                    {
                        AlwaysPercentage = true,
                        CoverValue = reg,
                        CoverText = "Regenerate {0} HP per second"
                    }
                ];
            })
        { MaxLevel = 5, Categories = [Properties.Skills.PathFighter] };

        readonly static Perk HeavyWeaponsPerk = new(HeavyWeaponsId, Properties.Skills.HeavyWeapons, 
            "Deal more damage with the heavier weapon types", [],
            (l, e, w, c) =>
            {
                double value = l * 1.5 * Stats.BasicDamageIncrease;
                List<Modifier> returnList = [
                    new($"{HeavyWeaponsId}_axe", ModifierType.Increase, value, [Tags.Damage, Abilities.Axe], []),
                    new($"{HeavyWeaponsId}_bln", ModifierType.Increase, value, [Tags.Damage, Abilities.Blunt], []),
                    new($"{HeavyWeaponsId}_lbl", ModifierType.Increase, value, [Tags.Damage, Abilities.LongBlade], [])
                ];
                if (l == 5)
                {
                    returnList.Add(new($"{HeavyWeaponsId}_more", ModifierType.More, 0.05, [Tags.Damage, Tags.Melee], []));
                }
                return returnList;
            })
        { MaxLevel = 5, Categories = [Properties.Skills.PathFighter] };

        readonly static Perk OneHandDamagePerk = new(OneHandDmgPerkId, Properties.Skills.OneHandMeleeDamage, "", [],
            (l, e, w, c) => [
                new($"{OneHandDmgPerkId}_more", ModifierType.More, 0.1, 
                    [Tags.Damage, Tags.Melee, Tags.OneHandedWeapon], []),
                new($"{OneHandDmgPerkId}_dmg", ModifierType.Increase, l * Stats.BasicDamageIncrease * 1.5, 
                    [Tags.Damage, Tags.Melee, Tags.OneHandedWeapon], [])
            ])
            { MaxLevel = 10, Categories = [Properties.Skills.PathFighter] };

        readonly static Perk TwoHandDamagePerk = new(TwoHandDmgPerkId, Properties.Skills.TwoHandMeleeDamage, "", [],
            (l, e, w, c) => [
                new($"{TwoHandDmgPerkId}_more", ModifierType.More, 0.1,
                    [Tags.Damage, Tags.Melee, Tags.TwoHandedWeapon], []),
                new($"{TwoHandDmgPerkId}_dmg", ModifierType.Increase, l * Stats.BasicDamageIncrease * 1.5,
                    [Tags.Damage, Tags.Melee, Tags.TwoHandedWeapon], [])
            ])
            { MaxLevel = 10, Categories = [Properties.Skills.PathFighter] };

        readonly static Perk AttackSpeedPerk = new(AtkSpeedPerkId, Properties.Skills.MeleeAttackSpeed, "", [],
            (l, e, w, c) => [
                new($"{AtkSpeedPerkId}_more", ModifierType.More, 0.1,
                    [Tags.AttackSpeed, Tags.Melee], []),
                new($"{AtkSpeedPerkId}_speed", ModifierType.Increase, l * Stats.BasicAttackSpeedIncrease * 1.5,
                    [Tags.AttackSpeed, Tags.Melee], [])
            ])
            { MaxLevel = 10, Categories = [Properties.Skills.PathFighter] };

        readonly static Perk ShieldDefensePerk = new(ShieldDefensePerkId, Properties.Skills.ShieldDefense, "", [],
            (l, e, w, c) => [
                new($"{ShieldDefensePerkId}_more", ModifierType.More, 0.1,
                    [Tags.Defense], []),
                new($"{ShieldDefensePerkId}_shield", ModifierType.Increase, l * Stats.BasicDefenseIncrease * 2.5,
                    [Tags.Defense, Tags.Shield], [])
            ])
            { MaxLevel = 10, Categories = [Properties.Skills.PathFighter] };

        readonly static Perk MomentumPerk = new(MomentumPerkId, "Momentum", "Gain momentum every time you hit an enemy with attacks",
            [],
            (l, e, w, c) => [
                new($"{MomentumPerkId}_limit", ModifierType.AddBase, l + 1, [Tags.MomentumLimit], [])
            ])
            { MaxLevel = 9, Categories = [Properties.Skills.PathFighter, "Momentum"] };

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
                Reward = new MultiReward([new PerkReward(RootPerk), 
                    new PerkReward(HeavyAttack.BasePerk
                        .WithCategories(Properties.Skills.PathFighter, Properties.Skills.HeavyAttack_Name)), 
                    StarterItems()]),
                StepNumber = 0
            });

            Path.AddStep(SimplePerkStep(LifePerLevelPerk,     1));
            Path.AddStep(SimplePerkStep(FighterIncLife,       2, LifePerLevelPerk.Id));
            Path.AddStep(SimplePerkStep(FighterIncLifeAndReg, 3, FighterIncLife.Id));
            Path.AddStep(SimplePerkStep(Unrelenting.BasePerk.WithCategories(Properties.Skills.PathFighter), 4, FighterIncLifeAndReg.Id));

            Path.AddStep(SimplePerkStep(HeavyWeaponsPerk, 1));

            Path.AddStep(SimplePerkStep(DoubleSwing.BasePerk.WithCategories(Properties.Skills.PathFighter), 1));

            Path.AddStep(new CharacterPathStep(DualWield2Id, Properties.Skills.DualWield2, "")
            {
                Reward = new PerkReward(OneHandDamagePerk),
                StepNumber = 2,
                PrerequisiteId = DoubleSwing.BasePerkId
            });
            Path.AddStep(new CharacterPathStep(DualWield3Id, Properties.Skills.DualWield3, "")
            {
                Reward = new PerkReward(DoubleStrike.BasePerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.DoubleStrike_Name)),
                StepNumber = 3,
                PrerequisiteId = DualWield2Id
            });

            Path.AddStep(new CharacterPathStep(Shield2Id, Properties.Skills.Shield2, "")
            {
                Reward = new PerkReward(ShieldDefensePerk),
                StepNumber = 1,
                PrerequisiteId = null
            });
            Path.AddStep(new CharacterPathStep(Shield3Id, Properties.Skills.Shield3, "")
            {
                Reward = new PerkReward(ShieldCharge.BasePerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.ShieldCharge_Name)),
                StepNumber = 3,
                PrerequisiteId = Shield2Id
            });

            Path.AddStep(new CharacterPathStep(Single2Id, Properties.Skills.SingleHanded2, "")
            {
                Reward = new PerkReward(AttackSpeedPerk),
                StepNumber = 1,
                PrerequisiteId = null
            });
            Path.AddStep(new CharacterPathStep(Single3Id, Properties.Skills.SingleHanded3, "")
            {
                Reward = new PerkReward(VitalStrike.BasePerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.VitalStrike_Name)),
                StepNumber = 3,
                PrerequisiteId = Single2Id
            });

            Path.AddStep(new CharacterPathStep(TwoHand2Id, Properties.Skills.TwoHanded2, "")
            {
                Reward = new PerkReward(TwoHandDamagePerk),
                StepNumber = 1,
                PrerequisiteId = null
            });
            Path.AddStep(new CharacterPathStep(TwoHand3Id, Properties.Skills.TwoHanded3, "")
            {
                Reward = new PerkReward(CrushingBlow.BasePerk.WithCategories(Properties.Skills.PathFighter, LocalizedStrings.ABL_TWOHANDED)),
                StepNumber = 3,
                PrerequisiteId = TwoHand2Id
            });

            Path.AddStep(SimplePerkStep(MomentumPerk, 1));

            Path.AddStep(SimplePerkStep(JuggernautStance.BasePerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.JuggernautName), 2));
            Path.AddStep(SimplePerkStep(JuggernautStance.FirstModPerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.JuggernautName), 
                3, JuggernautStance.BasePerkId));
            Path.AddStep(SimplePerkStep(JuggernautStance.SecondModPerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.JuggernautName), 
                4, JuggernautStance.BasePerkId));

            Path.AddStep(SimplePerkStep(BerserkerStance.BasePerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.BerserkerName), 2));
            Path.AddStep(SimplePerkStep(BerserkerStance.FirstModPerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.BerserkerName),
                3, BerserkerStance.BasePerkId));
            Path.AddStep(SimplePerkStep(BerserkerStance.SecondModPerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.BerserkerName),
                4, BerserkerStance.BasePerkId));

            Path.AddStep(new CharacterPathStep(EnvenomWeapon.BasePerkId, EnvenomWeapon.BasePerk.Name, EnvenomWeapon.BasePerk.Description)
            {
                Reward = new PerkReward(EnvenomWeapon.BasePerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.EnvWeapon_Name)),
                StepNumber = 2
            }); 
            Path.AddStep(new CharacterPathStep(EnvenomWeapon.DurationPerkId, EnvenomWeapon.SupportPerk.Name, EnvenomWeapon.SupportPerk.Description)
            {
                Reward = new PerkReward(EnvenomWeapon.SupportPerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.EnvWeapon_Name)),
                StepNumber = 3,
                PrerequisiteId = EnvenomWeapon.BasePerkId
            }); 

            Path.AddStep(new CharacterPathStep(BlazingWeapon.BasePerkId, BlazingWeapon.BasePerk.Name, BlazingWeapon.BasePerk.Description)
            {
                Reward = new PerkReward(BlazingWeapon.BasePerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.BlazingWeapon_Name)),
                StepNumber = 2
            });
            Path.AddStep(new CharacterPathStep(BlazingWeapon.SupportPerkId, BlazingWeapon.SupportPerk.Name, BlazingWeapon.SupportPerk.Description)
            {
                Reward = new PerkReward(BlazingWeapon.SupportPerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.BlazingWeapon_Name)),
                StepNumber = 3,
                PrerequisiteId = BlazingWeapon.BasePerkId
            });

            Path.AddStep(new CharacterPathStep(BattleCry.BasePerkId, BattleCry.BasePerk.Name, BattleCry.BasePerk.Description)
            {
                Reward = new PerkReward(BattleCry.BasePerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.BattleCry_Name)),
                StepNumber = 3
            });
            Path.AddStep(new CharacterPathStep(BattleCry.BuffPerkId, BattleCry.BuffPerk.Name, BattleCry.BuffPerk.Description)
            {
                Reward = new PerkReward(BattleCry.BuffPerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.BattleCry_Name)),
                StepNumber = 4,
                PrerequisiteId = BattleCry.BasePerkId
            });
            Path.AddStep(new CharacterPathStep(BattleCry.DebuffPerkId, BattleCry.DebuffPerk.Name, BattleCry.DebuffPerk.Description)
            {
                Reward = new PerkReward(BattleCry.DebuffPerk.WithCategories(Properties.Skills.PathFighter, Properties.Skills.BattleCry_Name)),
                StepNumber = 5,
                PrerequisiteId = BattleCry.BasePerkId
            });

            Path.BuildTopology(RootId);
        }

        static CharacterPathStep SimplePerkStep(Perk perk, int stepNumber, string? prerequisiteId = null)
        {
            return new CharacterPathStep(perk.Id, perk.Name, perk.Description)
            {
                Reward = new PerkReward(perk),
                StepNumber = stepNumber,
                PrerequisiteId = prerequisiteId
            };
        }
    }
}
