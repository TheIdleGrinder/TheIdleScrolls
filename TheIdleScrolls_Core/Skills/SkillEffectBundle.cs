using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Skills
{
    public enum TargetingMode { Self, SingleEnemy }

    public class SkillEffectBundle
    {
        public TargetingMode Target { get; set; }
        public HashSet<string> BundleTags { get; set; } = [];
        public List<ISkillEffect> Effects { get; set; } = [];
        public double? Accuracy { get; set; } = null;

        public SkillEffectBundle() { }
        public SkillEffectBundle(List<ISkillEffect> effects, TargetingMode target, HashSet<string> tags)
        {
            Effects = effects;
            Target = target;
            BundleTags = tags;
        }

        public SkillEffectBundle(ISkillEffect effect, TargetingMode target, HashSet<string> tags)
        {
            Effects = [effect];
            Target = target;
            BundleTags = tags;
        }

        public string Description =>
            $"To {TargetToString()}:\n" +
            string.Join("\n", Effects.Select(e => "\t" + e.Description));

        public List<ISkillEffectOutcome> ApplyToTarget(Entity target)
        {
            // Handle accuracy
            if (Accuracy.HasValue)
            {
                double evasion = target.GetComponent<BattleStatsComponent>()?.Evasion ?? 0.0;
                double charge = Math.Min(Stats.MaxResistanceFromEvasion, evasion / (evasion + Accuracy.Value));
                var chanceComp = target.GetOrAddComponent<ChanceChargeComponent>();
                int chargeCount = chanceComp.GetFullChargeCount(Tags.Evasion);
                if (chargeCount > 0)
                {
                    chanceComp.RemoveCharge(Tags.Evasion, chargeCount);
                }
                chanceComp.AddCharge(Tags.Evasion, charge);
                if (chargeCount > 0)
                    return [new HitEvaded(target)]; // Attack was evaded, so we don't apply the effects
            }

            List<ISkillEffectOutcome> returnOutcomes = [];
            DamageCluster doneDamage = new();
            DamageCluster preventedDamage = new();

            // Handle blocking
            // Only skill effect bundles that deal damage have the Attack or Projectile tags can be blocked.
            // The tag Unblockable removes any chance of blocking
            double blockMitigation = 0.0;
            BlockerComponent blockComp = target.GetOrAddComponent<BlockerComponent>();
            if (blockComp.IsReady
                && (BundleTags.Contains(Tags.AttackSkill) || BundleTags.Contains(Tags.Projectile)) 
                && !BundleTags.Contains(Tags.Unblockable))
            {
                double blockChance = target.ApplyAllApplicableModifiers(0.0, [Tags.BlockChance, ..BundleTags], target.GetTags());
                int blocks = target.GetComponent<ChanceChargeComponent>()?.AddCharge(Tags.BlockChance, blockChance) ?? 0;
                if (blocks > 0)
                {
                    blockMitigation = 1.0 - Math.Pow(1.0 - blockComp.BlockMitigation, blocks);
                    target.GetComponent<ChanceChargeComponent>()?.RemoveCharge(Tags.BlockChance, blocks);
                    returnOutcomes.Add(new HitBlocked(target, blockMitigation));
                    blockComp.StartCooldown();
                }
            }
            Modifier? blockMulti = null;
            if (blockMitigation > 0.0)
            {
                blockMulti = new("tmpBlockMulti", ModifierType.More, -blockMitigation, [Tags.DamageTakenMultiplier], []);
                target.GetOrAddComponent<ModifierComponent>().AddModifier(blockMulti);
            }

            foreach (var effect in Effects)
            {
                var outcomes = effect.ApplyToTarget(target);
                foreach (var outcome in outcomes)
                {
                    if (outcome is DamageApplied damageOutcome)
                    {
                        doneDamage.AddDamage(damageOutcome.Type, damageOutcome.Amount);
                    }
                    else if (outcome is DamagePrevented preventOutcome)
                    {
                        preventedDamage.AddDamage(preventOutcome.Type, preventOutcome.Amount);
                    }
                    else
                    {
                        returnOutcomes.Add(outcome);
                    }
                }
            }
            if (doneDamage.TotalDamage > 0)
            {
                returnOutcomes.Add(new DamageClusterApplied(target, doneDamage));
            }
            if (preventedDamage.TotalDamage > 0)
            {
                returnOutcomes.Add(new DamageClusterPrevented(target, preventedDamage));
            }

            if (blockMulti != null)
            {
                target.GetComponent<ModifierComponent>()!.RemoveModifier(blockMulti.Id);
            }
            return returnOutcomes;
        }

        string TargetToString()
        {
            return Target switch
            {
                TargetingMode.Self => "self",
                TargetingMode.SingleEnemy => "single enemy",
                _ => "unknown target"
            };
        }
    }
}
