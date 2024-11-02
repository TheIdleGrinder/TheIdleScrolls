using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Systems
{
    public class EvasionSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            foreach (var entity in coordinator.GetEntities<DefenseComponent>())
            {
                var defenseComponent = entity.GetComponent<DefenseComponent>()!;
                var evaderComponent = entity.GetComponent<EvaderComponent>();

                if (defenseComponent.Evasion <= 0)
                {
                    if (evaderComponent != null)
                    {
                        entity.RemoveComponent<EvaderComponent>();
                    }
                    continue;
                }
                else
                {
                    if (evaderComponent == null)
                    {
                        evaderComponent = new();
                        entity.AddComponent(evaderComponent);
                    }

                    var battlerComp = entity.GetComponent<BattlerComponent>();
                    if (battlerComp == null)
                        continue;

                    if (battlerComp.Battle.IsFinished)
                    {
                        ResetEvaderComponent(evaderComponent);
                    }
                    else if (battlerComp.Battle.State == Battle.BattleState.InProgress)
                    {
                        UpdateEvaderComponent(evaderComponent, dt);
                    }
                }
            }
        }

        private static void UpdateEvaderComponent(EvaderComponent evaderComponent, double dt)
        {
            evaderComponent.Duration.Update(dt);
            if (evaderComponent.Duration.HasFinished)
            {
                evaderComponent.Active = !evaderComponent.Active;
                evaderComponent.Duration.Reset();
                evaderComponent.Duration.ChangeDuration(evaderComponent.Active 
                                                        ? evaderComponent.EvasionDuration 
                                                        : evaderComponent.ChargeDuration);
            }
        }

        private static void ResetEvaderComponent(EvaderComponent evaderComponent)
        {
            evaderComponent.Active = false;
            evaderComponent.Duration.Reset();
            evaderComponent.Duration.ChangeDuration(evaderComponent.ChargeDuration);
        }
    }
}
