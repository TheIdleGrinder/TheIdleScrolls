using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Skills
{
	public interface ISkillEffect
	{
		public enum TargetingMode { Self, SingleEnemy }

		public string Description { get; }

		public TargetingMode Target { get; }

		public void ApplyToTarget(Entity target);
	}
}
