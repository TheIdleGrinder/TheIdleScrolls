using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Modifiers;
using static TheIdleScrolls_Core.Systems.LevelUpSystem;

namespace TheIdleScrolls_Core.Systems
{
    public class PerksSystem : AbstractSystem
    {
        bool FirstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            // Collect triggers
            List<UpdateTrigger> updateTriggers = CollectUpdateTriggers(coordinator);

            foreach (Entity entity in coordinator.GetEntities<PerksComponent>())
            {
                var perksComp = entity.GetComponent<PerksComponent>()!; // has to exist
                var modsComp = entity.GetComponent<ModifierComponent>();

                void UpdatePerk(Perk perk)
                {
                    if (modsComp != null)
                    {
                        perk.Modifiers.ForEach(m => modsComp.RemoveModifier(m.Id));
                    }
                    perk.UpdateModifiers(entity, world, coordinator);
                    if (modsComp != null)
                    {
                        perk.Modifiers.ForEach(m => modsComp.AddModifier(m));
                    }
                }

                if (FirstUpdate)
                {
                    AddBasicPerks(perksComp);
                    perksComp.GetPerks().ForEach(m => UpdatePerk(m));
                }

                // Update Modifiers
                
                foreach (var perk in perksComp.GetPerks())
                {
                    if (perk.UpdateTriggers.Any(t => updateTriggers.Contains(t)))
                    {
                        UpdatePerk(perk);
                        coordinator.PostMessage(this, new PerkUpdatedMessage(entity, perk));
                    }
                }
            }

            FirstUpdate = false;
        }

        List<UpdateTrigger> CollectUpdateTriggers(Coordinator coordinator)
        {
            List<UpdateTrigger> triggers = new();

            /// CornerCut: triggering entity is not checked
            if (coordinator.MessageTypeIsOnBoard<MobSpawnMessage>())
                triggers.Add(UpdateTrigger.BattleStarted);
            if (coordinator.MessageTypeIsOnBoard<DeathMessage>())
                triggers.Add(UpdateTrigger.BattleFinished);
            if (coordinator.MessageTypeIsOnBoard<DamageDoneMessage>())
                triggers.Add(UpdateTrigger.AttackPerformed);
            if (coordinator.MessageTypeIsOnBoard<LevelUpMessage>())
                triggers.Add(UpdateTrigger.LevelUp);
            if (coordinator.MessageTypeIsOnBoard<AbilityImprovedMessage>())
                triggers.Add(UpdateTrigger.AbilityIncreased);
            if (coordinator.MessageTypeIsOnBoard<ItemMovedMessage>())
                triggers.Add(UpdateTrigger.EquipmentChanged);
            if (coordinator.MessageTypeIsOnBoard<AchievementStatusMessage>())
                triggers.Add(UpdateTrigger.AchievementUnlocked);

            return triggers;
        }

        static void AddBasicPerks(PerksComponent perksComponent)
        {
            // Create perks for weapon abilities
            foreach (string ability in Definitions.Abilities.Weapons)
            {
                perksComponent.AddPerk(PerkFactory.MakeOffensiveAbilityBasedPerk(ability, ability,
                    Definitions.Stats.AttackDamagePerAbilityLevel, Definitions.Stats.AttackSpeedPerAbilityLevel));
            }
            // Create perks for armor abilities
            foreach (string ability in Definitions.Abilities.Armors)
            {
                perksComponent.AddPerk(PerkFactory.MakeDefensiveAbilityBasedPerk(ability, ability,
                    Definitions.Stats.DefensePerAbilityLevel));
            }
            // Create perk for dual wielding
            Perk dualWield = new("dw", "Dual Wielding", 
                $"{Definitions.Stats.DualWieldAttackSpeedMulti:0.#%}% more attack speed while dual wielding",
                new(), 
                delegate
                {
                    return new()
                    {
                        new("dw_aps", ModifierType.More, Definitions.Stats.DualWieldAttackSpeedMulti,
                            new() { Definitions.Tags.AttackSpeed, Definitions.Tags.DualWield })
                    };
                }
            );
            perksComponent.AddPerk(dualWield);
            // Create perk for damage per level
            Perk damagePerLevel = new("dpl", "Damage per Level",
                $"{Definitions.Stats.AttackBonusPerLevel:0.#%}% increased damage per level",
                new() { UpdateTrigger.LevelUp },
                delegate (Entity entity, World world, Coordinator coordinator)
                {
                    int level = entity.GetComponent<LevelComponent>()?.Level ?? 0;
                    return new()
                    {
                        new("dpl_dmg", ModifierType.Increase, (level - 1) * Definitions.Stats.AttackBonusPerLevel,
                            new() { Definitions.Tags.Damage })
                    };
                }
            );
            perksComponent.AddPerk(damagePerLevel);
        }
    }

    // Message that is posted when a perk has been added or updated
    public class PerkUpdatedMessage : IMessage
    {
        public PerkUpdatedMessage(Entity owner, Perk perk)
        {
            Owner = owner;
            Perk = perk;
        }

        public Perk Perk { get; }

        public Entity Owner { get; }

        string IMessage.BuildMessage() => $"Perk '{Perk.Name}' was updated for entity {Owner.GetName()}";

        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }
}
