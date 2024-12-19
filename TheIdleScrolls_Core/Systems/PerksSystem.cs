using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
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
            foreach (var messageType in coordinator.FetchAllMessages().Select(m => m.GetType()))
            {
                if (messageType == typeof(MobSpawnMessage))
                    triggers.Add(UpdateTrigger.BattleStarted);
                if (messageType == typeof(DeathMessage))
                    triggers.Add(UpdateTrigger.BattleFinished);
                if (messageType == typeof(DamageDoneMessage))
                    triggers.Add(UpdateTrigger.AttackPerformed);
                if (messageType == typeof(LevelUpMessage))
                    triggers.Add(UpdateTrigger.LevelUp);
                if (messageType == typeof(AbilityImprovedMessage) || messageType == typeof(AbilityAddedMessage))
                    triggers.Add(UpdateTrigger.AbilityIncreased);
                if (messageType == typeof(ItemMovedMessage))
                    triggers.Add(UpdateTrigger.EquipmentChanged);
                if (messageType == typeof(AchievementStatusMessage))
                    triggers.Add(UpdateTrigger.AchievementUnlocked);
            }
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
                values.Add(Stats.AttackDamagePerAbilityLevel);
                values.Add(Stats.AttackSpeedPerAbilityLevel);
                localTags.Add([Tags.Damage]);
                localTags.Add([Tags.AttackSpeed]);
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

            abilities = [.. Abilities.Defense];
            // Create perk for armor abilities
            perksComponent.AddPerk(PerkFactory.MakeAbilityLevelBasedMultiModPerk("ArmorAbilities", "Abilities: Defense",
                "Increases armor and evasion rating with different armor types based on ability levels",
                abilities,
                abilities.Select(_ => ModifierType.More).ToList(),
                abilities.Select(_ => Stats.DefensePerAbilityLevel).ToList(),
                abilities.Select(_ => (IEnumerable<string>)[Tags.Defense]).ToList(),
                abilities.Select(_ => (IEnumerable<string>)[]).ToList(),
                false)
            );

            // Create perk for fighting styles
            perksComponent.AddPerk(new("FightingStyles", "Fighting Styles", 
                "Gain combat bonusses based on the ability level of your current fighting style",
                [UpdateTrigger.AbilityIncreased],
                (e, w, c) => {
                    List<Modifier> modifiers = [];
                    int level = e.GetComponent<AbilitiesComponent>()?.GetAbility(Abilities.DualWield)?.Level ?? 0;
                    if (level > 0)
                    {
                        modifiers.Add(new($"abl{Abilities.DualWield}", ModifierType.More, level * 0.005, [Tags.Damage], [Tags.DualWield]));
                    }
                    level = e.GetComponent<AbilitiesComponent>()?.GetAbility(Abilities.Shielded)?.Level ?? 0;
                    if (level > 0)
                    {
                        modifiers.Add(new($"abl{Abilities.Shielded}", ModifierType.More, level * 0.005, [Tags.Defense], [Tags.Shielded]));
                    }
                    level = e.GetComponent<AbilitiesComponent>()?.GetAbility(Abilities.SingleHanded)?.Level ?? 0;
                    if (level > 0)
                    {
                        modifiers.Add(new($"abl{Abilities.SingleHanded}", ModifierType.More, level * 0.005, [Tags.TimeShield], [Tags.SingleHanded]));
                    }
                    level = e.GetComponent<AbilitiesComponent>()?.GetAbility(Abilities.TwoHanded)?.Level ?? 0;
                    if (level > 0)
                    {
                        modifiers.Add(new($"abl{Abilities.TwoHanded}", ModifierType.More, level * 0.005, [Tags.AttackSpeed], [Tags.TwoHanded]));
                    }
                    return modifiers;
                })
            );
                        
            // Create perk for damage per level
            Perk damagePerLevel = new("dpl", "Damage per Level",
                $"{Stats.AttackBonusPerLevel:0.#%} increased damage per level",
                new() { UpdateTrigger.LevelUp },
                delegate (Entity entity, World world, Coordinator coordinator)
                {
                    int level = entity.GetComponent<LevelComponent>()?.Level ?? 0;
                    return new()
                    {
                        new("dpl_dmg", ModifierType.Increase, (level - 1) * Stats.AttackBonusPerLevel,
                            new() { Tags.Damage }, new())
                    };
                }
            );

            perksComponent.AddPerk(damagePerLevel);
        }
    }

    // Message that is posted when a perk has been added or updated
    public record PerkUpdatedMessage(Entity Owner, Perk Perk) : IMessage
    {
        string IMessage.BuildMessage() => $"Perk '{Perk.Name}' was updated for entity {Owner.GetName()}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }
}
