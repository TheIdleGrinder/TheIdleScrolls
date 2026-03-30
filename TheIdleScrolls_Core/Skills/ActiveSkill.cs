using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Modifiers;
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

        public int UseCount { get; private set; } = 0;

        public readonly SkillTimer Timer = new(1.0, 0.0, 0.0);

        EffectsForState ChargingEffects { get; set; } = new();
        EffectsForState ActiveEffects { get; set; } = new();
		EffectsForState CooldownEffects { get; set; } = new();
		public SkillTimer.State CurrentState => Timer.CurrentState;

		public string NotUsableReason
		{
			get
			{
				if (User is null || !Definition.IsAvailableTo(User))
					return "";
				var (usable, reason) = Definition.IsUsableBy(User);
				if (!usable)
					return reason;
				return "";
			}
        }

        public List<ISkillEffect> ChargingStartEffects
		{
			get => ChargingEffects.OnEnter;
			set
			{
				if (CurrentState != SkillTimer.State.Charging)
					ChargingEffects.OnEnter = value;
            }
        }
		public ISkillEffectGenerator? ChargingRepeatedEffect
		{
			get => ChargingEffects.RepeatedWhileIn;
			set
			{
				if (CurrentState != SkillTimer.State.Charging)
					ChargingEffects.RepeatedWhileIn = value;
			}
        }
		public List<StatusEffect> ChargingWhileInEffects
		{
			get => ChargingEffects.WhileIn;
			set
			{
				if (CurrentState != SkillTimer.State.Charging)
					ChargingEffects.WhileIn = value;
            }
        }
        public List<ISkillEffect> ActivityStartEffects
        {
            get => ActiveEffects.OnEnter;
            set
            {
                if (CurrentState != SkillTimer.State.Active)
                    ActiveEffects.OnEnter = value;
            }
        }
        public ISkillEffectGenerator? ActivityRepeatedEffect
        {
            get => ActiveEffects.RepeatedWhileIn;
            set
            {
                if (CurrentState != SkillTimer.State.Active)
                    ActiveEffects.RepeatedWhileIn = value;
            }
        }
        public List<StatusEffect> ActivityWhileInEffects
        {
            get => ActiveEffects.WhileIn;
            set
            {
                if (CurrentState != SkillTimer.State.Active)
                    ActiveEffects.WhileIn = value;
            }
        }
        public List<ISkillEffect> CooldownStartEffects
        {
            get => CooldownEffects.OnEnter;
            set
            {
                if (CurrentState != SkillTimer.State.Cooldown)
                    CooldownEffects.OnEnter = value;
            }
        }
        public ISkillEffectGenerator? CooldownRepeatedEffect
        {
            get => CooldownEffects.RepeatedWhileIn;
            set
            {
                if (CurrentState != SkillTimer.State.Cooldown)
                    CooldownEffects.RepeatedWhileIn = value;
            }
        }
        public List<StatusEffect> CooldownWhileInEffects
        {
            get => CooldownEffects.WhileIn;
            set
            {
                if (CurrentState != SkillTimer.State.Cooldown)
                    CooldownEffects.WhileIn = value;
            }
        }


        public double ChargingTime
		{
			get => Timer.ChargingDuration;
			set => Timer.ChargingDuration = value;
		}
		public bool Enabled = true;

		public string Id => Definition.Id;
		public string Name => Definition.Name;
		public HashSet<string> Tags { get; set; } = [];

		public void SetupForUser(Entity user)
		{
			User = user;
			Definition.SetupForUser(user, this);
		}

		public void Reset()
		{
			Timer.Reset();
			UseCount = 0;
        }

		public bool IsInUse()
		{
			return !HasState([State.Unavailable, State.Disabled, State.NotUsable]);
		}

		public bool HasState(HashSet<State> states)
		{
			return states.Contains(GetState());
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
				UseCount++;
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

		// Utility functions, might move somehwere else later
		public double ScaleValue(double baseValue, List<string> situationalTags, List<Modifier>? exclusiveMods = null)
		{
			if (exclusiveMods is null || exclusiveMods.Count == 0)
			{
				return User!.ApplyAllApplicableModifiers(baseValue, [Id, .. Tags, .. situationalTags], User!.GetTags());
			}
			else
			{
				List<Modifier> mods = User!.GetComponent<ModifierComponent>()?.GetModifiers()?.Concat(exclusiveMods)?.ToList() 
										?? exclusiveMods;
				return mods.ApplyAllApplicable(baseValue, [Id, .. Tags, .. situationalTags], User!.GetTags());
			}
        }

		public DamageCluster ScaleDamage(DamageCluster baseDamage, HashSet<string> situationalTags, List<Modifier>? exclusiveMods = null)
		{
			List<Modifier> mods = User?.GetComponent<ModifierComponent>()?.GetModifiers() ?? [];
			if (exclusiveMods is not null && exclusiveMods.Count > 0)
			{
				mods.AddRange(exclusiveMods);
			}
            return baseDamage.ScaleWithModifiers(mods, [Id, .. Tags, .. situationalTags], User!.GetTags());
        }

        public Perk? GetPerk(string perkId)
        {
            return User?.GetComponent<PerksComponent>()?.GetPerk(perkId);
        }

        public int GetPerkLevel(string perkId)
		{
			return User?.GetComponent<PerksComponent>()?.GetPerkLevel(perkId) ?? 0;
        }
    }
}
