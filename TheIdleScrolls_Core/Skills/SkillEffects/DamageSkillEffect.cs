using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using static TheIdleScrolls_Core.Skills.ISkillEffect;
using TheIdleScrolls_Core.Definitions;

namespace TheIdleScrolls_Core.Skills.SkillEffects
{
	public class DamageSkillEffect(DamageType damageType, double damage, HashSet<string> tags) : ISkillEffect
	{
		public DamageType DamageType = damageType;
        public double Damage = Functions.ApplyDamageRounding(damage);
		public HashSet<string> Tags = tags;

		public double DamageDone { get; private set; } = 0.0;

        public string Description => $"{Damage:0.##} {DamageType.ToTag()} damage";

		public List<ISkillEffectOutcome> ApplyToTarget(Entity target)
		{
			var hpComp = target.GetComponent<LifePoolComponent>();
			if (hpComp is null)
            {
				return [];
            }

            double tmpDamage = Damage;

            // Consider armor for physical damage
            if (DamageType == DamageType.Physical)
			{
				double armor = target.GetComponent<BattleStatsComponent>()?.Armor ?? 0.0;
                double multi = Math.Max(tmpDamage / (tmpDamage + armor), 1.0 - Stats.MaxResistanceFromArmor);
                if (armor > 0.0)
                    tmpDamage *= multi;
            }

            var modComp = target.GetComponent<ModifierComponent>();
			if (modComp is not null)
			{
				var tags = Tags.ToHashSet();
				tags.UnionWith(DamageType.GetMatchingTags());
				tags.Remove(Definitions.Tags.Damage);
                tmpDamage = modComp.ApplyApplicableModifiers(tmpDamage, 
					[Definitions.Tags.DamageTaken, ..tags], target.GetTags());
                double resistance = modComp.ApplyApplicableModifiers(0.0,
					[Definitions.Tags.Resistance, .. DamageType.GetMatchingTags(), .. Tags], target.GetTags());
                resistance = Math.Min(resistance, Stats.MaxResistances);
                tmpDamage *= (1.0 - resistance / 100.0);
            }

            // Apply damage reduction
            double damageReduction = target.ApplyAllApplicableModifiers(0.0, [Definitions.Tags.DamageReduction], target.GetTags());
			tmpDamage = Math.Max(0.0, tmpDamage - damageReduction);

            // Apply damage ceiling from defense layers
            double defLayers = target.ApplyAllApplicableModifiers(0.0, [Definitions.Tags.DefensiveLayers], target.GetTags());
			double maxDamage = hpComp.Maximum / (defLayers + 1.0);
            tmpDamage = Math.Min(tmpDamage, maxDamage);

            hpComp.ApplyDamage(tmpDamage);
            DamageDone = tmpDamage;

            List<ISkillEffectOutcome> outcomes = [];
            if (tmpDamage > 0.0)
            {
                outcomes.Add(new DamageApplied(target, DamageType, tmpDamage));
            }
            if (tmpDamage < Damage)
            {
                outcomes.Add(new DamagePrevented(target, DamageType, Damage - tmpDamage));
            }
            return outcomes;
        }
	}
}
