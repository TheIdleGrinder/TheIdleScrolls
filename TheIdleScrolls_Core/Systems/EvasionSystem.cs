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
        static readonly string ModifierId = "EvaderComponent:Evasion";

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

                    if (battlerComp.Battle.State == Battle.BattleState.Initialized)
                    {
                        ResetEvaderComponent(evaderComponent);
                    }
                    else if (battlerComp.Battle.State == Battle.BattleState.InProgress)
                    {
                        SetupPlayerEvaderComponent(battlerComp.Battle);
                        bool toggled = UpdateEvaderComponent(evaderComponent, dt);
                        if (toggled)
                        {
                            var modComp = entity.GetComponent<ModifierComponent>();
                            if (modComp == null)
                            {
                                modComp = new();
                                entity.AddComponent(modComp);
                            }
                            if (evaderComponent.Active)
                            {
                                modComp.AddModifier(new(ModifierId, Modifiers.ModifierType.More, -1.0, [ Definitions.Tags.TimeLoss ], []));
                            }
                            else
                            {
                                modComp.RemoveModifier(ModifierId);
                            }
                        }
                    }
                }
            }
        }

        private static bool UpdateEvaderComponent(EvaderComponent evaderComponent, double dt)
        {
            evaderComponent.Duration.Update(dt);
            if (evaderComponent.Duration.HasFinished)
            {
                evaderComponent.Active = !evaderComponent.Active;
                evaderComponent.Duration.Reset();
                evaderComponent.Duration.ChangeDuration(evaderComponent.Active 
                                                        ? evaderComponent.EvasionDuration 
                                                        : evaderComponent.ChargeDuration);
                return true;
            }
            return false;
        }

        private static void ResetEvaderComponent(EvaderComponent evaderComponent)
        {
            evaderComponent.Active = false;
            evaderComponent.Duration.Reset();
            evaderComponent.Duration.ChangeDuration(evaderComponent.ChargeDuration);
        }

        private static void SetupPlayerEvaderComponent(Battle battle)
        {
            var evadeComp = battle.Player.GetComponent<EvaderComponent>();
            if (evadeComp == null)
                return;
            double evasion = battle.Player.GetComponent<DefenseComponent>()?.Evasion ?? 0.0;
            double accuracy = battle.Mob?.GetComponent<AccuracyComponent>()?.Accuracy ?? 1.0;
            double bonus = Functions.CalculateEvasionBonusMultiplier(evasion, accuracy) - 1.0;
            if (bonus <= 0.0)
            {
                // This should not happen, a player with 0 evasion should not have an EvaderComponent
                evadeComp.EvasionDuration = 0.0;
                evadeComp.ChargeDuration = 1.0;
            }
            else
            {
                double effectDuration = Math.Min(bonus * Definitions.Stats.MaxEvasionChargeDuration, Definitions.Stats.MaxEvasionEffectDuration);
                double chargeDuration = Math.Min(effectDuration / bonus, Definitions.Stats.MaxEvasionChargeDuration);
                evadeComp.EvasionDuration = effectDuration;
                evadeComp.ChargeDuration = chargeDuration;
            }
            evadeComp.Duration.ChangeDuration(evadeComp.Active ? evadeComp.EvasionDuration : evadeComp.ChargeDuration);
        }
    }
}
