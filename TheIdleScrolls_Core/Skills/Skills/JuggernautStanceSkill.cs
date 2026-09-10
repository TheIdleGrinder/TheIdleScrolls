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
    public static class JuggernautStance
    {
        public const string BasePerkId = "JuggernautStance";
        public const string FirstModPerkId = BasePerkId + "_mod1";
        public const string SecondModPerkId = BasePerkId + "_mod2";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.JuggernautName,
            Properties.Skills.JuggernautDescription,
            [],
            (l, e, w, c) =>
            {
                return [
                    new($"{BasePerkId}_armor", ModifierType.More,  0.08 + 0.02 * l, [Tags.ArmorRating, Abilities.HeavyArmor], []),
                ];
            }
        )
        {
            MaxLevel = 6,
            ApplyModifiersToOwner = false,
            Skill = new JuggernautStanceSkill()
        };

        public static readonly Perk FirstModPerk = new(
            FirstModPerkId,
            Properties.Skills.JuggernautFirstModName,
            Properties.Skills.JuggernautFirstModDescription,
            [],
            (l, e, w, c) =>
            {
                return [
                    new($"{FirstModPerkId}_shieldArmor", ModifierType.More, (l + 1) * 0.05, [Tags.ArmorRating, Tags.Shield], []),
                    new($"{FirstModPerkId}_blockMit", ModifierType.AddFlat, (l + 1) / 2 * 0.01, [Tags.BlockMitigation], [])
                        { AlwaysPercentage = true },
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false
        };

        public static readonly Perk SecondModPerk = new(
            SecondModPerkId,
            Properties.Skills.JuggernautSecondModName,
            Properties.Skills.JuggernautSecondModDescription,
            [],
            (l, e, w, c) =>
            {
                return [
                    new($"{SecondModPerkId}_stunResistance", ModifierType.AddBase, 0.25 + l * 0.15, [Tags.Stun, Tags.Resistance], []) 
                        { AlwaysPercentage = true },
                    new($"{SecondModPerkId}_slowResistance", ModifierType.AddBase, 0.25 + l * 0.15, [Tags.Slow, Tags.Resistance], [])
                        { AlwaysPercentage = true },
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false
        };
    }

    public class JuggernautStanceSkill : ActiveSkill
    {
        public override string Id => JuggernautStance.BasePerkId;

        public override string Name => "Juggernaut's Stance";

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, JuggernautStance.BasePerkId);
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
            var basePerkMods = GetPerk(JuggernautStance.BasePerkId)?.Modifiers ?? [];
            mods.AddRange(basePerkMods);
            var firstModMods = GetPerk(JuggernautStance.FirstModPerkId)?.Modifiers ?? [];
            mods.AddRange(firstModMods);
            var secondModMods = GetPerk(JuggernautStance.SecondModPerkId)?.Modifiers ?? [];
            mods.AddRange(secondModMods);

            if (ActivityWhileInEffects.Count == 0)
            {
                ActivityWhileInEffects.Add(new GenericModifierStatusEffect("Juggernaut", 0.0, [], []));
            }
            (ActivityWhileInEffects[0] as GenericModifierStatusEffect)!.UpdateModifiers(mods);
            
            Timer.ChargingDuration = 0.0;
            Timer.ActiveDuration = 3600.0;
            Timer.CooldownDuration = 1.0;
        }
    }
}
