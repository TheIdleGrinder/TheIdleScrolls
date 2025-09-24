using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Skills
{
    public class SkillFactory
    {
        public static Action<Entity, ActiveSkill> GetGenericUpdater(double chargeTime, double cooldown, List<Func<Entity, ISkillEffect>> scalers)
        {
            return (user, skill) =>
            {
                skill.Effects = scalers.Select(s => s(user)).ToList();
                skill.Timer.ChargingDuration = chargeTime;
                skill.Timer.CooldownDuration = cooldown;
            };
        }

        public static Func<Entity, ISkillEffect> GetStunScaler(double baseDuration)
            => (user) => new SkillEffects.StatusSkillEffect(
                ISkillEffect.TargetingMode.SingleEnemy,
                new StatusEffects.StunStatusEffect(user.ApplyAllApplicableModifiers(baseDuration, ["Stun", "Duration"], user.GetTags())));
    }
}
