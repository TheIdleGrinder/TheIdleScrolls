using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
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

			// Handle life reg here until there is a more fitting system
			foreach (var entity in coordinator.GetEntities<LifePoolComponent>())
			{
				var lifeComp = entity.GetComponent<LifePoolComponent>()!;
				if (lifeComp.IsDead && entity.HasComponent<BattlerComponent>())
					continue;
				double lifeReg = entity.ApplyAllApplicableModifiers(0.0, [Tags.LifeRegeneration], entity.GetTags());
				if (lifeReg > 0)
				{
					lifeComp.AddPoints(lifeReg * dt);
				}
			}

            // Handle block cooldowns here until there is a more fitting system
			foreach (var entity in coordinator.GetEntities<BlockerComponent>())
			{
				entity.GetComponent<BlockerComponent>()?.UpdateCooldown(dt);
            }

                foreach (var entity in coordinator.GetEntities<DoTComponent>())
			{
				var dotComp = entity.GetComponent<DoTComponent>()!;
                double damage = dotComp.Update(dt);
				if (dotComp.TotalDps == 0.0)
				{
					entity.RemoveComponent<DoTComponent>();
				}

                var lifeComp = entity.GetComponent<LifePoolComponent>();
				if (lifeComp is not null && !lifeComp.IsDead)
				{
					lifeComp.ApplyDamage(damage);
					continue;
				}

				var shieldComp = entity.GetComponent<TimeShieldComponent>();
                if (shieldComp is not null && !shieldComp.IsDepleted)
				{
					shieldComp.Drain(damage);
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
