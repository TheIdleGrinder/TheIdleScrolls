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
    public class SmokeBombSkill : ActiveSkill
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

        protected override void SetupStats(Entity user)
        {
            SkillTags = [Tags.Trick, Tags.Duration];

            var perkComp = user.GetComponent<PerksComponent>();
            if (perkComp is null)
                return;

            var modComp = user.GetComponent<ModifierComponent>();

            Perk basePerk = GetPerk(Perks.SmokeBombPerks.BasePerkId)!;

            ChargingTime = 1.0;
            Timer.ActiveDuration = 3.0 + 1.0 * GetPerkLevel(Perks.SmokeBombPerks.BasePerkId);
            if (ActivityWhileInEffects.Count == 0)
            {
                StatusEffect smokeEffect = new GenericModifierStatusEffect("Shrouded in Smoke", 0.0, basePerk.Modifiers, []);
                ActivityWhileInEffects = [smokeEffect];
            }

            int level = GetPerkLevel(Perks.SmokeBombPerks.DamageOnActivityEndId);
            if (level > 0)
            {
                Perk explosionPerk = GetPerk(Perks.SmokeBombPerks.DamageOnActivityEndId)!; // level > 0 => must not be null
                double dmg = explosionPerk.Modifiers[0].Value; // CornerCut: Assume that that perk only has one modifier
                dmg = ScaleValue(dmg, [Tags.Damage]);
                ISkillEffect dmgEffect = new DamageSkillEffect(DamageType.Fire, dmg, [Tags.Damage]);
                CooldownStartEffects = [new(dmgEffect, TargetingMode.SingleEnemy)];
                SkillTags.Add(Tags.Damage);
            }
            else
            {
                CooldownStartEffects = [];
            }

            level = GetPerkLevel(Perks.SmokeBombPerks.DamageWhileActiveId);
            if (level > 0)
            {
                Perk damagePerk = GetPerk(Perks.SmokeBombPerks.DamageWhileActiveId)!;
                double dmg = damagePerk.Modifiers[0].Value;
                dmg = ScaleValue(dmg, [Tags.Damage, Tags.DamageOverTime]);
                if (ActivityRepeatedEffect is null)
                {
                    ActivityRepeatedEffect = new DoTSkillEffectGenerator(DamageType.Poison, dmg);
                }
                (ActivityRepeatedEffect! as DoTSkillEffectGenerator)!.DPS = dmg;
                SkillTags.Add(Tags.DamageOverTime);
            }
            else
            {
                ActivityRepeatedEffect = null;
            }
        }
    }
}
