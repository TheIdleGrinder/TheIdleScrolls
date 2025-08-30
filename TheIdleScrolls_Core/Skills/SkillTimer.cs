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
		public double Update(double dt)
		{
			if (_CurrentState == State.NotStarted || dt <= 0.0)
				return 0.0;
			double remaining = 0.0;
			_Remaining -= dt;
			if (_Remaining < 0 && _CurrentState == State.Charging)
			{
				remaining = -_Remaining;
				_CurrentState = State.Active;
				_Remaining += _Active;
			}
			if (_Remaining < 0 && _CurrentState == State.Active)
			{
				_CurrentState = State.Cooldown;
				_Remaining += _Cooldown;
			}
			if (_Remaining < 0 && _CurrentState == State.Cooldown)
			{
				// Reset the timer to initial state, the cycle has finished
				_CurrentState = State.NotStarted;
				_Remaining = _Charge;
			}
			return remaining;
		}
	}
}
