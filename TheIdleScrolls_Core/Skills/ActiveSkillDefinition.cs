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
		public readonly string Id;
		public readonly string Name;
		public Action<Entity, ActiveSkill> SetupFunction;


		public ActiveSkillDefinition(string id, string name, Action<Entity, ActiveSkill> setupForUser)
		{
			Id = id;
			Name = name;
			SetupFunction = setupForUser;
		}

		public void SetupForUser(Entity user, ActiveSkill concrete)
		{
			SetupFunction(user, concrete);
		}

	}
}
