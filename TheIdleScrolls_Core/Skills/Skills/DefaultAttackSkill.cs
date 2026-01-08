using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Skills.SkillEffects;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class DefaultAttack : ActiveSkillDefinition
    {
        public static DefaultAttack Skill { get; } = new();

        private DefaultAttack() { }

        public override string Id => "DfltAttack";

        public override string Name => "Default Attack";

        public static void SetupPlayerAttackComponent(Entity user)
        {
            var attackComp = user.GetComponent<AttackComponent>();
            if (attackComp == null)
                return;

            List<string> AdditionalTags = [Tags.Attack, Skill.Id];

            var equipComp = user.GetComponent<EquipmentComponent>();
            var modComp = user.GetComponent<ModifierComponent>();

            double rawDamage = 2.0;
            double cooldown = 1.0;
            int weaponCount = 0;
            double encumbrance = 0.0;

            var globalTags = user.GetTags();
            attackComp.Reset();

            if (equipComp != null)
            {
                foreach (var item in equipComp.GetItems())
                {
                    var itemComp = item.GetComponent<ItemComponent>();
                    var weaponComp = item.GetComponent<WeaponComponent>();
                    var localTags = item.GetTags().Concat(AdditionalTags).ToList();
                    encumbrance += item.GetComponent<EquippableComponent>()?.Encumbrance ?? 0.0;

                    // Add situational local tags 
                    var slots = item.GetRequiredSlots();
                    if (slots.Count == 1 && slots[0] == EquipmentSlot.Hand)
                    {
                        localTags.Add(item.IsShield() || weaponCount > 0 ? Tags.OffHand : Tags.MainHand);
                    }

                    if (itemComp != null && weaponComp != null)
                    {
                        double localDmg = weaponComp.Damage;
                        double localCD = weaponComp.Cooldown;
                        weaponCount++;

                        if (modComp != null)
                        {
                            localDmg = modComp.ApplyApplicableModifiers(localDmg, localTags.Append(Tags.Damage), globalTags);
                            localCD = 1.0 / modComp.ApplyApplicableModifiers(1.0 / localCD,
                                localTags.Append(Tags.AttackSpeed),  // invert due to speed/cooldown mismatch
                                globalTags);
                        }

                        attackComp.AddAttackVector(localDmg, localCD);
                    }
                }
            }

            if (weaponCount == 0)
            {
                rawDamage = modComp?.ApplyApplicableModifiers(rawDamage,
                    [Tags.Damage, Abilities.Unarmed, .. AdditionalTags], globalTags) ?? rawDamage;
                // invert attack speed due to speed/cooldown mismatch
                cooldown = 1.0 / modComp?.ApplyApplicableModifiers(1.0 / cooldown,
                    [Tags.AttackSpeed, Abilities.Unarmed, .. AdditionalTags], globalTags) ?? cooldown;
                attackComp.AddAttackVector(rawDamage, cooldown);
            }

            double encumbranceSlowdown = 1.0 + Math.Max(encumbrance, 0.0) / 100.0;
            foreach (var vector in attackComp.AttackVectors)
            {
                vector.Cooldown *= encumbranceSlowdown;
                vector.Cooldown = Math.Max(vector.Cooldown, 1.0 / Stats.MaxAttacksPerSecond); // Cap attack speed
            }
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            var attackComp = user.GetComponent<AttackComponent>();
            if (attackComp == null)
            {
                attackComp = new();
                user.AddComponent(attackComp);
                SetupPlayerAttackComponent(user);
            }

            List<string> AdditionalTags = [Tags.Attack, Skill.Id];
            DamageSkillEffect dmgEffect = new(Math.Round(attackComp.AverageDamage), ISkillEffect.TargetingMode.SingleEnemy, [.. AdditionalTags]);
            skill.ActiveEffects.OnEnter = [dmgEffect];
            skill.ChargingTime = attackComp.AverageCooldown;
        }

        public override bool IsAvailableTo(Entity user)
        {
            return user.IsPlayer();
        }

        public override (bool available, string reason) IsUsableBy(Entity user)
        {
            return (IsAvailableTo(user), string.Empty);
        }
    }
}
