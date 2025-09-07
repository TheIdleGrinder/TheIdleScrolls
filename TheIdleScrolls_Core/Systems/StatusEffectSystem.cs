using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.StatusEffects;

namespace TheIdleScrolls_Core.Systems
{
	internal class StatusEffectSystem : AbstractSystem
	{
		public override void Update(World world, Coordinator coordinator, double dt)
		{
			var entities = coordinator.GetEntities<StatusEffectComponent>();
			foreach (var entity in entities)
			{
				var effects = entity.GetComponent<StatusEffectComponent>()?.StatusEffects ?? [];
				foreach (var effect in effects)
				{
					effect.Update(dt);
					if (effect.IsExpired)
					{
						coordinator.PostMessage(this, new StatusEffectExpiredMessage(entity, effect));
					}
				}
			}
		}
	}

	public record StatusEffectExpiredMessage(Entity target, StatusEffect effect) : IMessage
	{
		string IMessage.BuildMessage() => $"'{effect.Name}' expired on {target.GetName()}";

		IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Low;
	}
}
