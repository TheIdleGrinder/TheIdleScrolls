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
	public class DamageSkillEffect(double damage, TargetingMode target, HashSet<string> tags) : ISkillEffect
	{
		public double Damage = damage;
		public HashSet<string> Tags = tags;

		public double DamageDone { get; private set; } = 0.0;

        public string Description => $"{Damage:0.##} damage";

		public TargetingMode Target => target;

		public void ApplyToTarget(Entity target)
		{
			var hpComp = target.GetComponent<LifePoolComponent>();
			if (hpComp is null)
            {
				return;
            }

			var modComp = target.GetComponent<ModifierComponent>();
			double tmpDamage = Damage;
			if (modComp is not null)
			{
				tmpDamage = modComp.ApplyApplicableModifiers(tmpDamage, [Definitions.Tags.DamageTaken, ..Tags], target.GetTags());
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
        }
	}
}
