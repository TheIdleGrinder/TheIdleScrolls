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
    public class ExposeWeaknessSkill : ActiveSkill
    {
        public static ExposeWeaknessSkill Skill { get; } = new();

        private ExposeWeaknessSkill() { }

        public override string Id => "ExposeWeakness";

        public override string Name => "Expose Weakness";

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, Perks.ExposeWeaknessPerks.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            return (user.IsInBattle() ? UsePrevention.None : UsePrevention.NotInBattle, string.Empty);
        }

        protected override void SetupStats(Entity user)
        {
            SkillTags = [Tags.Insight];

            double cooldown = 10.0;
            double chargeTime = 2.0;
            chargeTime = user.ApplyAllApplicableModifiers(chargeTime, [Tags.ChargeSpeed, Id, .. SkillTags], user.GetTags());
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

            Timer.ChargingDuration = chargeTime;
            Timer.CooldownDuration = cooldown;
            ActivityStartEffects = [
                new([new StatusSkillEffect(effect)], TargetingMode.SingleEnemy, SkillTags)
            ];
        }
    }
}
