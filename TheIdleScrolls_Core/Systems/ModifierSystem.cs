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
    public class ModifierSystem : AbstractSystem
    {
        int m_initialFullUpdates = 2; // CornerCut: Do a full update on the first two frames to give all other systems time to setup all components

        static List<string> WeaponAbilities = new()
        {
            Properties.Constants.Key_Ability_Axe,
            Properties.Constants.Key_Ability_Blunt,
            Properties.Constants.Key_Ability_LongBlade,
            Properties.Constants.Key_Ability_Polearm,
            Properties.Constants.Key_Ability_ShortBlade,
        };

        static List<string> ArmorAbilities = new()
        {
            Properties.Constants.Key_Ability_HeavyArmor,
            Properties.Constants.Key_Ability_LightArmor,
        };

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
                        if (WeaponAbilities.Contains(key))
                        {
                            double dmgMult = Functions.CalculateAbilityAttackDamageBonus(ability.Level);
                            double speedMult = Functions.CalculateAbilityAttackSpeedBonus(ability.Level);
                            modComp.AddModifier(new($"{key}_dmg", Modifiers.ModifierType.More, dmgMult, new()
                                { Definitions.Tags.Damage, key }, new()));
                            modComp.AddModifier(new($"{key}_aps", Modifiers.ModifierType.More, speedMult, new()
                                { Definitions.Tags.AttackSpeed, key }, new()));
                        }
                        if (ArmorAbilities.Contains(key))
                        {
                            double defenseMult = Functions.CalculateAbilityDefenseBonus(ability.Level);
                            modComp.AddModifier(new($"{key}", Modifiers.ModifierType.More, defenseMult, new()
                                { Definitions.Tags.Defense, key }, new()));
                        }
                        if (key == Properties.Constants.Key_Ability_Crafting)
                        {
                            // skip, this is handled by the crafting system for now
                        }
                    }
                }

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
