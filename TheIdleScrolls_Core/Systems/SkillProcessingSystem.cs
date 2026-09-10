using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Skills;
using TheIdleScrolls_Core.Skills.SkillEffects;

namespace TheIdleScrolls_Core.Systems
{
    public class SkillProcessingSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            var users = coordinator.GetEntities<ActiveSkillComponent>().ToList();
            foreach (var user in users)
            {
                if (user.IsDefeated())
                    continue;
                ProcessSkills(user, dt, coordinator);
            }
        }

        SkillTimer.TimerUpdateResult ProcessSkill(Entity entity, ActiveSkill skill, double dt, Coordinator coordinator)
        {
            (SkillTimer.TimerUpdateResult updateResult, List<SkillEffectBundle> effects) = skill.Update(dt);

            double damage = 0;
            double damagePrevented = 0;
            foreach (var effect in effects)
            {
                if (effect.Target == TargetingMode.SingleEnemy)
                {
                    Entity? opponent = ActiveSkill.GetEnemiesInRange(entity, skill.Range).Where(e => !e.IsDefeated()).FirstOrDefault();
                    if (opponent is null)
                        continue; // No valid target in range, skip effect
                    List<ISkillEffectOutcome> outcomes = effect.ApplyToTarget(opponent);
                    foreach (var subEffect in effect.Effects)
                    {
                        if (subEffect is DamageSkillEffect dmgEffect)
                        {
                            damage += dmgEffect.DamageDone;
                            damagePrevented += dmgEffect.Damage - dmgEffect.DamageDone;

                            if (!dmgEffect.Tags.Contains(Tags.DamageOverTime))
                            {
                                coordinator.PostMessage(this, new HitLandedMessage(entity, opponent, skill));
                                coordinator.PostMessage(this, new DamageDoneMessage(entity, opponent, (int)damage, (int)damagePrevented));
                            }
                        }
                    }
                    foreach (var outcome in outcomes)
                    {
                        if (outcome is HitBlocked blockedOutcome)
                        {
                            coordinator.PostMessage(this, new HitBlockedMessage(entity, opponent, blockedOutcome.PreventionPercentage));
                        }
                    }
                    if (opponent.IsDefeated() && !opponent.HasComponent<KilledComponent>())
                    {
                        coordinator.PostMessage(this, new DeathMessage(opponent));
                        opponent.AddComponent(new KilledComponent { Killer = entity.Id });
                        ChargeTriggeredSkills(ActiveSkill.UseTrigger.OnKill, entity);
                    }
                }
                else
                {
                    effect.ApplyToTarget(entity);
                }
            }
            var battlerComp = entity.GetComponent<BattlerComponent>();
            if (battlerComp is not null)
            {
                battlerComp.DamageDealt += (int)damage;
            }
            return updateResult;
        }

        void ProcessSkills(Entity entity, double dt, Coordinator coordinator)
        {
            dt = entity.ApplyAllApplicableModifiers(dt, [Tags.ChargeSpeed], entity.GetTags());
            // Process player skills
            var skillComp = entity.GetComponent<ActiveSkillComponent>();
            if (skillComp is null || skillComp.Skills.Count == 0)
                return;

            double totalElapsed = 0.0;
            List<(SkillEffectBundle Effects, double Range)> collectedSkillEffects = [];
            while (totalElapsed < dt)
            {
                collectedSkillEffects.Clear();
                if (skillComp.CurrentSkill is not null
                    && skillComp.CurrentSkill.GetState() == ActiveSkill.State.NotUsable)
                {
                    skillComp.CurrentSkill.Timer.Stop();
                    skillComp.SwitchToNext();
                }

                double previouslyRemaining = dt - totalElapsed;
                if (skillComp.CurrentSkill is not null
                    && skillComp.CurrentSkill.CurrentState == SkillTimer.State.NotStarted)
                {
                    skillComp.CurrentSkill.StartCharging();
                }
                double remaining = previouslyRemaining;

                SkillTimer.TimerUpdateResult updateResult = new() { RemainingTime = previouslyRemaining };
                if (skillComp.CurrentSkill is not null)
                {
                    updateResult = ProcessSkill(entity, skillComp.CurrentSkill, previouslyRemaining, coordinator);
                }
                if (updateResult.ChargingComplete || updateResult.ActivityComplete || updateResult.CooldownComplete)
                {
                    if (updateResult.ChargingComplete)
                    {
                        var skillTags = skillComp.CurrentSkill?.SkillTags ?? [];
                        // Trigger skills that are set to trigger on attack or cast
                        if (skillTags.Contains(Tags.AttackSkill))
                        {
                            ChargeTriggeredSkills(ActiveSkill.UseTrigger.OnAttack, entity);
                        }
                        else if (skillTags.Contains(Tags.SpellSkill))
                        {
                            ChargeTriggeredSkills(ActiveSkill.UseTrigger.OnCast, entity);
                        }
                        entity.GetComponent<BattlerComponent>()!.SkillsUsed++;
                        // Switch hands after performing an attack
                        if (skillTags.Contains(Tags.AttackSkill))
                        {
                            entity.GetComponent<BattleStatsComponent>()?.SwitchHand();
                        }
                    }
                    coordinator.PostMessage(this, new SkillStateChangedMessage(entity, skillComp.CurrentSkill!, updateResult));
                }
                remaining = updateResult.RemainingTime;

                double elapsed = previouslyRemaining - remaining;
                totalElapsed += elapsed;
                // Also update timer for all other skills
                foreach (var skill in skillComp.Skills)
                {
                    SkillTimer.TimerUpdateResult result = new();
                    if (skill != skillComp.CurrentSkill)
                    {
                        result = ProcessSkill(entity, skill, elapsed, coordinator);
                        if (result.ChargingComplete || result.ActivityComplete || result.CooldownComplete)
                        {
                            coordinator.PostMessage(this, new SkillStateChangedMessage(entity, skill, result));
                        }
                    }
                    if (skill.CurrentState == SkillTimer.State.Active && (!skill.IsAvailableTo(entity) || !skill.IsInUse()))
                    {
                        skill.Deactivate();
                    }
                    if (skillComp.CurrentSkill is null && result.CooldownComplete)
                    {
                        // switch to a skill that finished cooldown if no skill is currently selected
                        skillComp.SwitchToNext();
                        if (skillComp.CurrentSkill is not null)
                            collectedSkillEffects.AddRange(skillComp.CurrentSkill.StartCharging().Select(e => (e, skillComp.CurrentSkill!.Range)));
                    }
                }

                if (remaining > 0.0) // Means that the skill has finished charging or no skill is active
                {
                    skillComp.SwitchToNext();
                    if (skillComp.CurrentSkill is null)
                    {
                        bool outOfRange = skillComp.Skills.Any(s => s.Prevention == ActiveSkill.UsePrevention.NoTargetInRange);
                        if (outOfRange)
                        {
                            MoveTowardsClosestEnemy(entity, remaining); //CornerCut: Use entire rest of frame to move
                        }
                        totalElapsed += remaining;
                    }
                    else
                    {
                        collectedSkillEffects.AddRange(skillComp.CurrentSkill.StartCharging().Select(e => (e, skillComp.CurrentSkill!.Range)));
                    }
                }
            } 
        }

        private static void MoveTowardsClosestEnemy(Entity entity, double dt)
        {
            double moveSpeed = entity.GetComponent<BattleStatsComponent>()?.MovementSpeed ?? Stats.BaseMovementSpeed;
            double coveredDistance = moveSpeed * dt;
            // Find closest enemy
            var position = entity.GetComponent<BattlerComponent>()!.Position;
            var enemies = ActiveSkill.GetEnemiesInRange(entity, double.MaxValue);
            if (enemies.Count == 0)
                return; // No enemies, no movement
            BattlePosition closest = new(double.MaxValue, double.MaxValue);
            foreach (var enemy in enemies)
            {
                var enemyPosition = enemy.GetComponent<BattlerComponent>()!.Position;
                if (position.DistanceTo(enemyPosition) < position.DistanceTo(closest))
                {
                    closest = enemyPosition;
                }
            }
            if (position.DistanceTo(closest) <= coveredDistance)
            {
                position.X = closest.X;
                position.Y = closest.Y;
            }
            else
            {
                double angle = Math.Atan2(closest.Y - position.Y, closest.X - position.X);
                position.X += coveredDistance * Math.Cos(angle);
                position.Y += coveredDistance * Math.Sin(angle);
            }
        }

        private void ChargeTriggeredSkills(ActiveSkill.UseTrigger trigger, Entity user)
        {
            var skillComp = user.GetComponent<ActiveSkillComponent>();
            if (skillComp is null)
                return;
            foreach (var skill in skillComp.Skills)
            {
                if (skill.Trigger == trigger)
                {
                    ChargeTriggeredSkill(skill);
                }
            }
        }

        private void ChargeTriggeredSkill(ActiveSkill skill)
        {
            if (!skill.IsTriggered || skill.CurrentState != SkillTimer.State.NotStarted)
                return;
            var chargeComp = skill.User!.GetOrAddComponent<ChanceChargeComponent>();
            int charges = chargeComp.AddCharge(skill.Id, skill.TriggerChance);
            if (charges > 0)
            {
                chargeComp.RemoveCharge(skill.Id, charges);
                skill.StartCharging();
            }
        }
    }
}
