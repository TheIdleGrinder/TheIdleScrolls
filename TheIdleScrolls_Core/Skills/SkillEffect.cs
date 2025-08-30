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
		public string Description { get; }

		public void ApplyToTarget(Entity target);
	}
}
