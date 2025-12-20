using MiniECS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Skills.SkillEffectGenerators;
using TheIdleScrolls_Core.Skills.SkillEffects;
using TheIdleScrolls_Core.StatusEffects;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class SmokeBombSkill : ActiveSkillDefinition
    {
        public static SmokeBombSkill Skill { get; } = new();

        private SmokeBombSkill() { }

        public override string Id => "SmokeBomb";

        public override string Name => "Smoke Bomb";

        public override bool IsAvailableTo(Entity user)
        {
            return user.GetComponent<PerksComponent>()?.IsPerkActive(Perks.SmokeBombPerks.BasePerkId) ?? false;
        }

        public override (bool available, string reason) IsUsableBy(Entity user)
        {
            return (IsAvailableTo(user), string.Empty);
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.Trick, Tags.Duration];

            var perkComp = user.GetComponent<PerksComponent>();
            if (perkComp is null)
                return;

            var modComp = user.GetComponent<ModifierComponent>();

            Perk basePerk = skill.GetPerk(Perks.SmokeBombPerks.BasePerkId)!;

            skill.ChargingTime = 1.0;
            skill.Timer.ActiveDuration = 3.0 + 1.0 * skill.GetPerkLevel(Perks.SmokeBombPerks.BasePerkId);
            if (skill.ActiveEffects.WhileIn.Count == 0)
            {
                StatusEffect smokeEffect = new GenericModifierStatusEffect("Shrouded in Smoke", 0.0, basePerk.Modifiers, []);
                skill.ActiveEffects.WhileIn = [smokeEffect];
            }

            int level = skill.GetPerkLevel(Perks.SmokeBombPerks.DamageOnActivityEndId);
            if (level > 0)
            {
                Perk explosionPerk = skill.GetPerk(Perks.SmokeBombPerks.DamageOnActivityEndId)!; // level > 0 => must not be null
                double dmg = explosionPerk.Modifiers[0].Value; // CornerCut: Assume that that perk only has one modifier
                dmg = skill.ScaleValue(dmg, [Tags.Damage]);
                ISkillEffect dmgEffect = new DamageSkillEffect(dmg, ISkillEffect.TargetingMode.SingleEnemy, [Tags.Damage]);
                skill.CooldownEffects.OnEnter = [dmgEffect];
                skill.Tags.Add(Tags.Damage);
            }
            else
            {
                skill.CooldownEffects.OnEnter = [];
            }

            level = skill.GetPerkLevel(Perks.SmokeBombPerks.DamageWhileActiveId);
            if (level > 0)
            {
                Perk damagePerk = skill.GetPerk(Perks.SmokeBombPerks.DamageWhileActiveId)!;
                double dmg = damagePerk.Modifiers[0].Value;
                dmg = skill.ScaleValue(dmg, [Tags.DamageOverTime]);
                if (skill.ActiveEffects.RepeatedWhileIn is null)
                {
                    skill.ActiveEffects.RepeatedWhileIn = new DoTSkillEffectGenerator(dmg);
                }
                (skill.ActiveEffects.RepeatedWhileIn! as DoTSkillEffectGenerator)!.DPS = dmg;
                skill.Tags.Add(Tags.DamageOverTime);
            }
            else
            {
                skill.ActiveEffects.RepeatedWhileIn = null;
            }
        }
    }
}
