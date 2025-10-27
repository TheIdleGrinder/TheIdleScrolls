using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
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

		public class EffectsForState
		{
			public List<ISkillEffect> OnEnter { get; set; } = [];
			public ISkillEffectGenerator? RepeatedWhileIn { get; set; } = null;
			public List<StatusEffect> WhileIn { get; set; } = [];
        }


		readonly ActiveSkillDefinition Definition = definition;
		Entity? User = null;

		public readonly SkillTimer Timer = new(1.0, 0.0, 0.0);

		public EffectsForState ChargingEffects { get; set; } = new();
        public EffectsForState ActiveEffects { get; set; } = new();
		public EffectsForState CooldownEffects { get; set; } = new();
  //      public List<ISkillEffect> ActivationEffects { get; set; } = [];
		//public StatusEffect? ActiveStatusEffect { get; set; } = null;
  //      public List<ISkillEffect> ActivityEndEffect { get; set; } = [];
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

		public List<ISkillEffect> StartCharging()
        {
            Timer.Start();
            if (User is not null)
				ChargingEffects.WhileIn.ForEach(se => se.ActivateOnEntity(User));
            return ChargingEffects.OnEnter;
        }

        /// <summary>
        /// Updates the internal timer of the skill. Returns the time that remained after fully charging.
        /// </summary>
        public (SkillTimer.TimerUpdateResult, List<ISkillEffect>) Update(double dt)
		{
			var timerResult = Timer.Update(dt);
			List<ISkillEffect> effects = [];

			StatusEffectComponent? statComp = User?.GetComponent<StatusEffectComponent>();

            // Handle effects that are repeated while charging
            if (timerResult.SpentCharging > 0.0 && ChargingEffects.RepeatedWhileIn is not null)
			{
				effects.AddRange(ChargingEffects.RepeatedWhileIn.Update(timerResult.SpentCharging));
            }
            // Handle effects from going from charging to active
            if (timerResult.ChargingComplete)
			{
				ChargingEffects.RepeatedWhileIn?.Reset();
				ChargingEffects.WhileIn.ForEach(se => se.Deactivate());
				effects.AddRange(ActiveEffects.OnEnter);
				if (User is not null)
					ActiveEffects.WhileIn.ForEach(se => se.ActivateOnEntity(User));
            }
            // Handle effects that are repeated while active
            if (timerResult.SpentActive > 0.0 && ActiveEffects.RepeatedWhileIn is not null)
            {
                effects.AddRange(ActiveEffects.RepeatedWhileIn.Update(timerResult.SpentActive));
            }
            // Handle effects from going from active to cooldown
            if (timerResult.ActivityComplete)
            {
                ActiveEffects.RepeatedWhileIn?.Reset();
                ActiveEffects.WhileIn.ForEach(se => se.Deactivate());
                effects.AddRange(CooldownEffects.OnEnter);
                if (User is not null)
                    CooldownEffects.WhileIn.ForEach(se => se.ActivateOnEntity(User));
            }
            // Handle effects that are repeated while cooling down
            if (timerResult.SpentCooldown > 0.0 && CooldownEffects.RepeatedWhileIn is not null)
            {
                effects.AddRange(CooldownEffects.RepeatedWhileIn.Update(timerResult.SpentCooldown));
            }
            // Handle effects from going from cooldown to ready
            if (timerResult.CooldownComplete)
            {
                CooldownEffects.RepeatedWhileIn?.Reset();
                CooldownEffects.WhileIn.ForEach(se => se.Deactivate());
            }


            return (timerResult, effects);
		}
	}
}
