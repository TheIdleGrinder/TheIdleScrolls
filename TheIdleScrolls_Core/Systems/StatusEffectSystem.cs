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

            // Handle DoT effects here until there is a more fitting system
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

            // Handle Momentum here until there is a more fitting system
			foreach (var entity in coordinator.GetEntities<MomentumComponent>())
			{
				var momentumComp = entity.GetComponent<MomentumComponent>()!;
				momentumComp.UpdateTimer(dt);

				var hitMessages = coordinator.FetchMessagesByType<HitLandedMessage>();
				foreach (var hitMessage in hitMessages)
				{
					if (hitMessage.Attacker == entity && hitMessage.Skill.SkillTags.Contains(Tags.AttackSkill))
					{
						momentumComp.ScoreAttackHit();
					}
					else if (hitMessage.Target == entity)
					{
						momentumComp.TakeHit();
					}
				}

                if (momentumComp.IsChanged)
				{
					// Generate modifiers when the entity first gains momentum
					// The component is added by the StatUpdateSystem if an entity has a momentum limit above 0
                    if (momentumComp.Modifiers.Count == 0)
					{
						momentumComp.Modifiers.Add(new Modifiers.Modifier("momentum_dmg", Modifiers.ModifierType.More, 0.0, [Tags.Damage, Tags.Attack], []));
						foreach (var mod in momentumComp.Modifiers)
						{
							entity.GetOrAddComponent<ModifierComponent>().AddModifier(mod);
						}
					}
					// Update modifiers
					momentumComp.Modifiers[0].Value = Stats.DamagePerMomentum * momentumComp.Momentum;
					coordinator.PostMessage(this, new MomentumChangedMessage(entity, momentumComp.Momentum, momentumComp.MomentumAtLastUpdate));
					momentumComp.MarkAsUpdated();
                }
            }
        }
    }

	public record StatusEffectExpiredMessage(Entity Target, StatusEffect Effect) : IMessage
	{
		string IMessage.BuildMessage() => $"'{Effect.Name}' expired on {Target.GetName()}";
		IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Low;
	}

    public record MomentumChangedMessage(Entity Entity, int NewValue, int OldValue) : IMessage
    {
        string IMessage.BuildMessage() => $"{Entity.GetName()} momentum changed: {OldValue} -> {NewValue}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Low;
    }
}
