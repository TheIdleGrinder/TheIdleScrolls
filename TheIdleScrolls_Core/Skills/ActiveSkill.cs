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
	public abstract class ActiveSkill()
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
			public List<SkillEffectBundle> OnEnter { get; set; } = [];
			public ISkillEffectGenerator? RepeatedWhileIn { get; set; } = null;
			public List<StatusEffect> WhileIn { get; set; } = [];
        }

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

        Entity? User = null;

        public int UseCount { get; private set; } = 0;

        public readonly SkillTimer Timer = new(1.0, 0.0, 0.0);

        EffectsForState ChargingEffects { get; set; } = new();
        EffectsForState ActiveEffects { get; set; } = new();
		EffectsForState CooldownEffects { get; set; } = new();
		public SkillTimer.State CurrentState => Timer.CurrentState;

        public abstract string Id { get; }
        public abstract string Name { get; }
        protected abstract void SetupStats(Entity user);
        public abstract (UsePrevention prevention, string details) IsUsableBy(Entity user);
        public abstract bool IsAvailableTo(Entity user);

        public HashSet<string> SkillTags { get; set; } = [];
        public UsePrevention Prevention { get; private set; } = UsePrevention.None;
        public double Range { get; set; } = 0.0;

        public void SetupForUser(Entity user)
        {
            User = user;
            SetupStats(user);
        }

        protected static bool HasPerkActive(Entity user, string perkId)
        {
            var perksComp = user.GetComponent<PerksComponent>();
            if (perksComp is null)
                return false;

            return perksComp.IsPerkActive(perkId);
        }

        public string NotUsableReason
		{
			get
			{
				if (User is null || !IsAvailableTo(User))
					return "";
				var (usable, reason) = IsUsableBy(User);
				if (usable != UsePrevention.None)
					return reason;
				return "";
			}
        }

        public List<SkillEffectBundle> ChargingStartEffects
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
        public List<SkillEffectBundle> ActivityStartEffects
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
        public List<SkillEffectBundle> CooldownStartEffects
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

		public void Reset()
		{
			Timer.Reset();
			UseCount = 0;
        }

		public bool IsInUse()
		{
			return !HasState([State.Unavailable, State.Disabled]);
		}

		public bool HasState(HashSet<State> states)
		{
			return states.Contains(GetState());
        }

		public State GetState()
		{
			if (User is null || !IsAvailableTo(User))
				return State.Unavailable;

			if (!Enabled)
				return State.Disabled;
			var (prevention, _) = IsUsableBy(User);
			Prevention = prevention;
			if (prevention != UsePrevention.None)
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

		public List<SkillEffectBundle> StartCharging()
        {
            Timer.Start();
            if (User is not null)
				ChargingEffects.WhileIn.ForEach(se => se.ActivateOnEntity(User));
            return ChargingEffects.OnEnter;
        }

        /// <summary>
        /// Updates the internal timer of the skill. Returns the time that remained after fully charging.
        /// </summary>
        public (SkillTimer.TimerUpdateResult, List<SkillEffectBundle>) Update(double dt)
		{
			var timerResult = Timer.Update(dt);
			List<SkillEffectBundle> effects = [];

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
				return User!.ApplyAllApplicableModifiers(baseValue, [Id, .. SkillTags, .. situationalTags], User!.GetTags());
			}
			else
			{
				List<Modifier> mods = User!.GetComponent<ModifierComponent>()?.GetModifiers()?.Concat(exclusiveMods)?.ToList() 
										?? exclusiveMods;
				return mods.ApplyAllApplicable(baseValue, [Id, .. SkillTags, .. situationalTags], User!.GetTags());
			}
        }

		public DamageCluster ScaleDamage(DamageCluster baseDamage, HashSet<string> situationalTags, List<Modifier>? exclusiveMods = null)
		{
			List<Modifier> mods = User?.GetComponent<ModifierComponent>()?.GetModifiers() ?? [];
			if (exclusiveMods is not null && exclusiveMods.Count > 0)
			{
				mods.AddRange(exclusiveMods);
			}
            return baseDamage.ScaleWithModifiers(mods, [Id, .. SkillTags, .. situationalTags], User!.GetTags());
        }

        public Perk? GetPerk(string perkId)
        {
            return User?.GetComponent<PerksComponent>()?.GetPerk(perkId);
        }

        public int GetPerkLevel(string perkId)
		{
			return User?.GetComponent<PerksComponent>()?.GetPerkLevel(perkId) ?? 0;
        }

		public static List<Entity> GetEnemiesInRange(Entity user, double range)
		{
			var battleComp = user.GetComponent<BattlerComponent>();
			if (battleComp is null || battleComp.Battle is null)
				return [];
			if (user.IsPlayer() && battleComp.Battle.Mobs.Count == 0)
				return [];
            List<Entity> enemies = user.IsPlayer() ? battleComp.Battle.Mobs : [battleComp.Battle.Player];
			return enemies.Where(e => e.GetComponent<BattlerComponent>()?.Position.DistanceTo(battleComp.Position) <= range).ToList();
        }
    }
}
