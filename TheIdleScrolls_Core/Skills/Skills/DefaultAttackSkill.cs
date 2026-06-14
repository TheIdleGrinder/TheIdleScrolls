using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static void SetupAttackComponent(Entity user, BattleStatsComponent statsComp, List<Modifier>? additionalMods = null)
        {
            List<string> AdditionalTags = [Tags.Attack, Skill.Id];

            var equipComp = user.GetComponent<EquipmentComponent>();

            double cooldown = 1.0;
            int weaponCount = 0;

            var globalTags = user.GetTags();
            statsComp.ResetAttacks();

            List<Modifier> modifiers = user.GetComponent<ModifierComponent>()?.GetModifiers().ToList() ?? [];
            if (additionalMods != null)
                modifiers.AddRange(additionalMods);

            if (equipComp != null)
            {
                foreach (var item in equipComp.GetItems())
                {
                    var itemComp = item.GetComponent<ItemComponent>();
                    var weaponComp = item.GetComponent<WeaponComponent>();
                    var localTags = item.GetTags().Concat(AdditionalTags).ToList();

                    // Add situational local tags 
                    var slots = item.GetRequiredSlots();
                    if (slots.Count == 1 && slots[0] == EquipmentSlot.Hand)
                    {
                        localTags.Add(item.IsShield() || weaponCount > 0 ? Tags.OffHand : Tags.MainHand);
                    }

                    if (itemComp != null && weaponComp != null)
                    {
                        DamageCluster localDmg = weaponComp.Damage;
                        double localCD = weaponComp.AttackTime;
                        double localRange = weaponComp.Range;
                        weaponCount++;
                        
                        localDmg = weaponComp.Damage.ScaleWithModifiers(
                            modifiers,
                            localTags, globalTags);
                        // Ranged attacks have their damage reduced by encumbrance
                        if (localTags.Contains(Tags.Ranged))
                        {
                            localDmg = localDmg.Multiply(1.0 / statsComp.EncumbranceSlowdown);
                        }
                        localCD = 1.0 / modifiers.ApplyAllApplicable(1.0 / localCD,
                            localTags.Append(Tags.AttackSpeed),  // invert due to speed/cooldown mismatch
                            globalTags);
                        localRange = modifiers.ApplyAllApplicable(weaponComp.Range, [.. localTags, Tags.Range], globalTags);

                        statsComp.AddAttackVector(localDmg, localCD, localRange);
                    }
                }
            }

            if (weaponCount == 0)
            {
                // use base attack from BattleStatsComponent if no weapons equipped, modified by unarmed and generic attack modifiers
                DamageCluster damage = new(statsComp.BaseAttack.RawDamage);
                damage = damage.ScaleWithModifiers(
                    modifiers,
                    [Abilities.Unarmed, Tags.Melee, .. AdditionalTags],
                    globalTags);
                // invert attack speed due to speed/cooldown mismatch
                cooldown = 1.0 / modifiers.ApplyAllApplicable(1.0 / statsComp.BaseAttack.AttackTime,
                    [Tags.AttackSpeed, Tags.Melee, Abilities.Unarmed, .. AdditionalTags], globalTags);
                statsComp.AddAttackVector(damage, cooldown, statsComp.BaseAttack.Range);
            }

            foreach (var vector in statsComp.AttackVectors)
            {
                vector.AttackTime *= statsComp.EncumbranceSlowdown;
                vector.AttackTime = Math.Max(vector.AttackTime, 1.0 / Stats.MaxAttacksPerSecond); // Cap attack speed
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
                    ? new DoTSkillEffect(type, damage, duration, [Tags.DamageOverTime, .. tags]) { StackLimit = stackLimit }
                    : new DamageSkillEffect(type, damage, [Tags.Damage, .. tags]);

                effects.Add(effect);
            }
            return effects;
        }

        protected override void SetupStats(Entity user, ActiveSkill skill)
        {
            var attackComp = user.GetComponent<BattleStatsComponent>();
            if (attackComp == null) // should never happen
            {
                Debug.WriteLine($"BattleStatsComponent is missing for entity '{user.GetName()}'");
                return;
            }

            List<string> AdditionalTags = [Tags.Attack, Skill.Id];
            SkillEffectBundle damage = new(CreateDefaultSkillEffectsForDamage(attackComp.AverageDamage, [.. AdditionalTags]),
                                            TargetingMode.SingleEnemy);
            damage.Accuracy = user.GetComponent<AccuracyComponent>()?.Accuracy;

            skill.Range = attackComp.AverageRange;
            skill.ActivityStartEffects = [damage];
            skill.ChargingTime = attackComp.AverageCooldown;
        }

        public override bool IsAvailableTo(Entity user)
        {
            return user.HasComponent<BattleStatsComponent>();
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            bool available = user.IsInBattle();
            if (!available)
                return (UsePrevention.NotInBattle, "Only usable in battle");
            if (ActiveSkill.GetEnemiesInRange(user, user.GetComponent<BattleStatsComponent>()?.AverageRange ?? 0.0).Count == 0)
                return (UsePrevention.NoTargetInRange, "No target in range");
            return (UsePrevention.None, "");
        }
    }
}
