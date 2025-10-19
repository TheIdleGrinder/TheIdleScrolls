using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.StatusEffects;
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
			Active,
			CoolingDown
		}

		readonly ActiveSkillDefinition Definition = definition;
		Entity? User = null;

		public readonly SkillTimer Timer = new(1.0, 0.0, 0.0);
		public List<ISkillEffect> ActivationEffects { get; set; } = [];
		public StatusEffect? ActiveStatusEffect { get; set; } = null;
        public List<ISkillEffect> ActivityEndEffect { get; set; } = [];
		public SkillTimer.State CurrentState => Timer.CurrentState;
		public double ChargingTime
		{
			get => Timer.ChargingDuration;
			set => Timer.ChargingDuration = value;
		}
		public bool Enabled = true;

		public string Id => Definition.Id;
		public string Name => Definition.Name;
		public List<string> Tags { get; set; } = [];

		public void SetupForUser(Entity user)
		{
			User = user;
			Definition.SetupForUser(user, this);
		}

		public bool IsInUse()
		{
			var state = GetState();
			return state != State.Unavailable && state != State.Disabled && state != State.NotUsable;
		}

		public State GetState()
		{
			if (User is null || !Definition.IsAvailableTo(User))
				return State.Unavailable;

			if (!Enabled)
				return State.Disabled;
			var (usable, _) = Definition.IsUsableBy(User);
			if (!usable)
				return State.NotUsable;

			return CurrentState switch
			{
				SkillTimer.State.NotStarted => State.Ready,
				SkillTimer.State.Charging => State.Charging,
				SkillTimer.State.Active => State.Active,
				SkillTimer.State.Cooldown => State.CoolingDown,
				_ => State.Unavailable,
			};
		}

		/// <summary>
		/// Updates the internal timer of the skill. Returns the time that remained after fully charging.
		/// </summary>
		public SkillTimer.TimerUpdateResult UpdateTimer(double dt)
		{
			return Timer.Update(dt);
		}
	}
}
