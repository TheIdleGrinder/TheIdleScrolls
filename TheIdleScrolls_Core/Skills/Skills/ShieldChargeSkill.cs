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
using TheIdleScrolls_Core.Properties;
using TheIdleScrolls_Core.StatusEffects;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class ShieldChargeSkill : ActiveSkillDefinition
    {
        public static ShieldChargeSkill Skill { get; } = new();
        public override string Id => ShieldCharge.BasePerkId;

        public override string Name => Properties.Skills.ShieldCharge_Name;

        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, ShieldCharge.BasePerkId);
        }

        public override (bool available, string reason) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
            {
                return (false, string.Empty);
            }

            bool hasShield = user.GetComponent<EquipmentComponent>()?.GetItems()?.Any(i => i.IsShield()) ?? false;

            return (hasShield, hasShield ? "" : "Requires Shield");
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.AttackSkill];

            Entity? weapon = user.GetComponent<EquipmentComponent>()?.GetItems()?.FirstOrDefault(i => i.IsWeapon());
            DamageCluster baseDamage = new(DamageType.Physical, Stats.UnarmedBaseDamage);
            HashSet<string> tags = [];
            if (weapon is null)
            {
                tags = [Tags.Unarmed, Tags.Melee, Tags.Attack];
            }
            else
            {
                tags = weapon.GetTags().ToHashSet();
                baseDamage = weapon.GetComponent<WeaponComponent>()?.Damage ?? baseDamage;
            }

            var attackComp = user.GetComponent<AttackComponent>();
            var perk = user.GetComponent<PerksComponent>()?.GetPerk(ShieldCharge.BasePerkId);
            if (attackComp is null || attackComp.AttackVectors.Count == 0 || perk is null)
            {
                return;
            }

            DamageCluster damage = skill.ScaleDamage(baseDamage, tags, perk?.Modifiers);            
                        
            skill.ActivityStartEffects = DefaultAttack.CreateDefaultSkillEffectsForDamage(damage, [.. skill.Tags]);
            skill.ChargingTime = attackComp.AttackVectors[0].Cooldown;
            skill.Timer.CooldownDuration = 5.0;

            Modifier defMod = perk?.GetModifier(ShieldCharge.BasePerkArmorModId)!;
            skill.ChargingWhileInEffects = [new GenericModifierStatusEffect("Raised Shield", 0.0, [defMod], [])];
        }
    }
}
