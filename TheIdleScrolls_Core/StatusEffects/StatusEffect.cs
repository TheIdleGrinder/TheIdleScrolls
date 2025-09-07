using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.StatusEffects
{
	public abstract class StatusEffect
	{
		Cooldown? Timer = null;
		protected Entity? Target = null;

		public string Name { get; init; }
		abstract protected void ActivateEffect(Entity target);
		abstract protected void DeactivateEffect(Entity target);

		public bool IsExpired => Timer?.HasFinished ?? false;

		public StatusEffect(string name, double duration = 0.0)
		{
			Name = name;
			if (duration > 0.0)
			{
				Timer = new(duration)
				{
					SingleShot = true
				};
			}
		}

		public virtual double ScaleDurationForTarget(Entity target, double duration) => duration;

		public void ActivateOnEntity(Entity target)
		{
			Target = target;
			if (Timer is not null)
			{
				double scaledDuration = ScaleDurationForTarget(Target, Timer.Duration);
				if (scaledDuration <= 0.0)
					return;
				Timer?.Reset(scaledDuration);
			}
			var effectComp = Target.GetComponent<StatusEffectComponent>();
			if (effectComp is null)
			{
				effectComp = new();
				Target.AddComponent(effectComp);
			}
			effectComp.Add(this);
			ActivateEffect(Target);
		}

		public void Deactivate()
		{
			if (Target is not null)
			{
				DeactivateEffect(Target);
				var effectComps = Target.GetComponent<StatusEffectComponent>();
				effectComps?.Remove(this);
				if (effectComps is not null && effectComps.StatusEffects.Count == 0)
				{
					Target.RemoveComponent<StatusEffectComponent>();
				}
			}
		}

		public void Update(double dt)
		{
			Timer?.Update(dt);
			if (IsExpired)
				Deactivate();
		}
	}
}
