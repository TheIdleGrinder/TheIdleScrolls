using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Skills.SkillEffects
{
	public class DamageSkillEffect(double damage, HashSet<string> tags) : ISkillEffect
	{
		public double Damage = damage;
		public HashSet<string> Tags = tags;

		public string Description => $"{Damage} damage";

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
				tmpDamage = modComp.ApplyApplicableModifiers(tmpDamage, ["DamageTaken", ..Tags], target.GetTags());
			}

			hpComp.ApplyDamage((int)Math.Round(tmpDamage, 0));
		}
	}
}
