using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Skills
{
	public enum UsePrevention
	{
		None,
		NotInBattle,
		NotResting,
		NoTargetPresent,
		NoTargetInRange,
		MissingPerk,
		WrongEquipment
    }

    public abstract class ActiveSkillDefinition
	{
		public abstract string Id { get; }
		public abstract string Name { get; }
		protected abstract void SetupStats(Entity user, ActiveSkill concrete);
		public abstract (UsePrevention prevention, string details) IsUsableBy(Entity user);
		public abstract bool IsAvailableTo(Entity user);

		public void SetupForUser(Entity user, ActiveSkill concrete)
		{
			SetupStats(user, concrete);
		}

		protected static bool HasPerkActive(Entity user, string perkId)
		{
            var perksComp = user.GetComponent<PerksComponent>();
            if (perksComp is null)
                return false;

            return perksComp.IsPerkActive(perkId);
        }
    }
}
