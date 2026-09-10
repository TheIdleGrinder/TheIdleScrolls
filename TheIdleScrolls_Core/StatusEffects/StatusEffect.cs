using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Skills;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.StatusEffects
{
	public abstract class StatusEffect
	{
		public Cooldown? Timer = null;
		protected Entity? Target = null;

		public string Name { get; init; }
        abstract public string Description { get; }
        abstract protected void ActivateEffect(Entity target);
		abstract protected void DeactivateEffect(Entity target);
		virtual protected void UpdateEffect(Entity target, double dt) { } 


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

		public List<ISkillEffectOutcome> ActivateOnEntity(Entity target)
		{
			Target = target;
			double originalDuration = Timer?.Duration ?? 0.0;
			double scaledDuration = originalDuration;
            if (Timer is not null)
			{
                scaledDuration = ScaleDurationForTarget(Target, Timer.Duration);
				if (scaledDuration <= 0.0)
					return [new StatusEffectPrevented(target, this, 1.0)];
				Timer?.Reset(scaledDuration);
			}
			double durationRatio = originalDuration > 0.0 ? scaledDuration / originalDuration : 1.0;
            var effectComp = Target.GetComponent<StatusEffectComponent>();
			if (effectComp is null)
			{
				effectComp = new();
				Target.AddComponent(effectComp);
			}
			effectComp.Add(this);
			ActivateEffect(Target);
            
			List<ISkillEffectOutcome> outcomes = [new StatusEffectApplied(target, this, durationRatio)];
			if (durationRatio < 1.0)
			{
				outcomes.Add(new StatusEffectPrevented(target, this, 1.0 - durationRatio));
            }
			return outcomes;
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
				Target = null;
			}
		}

		public void Update(double dt)
		{
			if (Target is not null)
			{
				UpdateEffect(Target, dt);
            }
            Timer?.Update(dt);
			if (IsExpired)
				Deactivate();
		}
	}
}
