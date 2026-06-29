using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Skills.SkillEffects;

namespace TheIdleScrolls_Core.Skills
{
	public class GenericSkill(string id, string name, Action<Entity, ActiveSkill> updateFunction) : ActiveSkill
	{
        public override string Id => id;

        public override string Name => name;

		protected override void SetupStats(Entity user)
		{
			updateFunction(user, this);
		}

        public override bool IsAvailableTo(Entity user)
        {
            return true;
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            return (UsePrevention.None, string.Empty);
        }
    }
}
