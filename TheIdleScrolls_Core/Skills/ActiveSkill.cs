using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Skills
{
	public class ActiveSkill(ActiveSkillDefinition definition)
	{
		public enum State 
		{ 
			Unavailable,
			Disabled, 
			NotUsable, 
			Ready, 
			Charging, 
			CoolingDown 
		}

		readonly ActiveSkillDefinition Definition = definition;

		public readonly SkillTimer Timer = new(1.0, 0.0, 0.0);
		public List<ISkillEffect> Effects = [];
		public SkillTimer.State CurrentState => Timer.CurrentState;
		public double ChargingTime
		{
			get => Timer.ChargingDuration;
			set => Timer.ChargingDuration = value;
		}

		public string Id => Definition.Id;
		public string Name => Definition.Name;

		public void SetupForUser(Entity user)
		{
			Definition.SetupForUser(user, this);
		}

		/// <summary>
		/// Updates the internal timer of the skill. Returns the time that remained after fully charging.
		/// </summary>
		public double UpdateTimer(double dt)
		{
			return Timer.Update(dt);
		}
	}
}
