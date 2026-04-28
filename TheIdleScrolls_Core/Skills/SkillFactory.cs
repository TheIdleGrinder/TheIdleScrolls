using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.StatusEffects;

namespace TheIdleScrolls_Core.Skills
{
    public class SkillFactory
    {
        public static Action<Entity, ActiveSkill> GetGenericUpdater(double chargeTime, double cooldown, List<Func<Entity, SkillEffectBundle>> scalers)
        {
            return (user, skill) =>
            {
                skill.ActiveEffects.OnEnter = scalers.Select(s => s(user)).ToList();
                skill.Timer.ChargingDuration = chargeTime;
                skill.Timer.CooldownDuration = cooldown;
            };
        }

        public static Func<Entity, SkillEffectBundle> GetStunScaler(double baseDuration)
            => (user) => new SkillEffectBundle
            {
                Target = TargetingMode.SingleEnemy,
                Effects = [
                    new SkillEffects.StatusSkillEffect(
                        new StunStatusEffect(user.ApplyAllApplicableModifiers(baseDuration, ["Stun", "Duration"], user.GetTags()))
                    )
                ]
            };
    }
}
