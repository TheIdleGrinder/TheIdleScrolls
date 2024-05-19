using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Systems
{
    /// <summary>
    ///     
    ///     DEPRECATED
    /// 
    /// Updates modifiers for all entities with a ModifierComponent. Obsolete and should be replaced by the PerksSystem.
    /// </summary>
    public class ModifierSystem : AbstractSystem
    {
        int m_initialFullUpdates = 2; // CornerCut: Do a full update on the first two frames to give all other systems time to setup all components

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            bool doUpdate = m_initialFullUpdates > 0
                || coordinator.MessageTypeIsOnBoard<LevelUpSystem.LevelUpMessage>()
                || coordinator.MessageTypeIsOnBoard<ItemMovedMessage>()
                || coordinator.MessageTypeIsOnBoard<AbilityImprovedMessage>()
                || coordinator.MessageTypeIsOnBoard<AchievementStatusMessage>();

            if (!doUpdate)
                return;

            foreach (var entity in coordinator.GetEntities<PlayerComponent>())
            {
                var modComp = entity.GetComponent<ModifierComponent>();
                if (modComp == null)
                    continue;
                modComp.Clear();

                var abilityComp = entity.GetComponent<AbilitiesComponent>();
                if (abilityComp != null)
                {
                    foreach (var ability in abilityComp.GetAbilities())
                    {
                        string key = ability.Key;
                        if (Definitions.Abilities.Weapons.Contains(key))
                        {
                            double dmgMult = Functions.CalculateAbilityAttackDamageBonus(ability.Level);
                            double speedMult = Functions.CalculateAbilityAttackSpeedBonus(ability.Level);
                            modComp.AddModifier(new($"{key}_dmg", Modifiers.ModifierType.More, dmgMult, new()
                                { Definitions.Tags.Damage, key }));
                            modComp.AddModifier(new($"{key}_aps", Modifiers.ModifierType.More, speedMult, new()
                                { Definitions.Tags.AttackSpeed, key }));
                        }
                        if (Definitions.Abilities.Armors.Contains(key))
                        {
                            double defenseMult = Functions.CalculateAbilityDefenseBonus(ability.Level);
                            modComp.AddModifier(new($"{key}", Modifiers.ModifierType.More, defenseMult, new()
                                { Definitions.Tags.Defense, key }));
                        }
                        if (key == Properties.Constants.Key_Ability_Crafting)
                        {
                            // skip, this is handled by the crafting system for now
                        }
                    }
                }

                var levelComp = entity.GetComponent<LevelComponent>();
                if (levelComp != null)
                {
                    double bonus = Definitions.Stats.AttackBonusPerLevel * (levelComp.Level - 1);
                    modComp.AddModifier(new("level_dmg", Modifiers.ModifierType.Increase, bonus, new() { Definitions.Tags.Damage }));
                }

                // Dual wield attack speed bonus
                modComp.AddModifier(new("dualWield_aps", Modifiers.ModifierType.More, Definitions.Stats.DualWieldAttackSpeedMulti,
                    new() { Definitions.Tags.DualWield, Definitions.Tags.AttackSpeed }));

                coordinator.PostMessage(this, new ModifiersUpdatedMessage(entity));
            }

            if (m_initialFullUpdates > 0)
                m_initialFullUpdates--;
        }
    }

    public class ModifiersUpdatedMessage : IMessage
    {
        public Entity Entity { get; set; }

        public ModifiersUpdatedMessage(Entity entity)
        {
            Entity = entity;
        }

        string IMessage.BuildMessage()
        {
            return $"Modifiers for '{Entity.GetName()}' updated";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }
}
