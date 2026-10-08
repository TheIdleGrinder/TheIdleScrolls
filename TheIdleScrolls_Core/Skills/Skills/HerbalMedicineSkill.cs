using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.StatusEffects;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public static class HerbalMedicine
    {
        public const string BasePerkId = "HerbalMedicine";
        public const string FirstModPerkId = BasePerkId + "_mod1";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.HerbalMedicine_Name,
            Properties.Skills.HerbalMedicine_Description,
            [],
            (l, e, w, c) =>
            {
                double reg = Math.Round(l * 8 * Math.Pow(1.2, l));
                return [
                    new($"{BasePerkId}_reg", ModifierType.AddBase,  reg, [Tags.LifeRegeneration], []),
                ];
            }
        )
        {
            MaxLevel = 4,
            ApplyModifiersToOwner = false,
            Skill = new HerbalMedicineSkill(),
            Categories = [Properties.Skills.HerbalMedicine_Name],
            ConditionFunc = PerkExtensions.FlatPerkLevelCondition(PerkIds.Vitality, 5)
        };

        public static readonly Perk FirstModPerk = new(
            FirstModPerkId,
            Properties.Skills.HerbalMedicineFirstMod_Name,
            Properties.Skills.HerbalMedicineFirstMod_Description,
            [],
            (l, e, w, c) =>
            {
                return [
                    new($"{FirstModPerkId}_poisonresist", ModifierType.AddBase, 0.08 * (l + 2), [Tags.Resistance, DamageType.Poison.ToTag()], [])
                    {
                        AlwaysPercentage = true
                    }
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false,
            Categories = [Properties.Skills.HerbalMedicine_Name],
            ConditionFunc = PerkExtensions.FlatPerkLevelCondition(BasePerkId, 1)
        };
    }

    public class HerbalMedicineSkill : ActiveSkill
    {
        public override string Id => HerbalMedicine.BasePerkId;

        public override string Name => Properties.Skills.HerbalMedicine_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, HerbalMedicine.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            if (CurrentState == SkillTimer.State.Active)
                return (UsePrevention.None, "");
            if ((user.GetComponent<AdventurerComponent>()?.State ?? AdventurerState.Idle) != AdventurerState.Resting)
                return (UsePrevention.NotResting, "You can only use this skill while resting.");
            return (UsePrevention.None, "");
        }

        protected override void SetupStats(Entity user)
        {
            SkillTags = [Tags.BuffSkill];

            List<Modifier> mods = [];
            var basePerkMods = GetPerk(HerbalMedicine.BasePerkId)?.Modifiers ?? [];
            mods.AddRange(basePerkMods);
            var firstModMods = GetPerk(HerbalMedicine.FirstModPerkId)?.Modifiers ?? [];
            mods.AddRange(firstModMods);

            if (ActivityWhileInEffects.Count == 0)
            {
                ActivityWhileInEffects.Add(new GenericModifierStatusEffect(Properties.Skills.HerbalMedicine_Name, 0.0, [], []));
            }
            (ActivityWhileInEffects[0] as GenericModifierStatusEffect)!.UpdateModifiers(mods);

            Timer.ChargingDuration = 1.0;
            Timer.ActiveDuration = 60.0;
            Timer.CooldownDuration = 0.0;
        }
    }
}
