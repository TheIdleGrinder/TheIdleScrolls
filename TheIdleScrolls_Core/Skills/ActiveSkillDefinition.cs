using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Skills
{
	public class ActiveSkillDefinition
	{
		public enum TargetingMode { Self, SingleEnemy }

		public readonly string Id;
		public readonly string Name;
		public readonly TargetingMode TargetMode = TargetingMode.SingleEnemy;
		public Action<Entity, ActiveSkill> SetupFunction;


		public ActiveSkillDefinition(string id, string name, TargetingMode target, Action<Entity, ActiveSkill> setupForUser)
		{
			Id = id;
			Name = name;
			TargetMode = target;
			SetupFunction = setupForUser;
		}

		public void SetupForUser(Entity user, ActiveSkill concrete)
		{
			SetupFunction(user, concrete);
		}

	}
}
