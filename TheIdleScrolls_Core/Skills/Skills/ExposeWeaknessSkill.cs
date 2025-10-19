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

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class ExposeWeaknessSkill : ActiveSkillDefinition
    {
        public static ExposeWeaknessSkill Skill { get; } = new();

        private ExposeWeaknessSkill() { }

        public override string Id => "ExposeWeakness";

        public override string Name => "Expose Weakness";

        public override bool IsAvailableTo(Entity user)
        {
            var perksComp = user.GetComponent<PerksComponent>();
            if (perksComp is null)
                return false;

            return perksComp.IsPerkActive(Perks.ExposeWeaknessPerks.BasePerkId);
        }

        public override (bool available, string reason) IsUsableBy(Entity user)
        {
            return (IsAvailableTo(user), string.Empty);
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.Insight];

            double cooldown = 10.0;
            double chargeTime = 2.0;
            chargeTime = user.ApplyAllApplicableModifiers(chargeTime, [Tags.ChargeSpeed, Id, .. skill.Tags], user.GetTags());

            var perksComp = user.GetComponent<PerksComponent>();
            if (perksComp is null)
                return;

            List<Modifier> mods = perksComp.GetPerk(Perks.ExposeWeaknessPerks.BasePerkId)?.Modifiers.ToList() ?? [];
            if (perksComp.IsPerkActive(Perks.ExposeWeaknessPerks.DmgTakenPerkId))
            {
                mods.AddRange(perksComp.GetPerk(Perks.ExposeWeaknessPerks.DmgTakenPerkId)?.Modifiers.ToList() ?? []);
            }

            //List<string> perks = [Perks.ExposeWeaknessPerks.DmgTakenPerkId];
            //mods.AddRange(perks
            //    .Where(perksComp.IsPerkActive)
            //    .SelectMany(id => perksComp.GetPerks().FirstOrDefault(p => p.Id == id)?.Modifiers ?? [])
            //    .ToList());

            GenericModifierStatusEffect effect = new("Exposed", 12.0, mods, []);

            skill.Timer.ChargingDuration = chargeTime;
            skill.Timer.CooldownDuration = cooldown;
            skill.ActivationEffects = [new StatusSkillEffect(ISkillEffect.TargetingMode.SingleEnemy, effect)];
        }
    }
}
