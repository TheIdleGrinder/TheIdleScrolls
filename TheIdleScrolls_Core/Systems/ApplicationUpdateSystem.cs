﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Properties;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Systems
{
    public class ApplicationUpdateSystem : AbstractSystem
    {
        IApplicationModel? m_appModel = null;
        bool m_firstUpdate = true;
        uint m_playerId = 0;
        readonly Cooldown m_abilityUpdate = new(1.0);
        readonly Cooldown m_statisticsUpdate = new(1.0);

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_appModel == null)
                return;
            if (m_playerId == 0)
                m_playerId = coordinator.GetEntities<PlayerComponent>().FirstOrDefault()?.Id ?? 0;

            var player = coordinator.GetEntity(m_playerId);
            if (player == null)
                return;
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<LevelUpSystem.LevelUpMessage>())
            {
                int level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                string @class = PlayerFactory.GetCharacterClass(player).Localize();
                m_appModel?.SetPlayerCharacter(new(m_playerId, player.GetName(), @class, level));
            }

            // Update XP
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<XpGainMessage>())
            {
                int level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                int xp = player.GetComponent<XpGainerComponent>()?.Current ?? 0;
                int target = player.GetComponent<XpGainerComponent>()?.TargetFunction(level) ?? 0;
                m_appModel?.SetPlayerXP(xp, target);
            }

            // Update items
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<InventoryChangedMessage>()
                || coordinator.MessageTypeIsOnBoard<ItemReforgedMessage>())
            {
                var inventoryComp = player.GetComponent<InventoryComponent>();
                var equipmentComp = player.GetComponent<EquipmentComponent>();
                var invItems = new List<ItemRepresentation>();
                var equipItems = new List<ItemRepresentation>();

                if (inventoryComp != null)
                {
                    foreach (var item in inventoryComp.GetItems())
                    {
                        var invItem = GenerateItemRepresentation(item);
                        if (invItem != null)
                            invItems.Add(invItem);
                    }
                }
                invItems = invItems.OrderBy(i => i.Id).ToList();

                if (equipmentComp != null)
                {
                    foreach (var item in equipmentComp.GetItems().OrderBy(i => i.IsShield() ? 1: 0))
                    {
                        var eItem = GenerateItemRepresentation(item);
                        if (eItem != null)
                            equipItems.Add(eItem);
                    }
                }

                m_appModel?.SetPlayerItems(invItems, equipItems);
            }

            // Update encumbrance
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<EncumbranceChangedMessage>())
            {
                m_appModel?.SetPlayerEncumbrance(player.GetComponent<EquipmentComponent>()?.TotalEncumbrance ?? 0.0);
            }

            // Update coins
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<CoinsChangedMessage>())
            {
                m_appModel?.SetPlayerCoins(player.GetComponent<CoinPurseComponent>()?.Coins ?? 0);
            }

            // Update attack
            var attackComp = player.GetComponent<AttackComponent>();
            if (attackComp != null)
            {
                m_appModel?.SetPlayerAttack(attackComp.RawDamage, attackComp.Cooldown.Duration, attackComp.Cooldown.Remaining);
            }

            // Update defenses
            var defenseComp = player.GetComponent<DefenseComponent>();
            if (defenseComp != null && (m_firstUpdate || coordinator.MessageTypeIsOnBoard<StatsUpdatedMessage>()))
            {
                m_appModel?.SetPlayerDefense(defenseComp.Armor, defenseComp.Evasion);
            }

            // Update Abilities
            if (m_firstUpdate || m_abilityUpdate.Update(dt) > 0 || coordinator.MessageTypeIsOnBoard<AbilityImprovedMessage>())
            {
                var abilityComp = player.GetComponent<AbilitiesComponent>();
                if (abilityComp != null)
                {
                    var representations = abilityComp.GetAbilities()
                        .Where(a => a.Level > 1 || a.XP > 0) // CornerCut: Filter out crafting ability before first use
                        .Select(a => new AbilityRepresentation(a.Key, a.Key.Localize(), a.Level, a.XP, a.TargetXP))
                        .ToList();
                    m_appModel?.SetPlayerAbilities(representations);
                }
            }

            // Update mob
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<MobSpawnMessage>() 
                || coordinator.MessageTypeIsOnBoard<DamageDoneMessage>())
            {
                Entity? mob = coordinator.GetEntities().Where(e => e.HasComponent<MobComponent>()).FirstOrDefault();
                if (mob != null)
                {
                    var rep = GenerateMobRepresentation(mob);
                    if (rep != null)
                        m_appModel?.SetMob(rep);
                }
            }

            // Update area
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<TravelMessage>())
            {
                m_appModel?.SetArea(world.Zone.Name, world.Zone.Level, world.IsInDungeon());
            }

            // Update accessible areas
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<DungeonOpenedMessage>()
                || coordinator.MessageTypeIsOnBoard<AreaUnlockedMessage>())
            {
                List<DungeonRepresentation> dungeons = new();
                int maxWilderness = player.GetLevel();
                var travelComp = player.GetComponent<TravellerComponent>();
                if (travelComp != null)
                {
                    maxWilderness = travelComp.MaxWilderness;
                    foreach (var dungeon in world.AreaKingdom.Dungeons)
                    {
                        if (!travelComp.AvailableDungeons.Contains(dungeon.Name))
                            continue;
                        dungeons.Add(new DungeonRepresentation(dungeon.Name, 
                            dungeon.Name.Localize(),
                            dungeon.Level,
                            $"{dungeon.Name}_DESCRIPTION".Localize().Replace("\\n", "\n"),
                            dungeon.Rarity
                        ));
                    }
                }
                m_appModel?.SetAccessibleAreas(maxWilderness, dungeons);
            }

            // Update achievements
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<AchievementStatusMessage>())
            {
                var achComp = coordinator.GetEntities<AchievementsComponent>().FirstOrDefault()?.GetComponent<AchievementsComponent>();
                if (achComp != null)
                {
                    const string hiddenInfo = "?????????? (Secret achievement)";
                    List<AchievementRepresentation> achievements = achComp.Achievements
                        .Where(a => a.Status != Achievements.AchievementStatus.Unavailable)
                        .Select(a => new AchievementRepresentation(
                            a.Title,
                            (a.Hidden && a.Status != AchievementStatus.Awarded) ? hiddenInfo : a.Description, 
                            a.Status == Achievements.AchievementStatus.Awarded)
                    ).ToList();
                    m_appModel?.SetAchievements(achievements, achComp.Achievements.Count);
                }
            }

            // Update Statistics Report
            if (m_firstUpdate || m_statisticsUpdate.Update(dt) > 0)
            {
                var progComp = player.GetComponent<PlayerProgressComponent>();
                if (progComp != null)
                {
                    var report = progComp.Data.GetReport(world);
                    m_appModel?.SetStatisticsReport(report);
                }
            }

            // Update time limit
            m_appModel?.SetTimeLimit(world.TimeLimit.Remaining, world.TimeLimit.Duration);

            // Update auto proceed
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<AutoProceedStatusMessage>())
            {
                var message = coordinator.FetchMessagesByType<AutoProceedStatusMessage>().LastOrDefault();
                m_appModel?.SetAutoProceedStatus(message?.AutoProceed ?? false);
            }

            // Handle messages: Attach tutorial messages to quest messages, then handle remaining tutorial messages
            var questMessages = coordinator.FetchMessagesByType<QuestProgressMessage>();
            var tutorialMessages = coordinator.FetchMessagesByType<TutorialMessage>();
            foreach (var questMessage in questMessages)
            {
                string title = questMessage.Quest.ToString();
                string text = questMessage.QuestMessage ?? "";
                // Loop over attached tutorial messages, though there is probably never more than one
                foreach (TutorialMessage tutMessage in tutorialMessages.Where(m => m.QuestMessage == questMessage))
                {
                    if (tutMessage.Title != String.Empty)
                    {
                        title = tutMessage.Title; // CornerCut: multiple tutorial messages would override each other
                    }

                    text += (text != String.Empty ? "\n\n" : "") + tutMessage.Text;
                }
                if (text != String.Empty)
                    m_appModel?.DisplayMessage(title, text);
            }
            foreach (var tutorialMessage in tutorialMessages.Where(m => m.QuestMessage == null))
            {
                m_appModel?.DisplayMessage(tutorialMessage.Title, tutorialMessage.Text);
            }


            // Enable previously unlocked features
            if (m_firstUpdate)
            {
                var allFeatures = Enum.GetValues(typeof(GameFeature));
                var availableFeatures = player.GetComponent<PlayerComponent>()?.AvailableFeatures ?? new();
                foreach (var anonymousFeature in Enum.GetValues(typeof(GameFeature)))
                {
                    var feature = (GameFeature)anonymousFeature;
                    m_appModel?.SetFeatureAvailable(feature, availableFeatures.Contains(feature));
                }
            }
            // React to feature state messages
            foreach (var featureMessage in coordinator.FetchMessagesByType<FeatureStateMessage>())
            {
                m_appModel?.SetFeatureAvailable(featureMessage.Feature, featureMessage.Enabled);
            }

            // Add log messages
            var relevantMessages = m_appModel?.GetRelevantMessagePriorties() ?? new();
            m_appModel?.AddLogMessages(FilterMessages(relevantMessages, coordinator.FetchAllMessages()).Select(m => m.Message).ToList());

            m_firstUpdate = false;
        }

        public void SetApplicationInterface(IApplicationModel? appInterface)
        {
            m_appModel = appInterface;
            m_firstUpdate = true;
        }

        static MobRepresentation? GenerateMobRepresentation(Entity mob)
        {
            if (!mob.HasComponent<MobComponent>())
                return null;
            var mobName = mob.GetComponent<NameComponent>()?.Name ?? "<error>";
            var mobLevel = mob.GetComponent<LevelComponent>()?.Level ?? 0;
            var mobHp = mob.GetComponent<LifePoolComponent>()?.Current ?? 0;
            var mobHpMax = mob.GetComponent<LifePoolComponent>()?.Maximum ?? 0;
            return new MobRepresentation(mob.Id, mobName, mobLevel, mobHp, mobHpMax);
        }

        static ItemRepresentation? GenerateItemRepresentation(Entity item)
        {
            var itemComp = item.GetComponent<ItemComponent>();
            var equipComp = item.GetComponent<EquippableComponent>();
            var weaponComp = item.GetComponent<WeaponComponent>();
            var armorComp = item.GetComponent<ArmorComponent>();
            var valueComp = item.GetComponent<ItemValueComponent>();
            var forgeComp = item.GetComponent<ItemReforgeableComponent>();
            var levelComp = item.GetComponent<LevelComponent>();
            string typeName = GetItemTypeName(item);
            string description = $"Type: {typeName}";            
            if (equipComp != null)
            {
                List<string> slotStrings = new();
                var slots = (EquipmentSlot[])Enum.GetValues(typeof(EquipmentSlot));
                foreach (var slot in slots)
                {
                    int count = equipComp.Slots.Count(s => s == slot);
                    if (count == 0)
                    {
                        continue;
                    }
                    slotStrings.Add((count > 1 ? $"{count}x" : "") + slot.ToString());
                }
                description += $"; Used Slot(s): {string.Join(", ", slotStrings)}";
            }
            description += $"; Skill: {itemComp?.FamilyName ?? "??"}";
            if (levelComp != null)
            {
                description += $"; Drop Level: {levelComp.Level}";
            }
            description += "; ";
            if (weaponComp != null)
            {
                description += $"; Damage: {weaponComp.Damage}; Attack Time: {weaponComp.Cooldown} s";
            }
            if (armorComp != null)
            {
                description += armorComp.Armor != 0.0 ? $"; Armor: {armorComp.Armor}" : "";
                description += armorComp.Evasion != 0.0 ? $"; Evasion: {armorComp.Evasion}" : "";
            }
            if (equipComp != null)
            {
                description += equipComp.Encumbrance != 0.0 ? $"; Encumbrance: {equipComp.Encumbrance}%" : "";
            }
            if (valueComp != null)
            {
                description += $"; ; Value: {valueComp.Value}c";
            }

            return new ItemRepresentation(
                item.Id,
                item.GetName(),
                description,
                equipComp?.Slots ?? new() { EquipmentSlot.Hand },
                itemComp?.Code.RarityLevel ?? 0,
                item.GetComponent<ItemValueComponent>()?.Value ?? 0,
                forgeComp?.Cost ?? -1,
                forgeComp?.Reforged ?? false
                );
        }

        static string GetItemTypeName(Entity item)
        {
            if (item.IsWeapon())
                return LocalizedStrings.Equip_Weapon;
            if (item.IsShield())
                return LocalizedStrings.Equip_Shield;
            var slots = item.GetComponent<EquippableComponent>()?.Slots ?? new();
            if (slots.Count == 0 || !item.IsArmor()) // Should not happen with the current item kingdom
                return "??";

            if (slots.Count > 1)
                return "Custom Gear"; // Does not currently exist
            return slots[0] switch
            {
                EquipmentSlot.Head => LocalizedStrings.Equip_HeadArmor,
                EquipmentSlot.Chest => LocalizedStrings.Equip_ChestArmor,
                EquipmentSlot.Arms => LocalizedStrings.Equip_ArmArmor,
                EquipmentSlot.Legs => LocalizedStrings.Equip_LegArmor,
                _ => "??",
            };
        }

        static List<IMessage> FilterMessages(HashSet<IMessage.PriorityLevel> relevantMessages, List<IMessage> messages)
        {
            return messages.Where(m => relevantMessages.Contains(m.Priority)).ToList();
        }
    }
}
