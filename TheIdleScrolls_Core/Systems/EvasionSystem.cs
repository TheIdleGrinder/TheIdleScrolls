using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
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
                        RemoveEvasionModifier(entity);
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
                        RemoveEvasionModifier(entity);
                    }
                    else if (battlerComp.Battle.State == Battle.BattleState.InProgress)
                    {
                        SetupPlayerEvaderComponent(battlerComp.Battle);
                        double prevention = evaderComponent.Prevention;
                        UpdateEvaderComponent(entity, dt);
                        if (evaderComponent.Prevention != prevention)
                        {
                            SetEvasionModifier(entity, evaderComponent.Prevention);
                        }
                    }
                }
            }
        }

        private static void UpdateEvaderComponent(Entity entity, double dt)
        {
            var evaderComponent = entity.GetComponent<EvaderComponent>();
            if (evaderComponent is null)
                return;
            var modComp = entity.GetComponent<ModifierComponent>();
            evaderComponent.ChargeMultiplier = modComp?.ApplyApplicableModifiers(1.0, [Tags.Evasion, Tags.ChargeSpeed], entity.GetTags()) ?? 1.0;
            // depletion rate is immutable for now
            evaderComponent.DepletionMultiplier = 1.0;
            evaderComponent.Prevention = evaderComponent.UpdateTimer(dt);
            //Console.WriteLine($"Active: {evaderComponent.Active}, dt: {dt:0.####}, prev: {evaderComponent.Prevention}, " +
            //    $"timer: {evaderComponent.Duration.Remaining:0.####}/{evaderComponent.Duration.Duration:0.####}");
        }

        private static void ResetEvaderComponent(EvaderComponent evaderComponent)
        {
            evaderComponent.Active = false;
            evaderComponent.Duration.Reset();
            evaderComponent.Duration.ChangeDuration(evaderComponent.ChargeDuration);
            evaderComponent.Prevention = 0.0;
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
                double effectDuration = Math.Min(bonus * Stats.MaxEvasionChargeDuration, Stats.MaxEvasionEffectDuration);
                double chargeDuration = Math.Min(effectDuration / bonus, Stats.MaxEvasionChargeDuration);
                evadeComp.EvasionDuration = effectDuration;
                evadeComp.ChargeDuration = chargeDuration;
            }
            evadeComp.Duration.ChangeDuration(evadeComp.Active ? evadeComp.EvasionDuration : evadeComp.ChargeDuration);
        }

        private static void SetEvasionModifier(Entity entity, double prevention)
        {
            if (prevention > 0.0)
            {
                AddEvasionModifier(entity, prevention);
            }
            else
            {
                RemoveEvasionModifier(entity);
            }
        }

        private static void AddEvasionModifier(Entity entity, double prevention)
        {
            var modComp = entity.GetComponent<ModifierComponent>();
            if (modComp == null)
            {
                modComp = new();
                entity.AddComponent(modComp);
            }
            modComp.AddModifier(new(ModifierId, Modifiers.ModifierType.More, -prevention, [Tags.TimeLoss], []));
            modComp.AddModifier(new(ModifierId + "-Status", Modifiers.ModifierType.AddFlat, 100.0 * prevention, [Tags.Status, Tags.Resistance], []));
        }

        private static void RemoveEvasionModifier(Entity entity)
        {
            entity.GetComponent<ModifierComponent>()?.RemoveModifier(ModifierId);
            entity.GetComponent<ModifierComponent>()?.RemoveModifier(ModifierId + "-Status");
        }
    }
}
