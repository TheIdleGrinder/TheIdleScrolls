using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Skills.SkillEffects;
using TheIdleScrolls_Core.Utility;

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
                        DamageCluster localDmg = weaponComp.Damage;
                        double localCD = weaponComp.Cooldown;
                        weaponCount++;

                        if (modComp != null)
                        {
                            localDmg = weaponComp.Damage.ScaleWithModifiers(
                                modComp.GetModifiers(),
                                localTags, globalTags);
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
                DamageCluster damage = new();
                damage.AddDamage(DamageType.Physical, 2.0); // Base unarmed damage
                damage = damage.ScaleWithModifiers(
                    modComp?.GetModifiers() ?? [],
                    [Abilities.Unarmed, .. AdditionalTags],
                    globalTags);
                // invert attack speed due to speed/cooldown mismatch
                cooldown = 1.0 / modComp?.ApplyApplicableModifiers(1.0 / cooldown,
                    [Tags.AttackSpeed, Abilities.Unarmed, .. AdditionalTags], globalTags) ?? cooldown;
                attackComp.AddAttackVector(damage, cooldown);
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
