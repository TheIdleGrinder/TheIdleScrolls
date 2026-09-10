using MiniECS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.StatusEffects;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public static class BerserkerStance
    {
        public const string BasePerkId = "BerserkerStance";
        public const string FirstModPerkId = BasePerkId + "_mod1";
        public const string SecondModPerkId = BasePerkId + "_mod2";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.BerserkerName,
            Properties.Skills.BerserkerDescription,
            [],
            (l, e, w, c) =>
            {
                return [
                    new($"{BasePerkId}_dmg", ModifierType.More,  0.05 + 0.02 * l, [Tags.Attack, Tags.Damage], []),
                ];
            }
        )
        {
            MaxLevel = 6,
            ApplyModifiersToOwner = false,
            Skill = new BerserkerStanceSkill()
        };

        public static readonly Perk FirstModPerk = new(
            FirstModPerkId,
            Properties.Skills.BerserkerFirstModName,
            Properties.Skills.BerserkerFirstModDescription,
            [],
            (l, e, w, c) =>
            {
                return [
                    new($"{FirstModPerkId}_dmg", ModifierType.AddBase, 2 * (l + 2), [Tags.Attack, Tags.Damage, DamageType.Physical.ToTag()], [])
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false
        };

        public static readonly Perk SecondModPerk = new(
            SecondModPerkId,
            Properties.Skills.BerserkerSecondModName,
            Properties.Skills.BerserkerSecondModDescription,
            [],
            (l, e, w, c) =>
            {
                return [
                    new($"{SecondModPerkId}_dmg", ModifierType.More, 0.08 + l * 0.02, [Tags.AttackSkill, Tags.Damage], [Tags.LowLife]),
                    new($"{SecondModPerkId}_spd", ModifierType.More, 0.05 + l * 0.01, [Tags.AttackSkill, Tags.AttackSpeed], [Tags.LowLife]),
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false
        };
    }

    public class BerserkerStanceSkill : ActiveSkill
    {
        public override string Id => BerserkerStance.BasePerkId;

        public override string Name => "Berserker's Stance";

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, BerserkerStance.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            int limit = FocusSkill.ActiveFocusSkillLimit(user);
            if (FocusSkill.ActiveFocusSkills(user, this).Count >= limit)
            {
                return (UsePrevention.WrongEquipment, $"Can only have {limit} active Focus");
            }
            return (UsePrevention.None, "");
        }

        protected override void SetupStats(Entity user)
        {
            SkillTags = [Tags.FocusSkill, Tags.BuffSkill];

            List<Modifier> mods = [];
            var basePerkMods = GetPerk(BerserkerStance.BasePerkId)?.Modifiers ?? [];
            mods.AddRange(basePerkMods);
            var firstModMods = GetPerk(BerserkerStance.FirstModPerkId)?.Modifiers ?? [];
            mods.AddRange(firstModMods);
            var secondModMods = GetPerk(BerserkerStance.SecondModPerkId)?.Modifiers ?? [];
            mods.AddRange(secondModMods);

            if (ActivityWhileInEffects.Count == 0)
            {
                ActivityWhileInEffects.Add(new GenericModifierStatusEffect("Berserker", 0.0, [], []));
            }
            (ActivityWhileInEffects[0] as GenericModifierStatusEffect)!.UpdateModifiers(mods);
            
            Timer.ChargingDuration = 0.0;
            Timer.ActiveDuration = 3600.0;
            Timer.CooldownDuration = 1.0;
        }
    }
}
