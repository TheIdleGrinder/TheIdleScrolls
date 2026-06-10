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

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            return (user.IsInBattle() ? UsePrevention.None : UsePrevention.NotInBattle, string.Empty);
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
            if (skill.ActivityWhileInEffects.Count == 0)
            {
                StatusEffect smokeEffect = new GenericModifierStatusEffect("Shrouded in Smoke", 0.0, basePerk.Modifiers, []);
                skill.ActivityWhileInEffects = [smokeEffect];
            }

            int level = skill.GetPerkLevel(Perks.SmokeBombPerks.DamageOnActivityEndId);
            if (level > 0)
            {
                Perk explosionPerk = skill.GetPerk(Perks.SmokeBombPerks.DamageOnActivityEndId)!; // level > 0 => must not be null
                double dmg = explosionPerk.Modifiers[0].Value; // CornerCut: Assume that that perk only has one modifier
                dmg = skill.ScaleValue(dmg, [Tags.Damage]);
                ISkillEffect dmgEffect = new DamageSkillEffect(DamageType.Fire, dmg, [Tags.Damage]);
                skill.CooldownEffects.OnEnter = [new(dmgEffect, TargetingMode.SingleEnemy)];
                skill.Tags.Add(Tags.Damage);
            }
            else
            {
                skill.CooldownStartEffects = [];
            }

            level = skill.GetPerkLevel(Perks.SmokeBombPerks.DamageWhileActiveId);
            if (level > 0)
            {
                Perk damagePerk = skill.GetPerk(Perks.SmokeBombPerks.DamageWhileActiveId)!;
                double dmg = damagePerk.Modifiers[0].Value;
                dmg = skill.ScaleValue(dmg, [Tags.Damage, Tags.DamageOverTime]);
                if (skill.ActivityRepeatedEffect is null)
                {
                    skill.ActivityRepeatedEffect = new DoTSkillEffectGenerator(DamageType.Poison, dmg);
                }
                (skill.ActivityRepeatedEffect! as DoTSkillEffectGenerator)!.DPS = dmg;
                skill.Tags.Add(Tags.DamageOverTime);
            }
            else
            {
                skill.ActivityRepeatedEffect = null;
            }
        }
    }
}
