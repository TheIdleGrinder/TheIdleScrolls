using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Skills.SkillEffects;
using TheIdleScrolls_Core.Utility;
using static TheIdleScrolls_Core.Skills.ISkillEffect;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public class DefaultAttack : ActiveSkillDefinition
    {
        public const string SkillId = "DfltAttack";

        public static DefaultAttack Skill { get; } = new();

        private DefaultAttack() { }

        public override string Id => SkillId;

        public override string Name => "Default Attack";

        public static void SetupAttackComponent(Entity user, AttackComponent attackComp, List<Modifier>? additionalMods = null)
        {
            if (attackComp == null)
                return;

            List<string> AdditionalTags = [Tags.Attack, Skill.Id];

            var equipComp = user.GetComponent<EquipmentComponent>();
            
            List<Modifier> modifiers = user.GetComponent<ModifierComponent>()?.GetModifiers().ToList() ?? [];
            if (additionalMods != null)
                modifiers.AddRange(additionalMods);

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

                        localDmg = weaponComp.Damage.ScaleWithModifiers(
                            modifiers,
                            localTags, globalTags);
                        localCD = 1.0 / modifiers.ApplyAllApplicable(1.0 / localCD,
                            localTags.Append(Tags.AttackSpeed),  // invert due to speed/cooldown mismatch
                            globalTags);

                        attackComp.AddAttackVector(localDmg, localCD);
                    }
                }
            }

            if (weaponCount == 0)
            {
                DamageCluster damage = new();
                damage.AddDamage(DamageType.Physical, Stats.UnarmedBaseDamage); // Base unarmed damage
                damage = damage.ScaleWithModifiers(
                    modifiers,
                    [Abilities.Unarmed, Tags.Melee, .. AdditionalTags],
                    globalTags);
                // invert attack speed due to speed/cooldown mismatch
                cooldown = 1.0 / modifiers.ApplyAllApplicable(1.0 / cooldown,
                    [Tags.AttackSpeed, Abilities.Unarmed, .. AdditionalTags], globalTags);
                attackComp.AddAttackVector(damage, cooldown);
            }

            double encumbranceSlowdown = 1.0 + Math.Max(encumbrance, 0.0) / 100.0;
            foreach (var vector in attackComp.AttackVectors)
            {
                vector.Cooldown *= encumbranceSlowdown;
                vector.Cooldown = Math.Max(vector.Cooldown, 1.0 / Stats.MaxAttacksPerSecond); // Cap attack speed
            }
        }

        public static List<ISkillEffect> CreateDefaultSkillEffectsForDamage(DamageCluster damages, HashSet<string> tags) 
        {
            List<ISkillEffect> effects = [];
            foreach (var type in damages.Types)
            {
                double damage = damages.DamageOfType(type);
                if (damage <= 0.0)
                    continue;

                double duration = type switch
                {
                    DamageType.Fire => 2.0,
                    DamageType.Poison => 5.0,
                    _ => 0.0
                };

                int stackLimit = type switch
                {
                    DamageType.Fire => 1,
                    _ => int.MaxValue
                };


                ISkillEffect effect = (duration > 0.0)
                    ? new DoTSkillEffect(type, damage, duration, TargetingMode.SingleEnemy, [Tags.DamageOverTime, .. tags]) { StackLimit = stackLimit }
                    : new DamageSkillEffect(type, damage, TargetingMode.SingleEnemy, [Tags.Damage, .. tags]);
                
                effects.Add(effect);
            }
            return effects;
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            skill.Tags = [Tags.AttackSkill];

            var attackComp = user.GetComponent<AttackComponent>();
            if (attackComp == null)
            {
                attackComp = new();
                user.AddComponent(attackComp);
                SetupAttackComponent(user, attackComp);
            }

            List<string> AdditionalTags = [Tags.Attack, Skill.Id];
            skill.ActivityStartEffects = CreateDefaultSkillEffectsForDamage(attackComp.AverageDamage, [.. AdditionalTags]);
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
