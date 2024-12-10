using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Modifiers;
using static TheIdleScrolls_Core.Systems.LevelUpSystem;

namespace TheIdleScrolls_Core.Systems
{
    public class PerksSystem : AbstractSystem
    {
        bool FirstUpdate = true;
        bool FirstAchievementUpdate = true;

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
                    perksComp.MarkPerkAsUpdated(perk);
                }

                // Add basic perks
                if (FirstUpdate)
                {
                    AddBasicPerks(perksComp);
                    perksComp.GetPerks().ForEach(m => UpdatePerk(m));
                }

                // Update Modifiers
                var changedPerks = perksComp.GetChangedPerks();
                foreach (var perk in perksComp.GetPerks())
                {
                    if (perk.UpdateTriggers.Any(t => updateTriggers.Contains(t)) 
                        || changedPerks.Contains(perk))
                    {
                        UpdatePerk(perk);
                        coordinator.PostMessage(this, new PerkUpdatedMessage(entity, perk));
                    }
                }
            }

            FirstUpdate = false;
        }

        static List<UpdateTrigger> CollectUpdateTriggers(Coordinator coordinator)
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
            // Create perk for weapon abilities
            List<string> abilities = new();
            List<ModifierType> modifiers = new();
            List<double> values = new();
            List<IEnumerable<string>> localTags = new();
            List<IEnumerable<string>> globalTags = new();
            foreach (string ability in Definitions.Abilities.Attack)
            {
                abilities.Add(ability);
                abilities.Add(ability);
                modifiers.Add(ModifierType.More);
                modifiers.Add(ModifierType.More);
                values.Add(Definitions.Stats.AttackDamagePerAbilityLevel);
                values.Add(Definitions.Stats.AttackSpeedPerAbilityLevel);
                localTags.Add([Definitions.Tags.Damage]);
                localTags.Add([Definitions.Tags.AttackSpeed]);
                globalTags.Add([]);
                globalTags.Add([]);
            }
            perksComponent.AddPerk(PerkFactory.MakeAbilityLevelBasedMultiModPerk("WeaponAbilities", "Abilities: Offense",
                "Increases damage and speed of attacks with different weapons based on ability levels",
                abilities, modifiers,
                values,
                localTags,
                globalTags,
                false)
            );

            abilities = [.. Definitions.Abilities.Defense];
            // Create perk for armor abilities
            perksComponent.AddPerk(PerkFactory.MakeAbilityLevelBasedMultiModPerk("ArmorAbilities", "Abilities: Defense",
                "Increases armor and evasion rating with different armor types based on ability levels",
                abilities,
                abilities.Select(_ => ModifierType.More).ToList(),
                abilities.Select(_ => Definitions.Stats.DefensePerAbilityLevel).ToList(),
                abilities.Select(_ => (IEnumerable<string>)[Definitions.Tags.Defense]).ToList(),
                abilities.Select(_ => (IEnumerable<string>)[]).ToList(),
                false)
            );


            // Create perk for dual wielding
            Perk dualWield = new("dw", "Dual Wielding", 
                $"{Definitions.Stats.DualWieldAttackSpeedMulti:0.#%} more attack speed while dual wielding",
                new(), 
                delegate
                {
                    return new()
                    {
                        new("dw_aps", ModifierType.More, Definitions.Stats.DualWieldAttackSpeedMulti,
                            new() { Definitions.Tags.AttackSpeed },
                            new() { Definitions.Tags.DualWield })
                    };
                }
            );
            
            // Create perk for damage per level
            Perk damagePerLevel = new("dpl", "Damage per Level",
                $"{Definitions.Stats.AttackBonusPerLevel:0.#%} increased damage per level",
                new() { UpdateTrigger.LevelUp },
                delegate (Entity entity, World world, Coordinator coordinator)
                {
                    int level = entity.GetComponent<LevelComponent>()?.Level ?? 0;
                    return new()
                    {
                        new("dpl_dmg", ModifierType.Increase, (level - 1) * Definitions.Stats.AttackBonusPerLevel,
                            new() { Definitions.Tags.Damage }, new())
                    };
                }
            );

            perksComponent.AddPerk(damagePerLevel);
            perksComponent.AddPerk(dualWield);
        }
    }

    // Message that is posted when a perk has been added or updated
    public record PerkUpdatedMessage(Entity Owner, Perk Perk) : IMessage
    {
        string IMessage.BuildMessage() => $"Perk '{Perk.Name}' was updated for entity {Owner.GetName()}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }
}
