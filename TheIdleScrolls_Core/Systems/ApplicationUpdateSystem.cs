using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Systems
{
    public class ApplicationUpdateSystem : AbstractSystem
    {
        IApplicationModel? m_appModel = null;
        bool m_firstUpdate = true;
        uint m_playerId = 0;

        Cooldown m_abilityUpdate = new(1.0);
        Cooldown m_statisticsUpdate = new(1.0);

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_appModel == null)
                return;
            if (m_playerId == 0)
                m_playerId = coordinator.GetEntities<PlayerComponent>().FirstOrDefault()?.Id ?? 0;

            var player = coordinator.GetEntity(m_playerId);
            if (player == null)
                return;
            if (m_firstUpdate)
            {
                m_appModel?.SetPlayerCharacter(m_playerId, player.GetName());
            }

            // Update XP
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<XpGainMessage>() || coordinator.MessageTypeIsOnBoard<LevelUpSystem.LevelUpMessage>())
            {
                int level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                int xp = player.GetComponent<XpGainerComponent>()?.Current ?? 0;
                int target = player.GetComponent<XpGainerComponent>()?.TargetFunction(level) ?? 0;
                m_appModel?.SetPlayerLevel(level, xp, target);
            }

            // Update items
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<InventoryChangedMessage>())
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

                if (equipmentComp != null)
                {
                    foreach (var item in equipmentComp.GetItems())
                    {
                        var eItem = GenerateItemRepresentation(item);
                        if (eItem != null)
                            equipItems.Add(eItem);
                    }
                }

                m_appModel?.SetPlayerItems(invItems, equipItems);
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
                        .Select(a => new AbilityRepresentation(a.Key, a.Name, a.Level, a.XP, a.TargetXP))
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
                            $"{dungeon.Name}_DESCRIPTION".Localize()
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

            // React to tutorial messages
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<TutorialMessage>())
            {
                var progress = player.GetComponent<PlayerProgressComponent>()?.Data?.TutorialProgress;
                if (progress != null)
                {
                    m_appModel?.SetFeatureAvailable(GameFeature.Inventory, player.HasComponent<InventoryComponent>());
                    m_appModel?.SetFeatureAvailable(GameFeature.Armor, progress.Contains(TutorialStep.Armor));
                    m_appModel?.SetFeatureAvailable(GameFeature.Abilities, progress.Contains(TutorialStep.Abilities));
                    m_appModel?.SetFeatureAvailable(GameFeature.Travel, player.HasComponent<TravellerComponent>());
                }

                var messages = coordinator.FetchMessagesByType<TutorialMessage>();
                foreach (var message in messages)
                {
                    m_appModel?.DisplayMessage(message.Title, message.Text);
                }
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
            string description = $"[{itemComp?.FamilyName ?? "??"}]";
            if (weaponComp != null)
            {
                description += $"; {weaponComp.Damage} Damage; {weaponComp.Cooldown} s/A";
            }
            if (armorComp != null)
            {
                description += armorComp.Armor != 0.0 ? $"; {armorComp.Armor} Armor" : "";
                description += armorComp.Evasion != 0.0 ? $"; {armorComp.Evasion} Evasion" : "";
            }
            if (equipComp != null)
            {
                description += equipComp.Encumbrance != 0.0 ? $"; {equipComp.Encumbrance} Encumbrance" : "";
            }
            return new ItemRepresentation(
                item.Id,
                item.GetName(),
                description,
                new() { equipComp?.Slot ?? EquipmentSlot.Hand }
                );
        }

        static List<IMessage> FilterMessages(HashSet<IMessage.PriorityLevel> relevantMessages, List<IMessage> messages)
        {
            return messages.Where(m => relevantMessages.Contains(m.Priority)).ToList();
        }
    }
}
