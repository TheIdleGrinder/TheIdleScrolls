using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Perks;
using TheIdleScrolls_Core.Skills.SkillEffects;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class UnrelentingSkill : ActiveSkill
    {
        public override string Id => Unrelenting.BasePerkId;
        public override string Name => "Unrelenting";
        public override UseTrigger Trigger => UseTrigger.OnKill;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, Unrelenting.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            return (UsePrevention.None, string.Empty);
        }

        protected override void SetupStats(Entity user)
        {
            var pct = user.GetComponent<PerksComponent>()
                ?.GetPerk(Unrelenting.BasePerkId)?.GetModifier(Unrelenting.BasePerkPercentageId)?.Value ?? 0.0;
            if (pct > 0)
            {
                TriggerChance = 1.0;
                double totalHp = user.GetComponent<LifePoolComponent>()?.Maximum ?? 0.0;
                double toHeal = totalHp * pct;
                var effect = new HealingSkillEffect(toHeal, [Definitions.Tags.Healing]);
                ActivityStartEffects = [new(effect, TargetingMode.Self, [Definitions.Tags.Healing])];
                ChargingTime = 0.0;
                Timer.CooldownDuration = 0.0;
            }
            else
            {
                TriggerChance = 1.0;
                ActivityStartEffects = [];
            }
        }
    }
}
