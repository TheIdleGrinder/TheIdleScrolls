using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Skills
{
	public class SkillTimer(double chargeDuration, double activeDuration, double cooldownDuration)
	{
		public enum State { NotStarted, Charging, Active, Cooldown }

        public class TimerUpdateResult
		{
			public double RemainingTime { get; set; } = 0.0;
            public bool ChargingComplete { get; set; } = false;
            public bool ActivityComplete { get; set; } = false;
            public bool CooldownComplete { get; set; } = false;
            public double SpentCharging { get; set; } = 0.0;
            public double SpentActive { get; set; } = 0.0;
            public double SpentCooldown { get; set; } = 0.0;
        };

        State _CurrentState = State.NotStarted;
		double _Charge = chargeDuration;
		double _Active = activeDuration;
		double _Cooldown = cooldownDuration;
		double _Remaining = chargeDuration;

		public State CurrentState { get { return _CurrentState; } }

		public double ChargingDuration 
		{ 
			get { return _Charge; } 
			set 
			{ 
				if ((_CurrentState == State.NotStarted || _CurrentState == State.Charging) && _Charge != 0.0)
				{
					_Remaining *= value / _Charge;
				}
				_Charge = value; 
			}
		}

		public double ActiveDuration 
		{ 
			get { return _Active; }
			set
			{
				if (_CurrentState == State.Active && _Active != 0.0)
				{
					_Remaining *= value / _Active;
				}
				_Active = value;
			}
		}

		public double CooldownDuration
		{
			get { return _Cooldown; }
			set
			{
				if (_CurrentState == State.Cooldown && _Cooldown != 0.0)
				{
					_Remaining *= value / _Cooldown;
				}
				_Cooldown = value;
			}
		}

		public double Remaining 
		{ 
			get { return _Remaining; }
		}

		public void Start()
		{
			if (_CurrentState == State.NotStarted)
				_CurrentState = State.Charging;
		}

		public void Stop()
		{
            if (_CurrentState == State.Charging)
            {
				Reset();
            }
        }

		public void Reset()
		{
			_CurrentState = State.NotStarted;
			_Remaining = _Charge;
		}

		/// <summary>
		/// Moves the state of the timer ahead by dt seconds. Returns 0.0 unless the skill finished charging
		/// </summary>
		/// <param name="dt"></param>
		/// <returns>Remainder of dt after charging completes</returns>
		public TimerUpdateResult Update(double dt)
		{
            TimerUpdateResult result = new();
            if (_CurrentState == State.NotStarted || dt <= 0.0)
				return result;
			double leftToSpend = dt;

			if (_CurrentState == State.Charging)
			{
				result.SpentCharging = Math.Min(_Remaining, leftToSpend);
                leftToSpend -= result.SpentCharging;
                if (leftToSpend > 0.0)
				{
					result.RemainingTime = leftToSpend;
                    result.ChargingComplete = true;
                    _CurrentState = State.Active;
                    _Remaining = _Active;
                }
				else
				{
                    _Remaining -= result.SpentCharging;
                }
			}
            if (_CurrentState == State.Active && leftToSpend > 0.0)
            {
                result.SpentActive = Math.Min(_Remaining, leftToSpend);
                leftToSpend -= result.SpentActive;
                if (leftToSpend > 0.0)
                {
                    result.ActivityComplete = true;
                    _CurrentState = State.Cooldown;
                    _Remaining = _Cooldown;
                }
                else
                {
                    _Remaining -= result.SpentActive;
                }
            }
            if (_CurrentState == State.Cooldown && leftToSpend > 0.0)
            {
                result.SpentCooldown = Math.Min(_Remaining, leftToSpend);
                leftToSpend -= result.SpentCooldown;
                if (leftToSpend > 0.0)
                {
                    result.CooldownComplete = true;
                    // Reset the timer to initial state, the cycle has finished
                    _CurrentState = State.NotStarted;
                    _Remaining = _Charge;
                }
                else
                {
                    _Remaining -= result.SpentCooldown;
                }
            }
			return result;
		}
	}
}
