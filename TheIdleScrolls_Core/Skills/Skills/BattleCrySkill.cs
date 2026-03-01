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
    public class BattleCrySkill : ActiveSkillDefinition
    {
        public static BattleCrySkill Skill { get; } = new();

        public override string Id => BattleCry.BasePerkId;

        public override string Name => Properties.Skills.BattleCry_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, BattleCry.BasePerkId);
        }

        public override (bool available, string reason) IsUsableBy(Entity user)
        {
            return (IsAvailableTo(user), "");
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.BuffSkill, Tags.DurationSkill];

            Perk basePerk = skill.GetPerk(BattleCry.BasePerkId)!;
            int baseLevel = basePerk.CurrentLevel;
            Perk? buffPerk = skill.GetPerk(BattleCry.BuffPerkId);
            List<Modifier> mods = [];

            double chargeSpeed = skill.ScaleValue(1.0, [.. skill.Tags, Tags.Speed], mods);
            double duration = skill.ScaleValue(1.0, [.. skill.Tags, Tags.Duration], mods);
            double recovery = skill.ScaleValue(1.0, [.. skill.Tags, Tags.CooldownRecovery], mods);

            skill.Timer.ChargingDuration = 0.5 / (chargeSpeed != 0.0 ? chargeSpeed : 1.0);
            skill.Timer.ActiveDuration = (3.0 + baseLevel) * duration;
            skill.Timer.CooldownDuration = 5.0 / (recovery != 0.0 ? recovery : 1.0);

            List<Modifier> buffEffectMods = basePerk.Modifiers.ToList();
            if (skill.GetPerkLevel(BattleCry.BuffPerkId) > 0)
                buffEffectMods.AddRange(buffPerk!.Modifiers);
            StatusEffect buff = new GenericModifierStatusEffect(Properties.Skills.BattleCry_Name, 0.0, buffEffectMods, []);
            skill.ActivityWhileInEffects = [buff];

            if (skill.GetPerkLevel(BattleCry.DebuffPerkId) > 0)
            {
                Perk debuffPerk = skill.GetPerk(BattleCry.DebuffPerkId)!;
                SlowStatusEffect debuff = new(skill.Timer.ActiveDuration, debuffPerk.Modifiers[0].Value);
                skill.ActivityStartEffects = [new StatusSkillEffect(ISkillEffect.TargetingMode.SingleEnemy, debuff)];
            }
        }
    }
}
