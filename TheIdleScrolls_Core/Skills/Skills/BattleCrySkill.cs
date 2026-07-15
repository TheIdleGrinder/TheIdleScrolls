using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Perks;
using TheIdleScrolls_Core.Skills.SkillEffects;
using TheIdleScrolls_Core.StatusEffects;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class BattleCrySkill : ActiveSkill
    {
        public static BattleCrySkill Skill { get; } = new();

        public override string Id => BattleCry.BasePerkId;

        public override string Name => Properties.Skills.BattleCry_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, BattleCry.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            return (user.IsInBattle() ? UsePrevention.None : UsePrevention.NotInBattle, string.Empty);
        }

        protected override void SetupStats(Entity user)
        {
            SkillTags = [Tags.BuffSkill, Tags.DurationSkill];

            Perk basePerk = GetPerk(BattleCry.BasePerkId)!;
            int baseLevel = basePerk.CurrentLevel;
            Perk? buffPerk = GetPerk(BattleCry.BuffPerkId);
            List<Modifier> mods = [];

            double chargeSpeed = ScaleValue(1.0, [.. SkillTags, Tags.Speed], mods);
            double duration = ScaleValue(1.0, [.. SkillTags, Tags.Duration], mods);
            double recovery = ScaleValue(1.0, [.. SkillTags, Tags.CooldownRecovery], mods);

            Timer.ChargingDuration = 0.5 / (chargeSpeed != 0.0 ? chargeSpeed : 1.0);
            Timer.ActiveDuration = (3.0 + baseLevel) * duration;
            Timer.CooldownDuration = 5.0 / (recovery != 0.0 ? recovery : 1.0);
            List<Modifier> buffEffectMods = basePerk.Modifiers.ToList();
            if (GetPerkLevel(BattleCry.BuffPerkId) > 0)
                buffEffectMods.AddRange(buffPerk!.Modifiers);
            StatusEffect buff = new GenericModifierStatusEffect(Properties.Skills.BattleCry_Name, 0.0, buffEffectMods, []);
            ActivityWhileInEffects = [buff];

            if (GetPerkLevel(BattleCry.DebuffPerkId) > 0)
            {
                Perk debuffPerk = GetPerk(BattleCry.DebuffPerkId)!;
                SlowStatusEffect debuff = new(Timer.ActiveDuration, debuffPerk.Modifiers[0].Value);
                ActivityStartEffects = [new([new StatusSkillEffect(debuff)], TargetingMode.SingleEnemy, SkillTags)];
            }
        }
    }
}
