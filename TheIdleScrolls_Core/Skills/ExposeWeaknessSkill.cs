using MiniECS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Skills.SkillEffects;
using TheIdleScrolls_Core.StatusEffects;

namespace TheIdleScrolls_Core.Skills
{
    public static class ExposeWeaknessSkill
    {
        public static readonly ActiveSkillDefinition Skill = new(
            "ExposeWeakness",
            "Expose Weakness",
            UpdateFunction
        );

        public static void UpdateFunction(Entity user, ActiveSkill skill)
        {
            List<string> AdditionalTags = [Tags.Spell, skill.Id];

            double cooldown = 10.0;
            double chargeTime = 2.0;
            chargeTime = user.ApplyAllApplicableModifiers(chargeTime, [Tags.ChargeSpeed, ..AdditionalTags], user.GetTags());

            var perksComp = user.GetComponent<PerksComponent>();
            if (perksComp is null)
                return;

            List<string> perks = [Perks.ExposeWeaknessPerks.BasePerkId, Perks.ExposeWeaknessPerks.DmgTakenPerkId];
            var mods = perks
                .Where(perksComp.IsPerkActive)
                .SelectMany(id => perksComp.GetPerks().FirstOrDefault(p => p.Id == id)?.Modifiers ?? [])
                .ToList();

            GenericModifierStatusEffect effect = new("Exposed", 12.0, mods, []);
            
            skill.Timer.ChargingDuration = chargeTime;
            skill.Timer.CooldownDuration = cooldown;
            skill.Effects = [new StatusSkillEffect(ISkillEffect.TargetingMode.SingleEnemy, effect)];
        }
    }
}
