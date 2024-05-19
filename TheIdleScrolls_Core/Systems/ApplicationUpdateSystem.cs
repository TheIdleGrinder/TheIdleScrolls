using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Messages;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Properties;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Systems
{
    public class ApplicationUpdateSystem : AbstractSystem, IGameEventEmitter
    {
        IApplicationModel? m_appModel = null;
        bool m_firstUpdate = true;
        uint m_playerId = 0;
        readonly Cooldown m_abilityUpdate = new(1.0);
        readonly Cooldown m_statisticsUpdate = new(1.0);

        public event PlayerCharacterChangedHandler? PlayerCharacterChanged;
        public event CharacterXpChangedHandler? PlayerXpChanged;
        public event TimeLimitChangedHandler? TimeLimitChanged;
        public event ItemsChangedHandler? PlayerInventoryChanged;
        public event ItemsChangedHandler? PlayerEquipmentChanged;
        public event EncumbranceChangedHandler? PlayerEncumbranceChanged;
        public event CoinsChangedHandler? PlayerCoinsChanged;
        public event OffenseChangedHandler? PlayerOffenseChanged;
        public event DefenseChangedHandler? PlayerDefenseChanged;
        public event AbilitiesChangedHandler? PlayerAbilitiesChanged;
        public event PerksChangedHandler? PlayerPerksChanged;
        public event ModifiersChangedHandler? PlayerModifiersChanged;
        public event MobChangedHandler? MobChanged;
        public event AreaChangedHandler? PlayerAreaChanged;
        public event AutoProceedStateChangedHandler? PlayerAutoProceedStateChanged;
        public event FeatureAvailabilityChangedHandler? FeatureAvailabilityChanged;
        public event AccessibleAreasChangedHandler? AccessibleAreasChanged;
        public event AchievementsChangedHandler? AchievementsChanged;
        public event StatReportChangedHandler? StatReportChanged;
        public event DisplayMessageHandler? DisplayMessageReceived;
        public event NewLogMessagesHandler? NewLogMessages;
        public event DialogueMessageHandler? DialogueMessageReceived;

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
                PlayerCharacterChanged?.Invoke(new(m_playerId, player.GetName(), @class, level));
            }

            // Update XP
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<XpGainMessage>())
            {
                int level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                int xp = player.GetComponent<XpGainerComponent>()?.Current ?? 0;
                int target = player.GetComponent<XpGainerComponent>()?.TargetFunction(level) ?? 0;
                PlayerXpChanged?.Invoke(xp, target);
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
                PlayerInventoryChanged?.Invoke(invItems);
                PlayerEquipmentChanged?.Invoke(equipItems);
            }

            // Update encumbrance
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<EncumbranceChangedMessage>())
            {
                PlayerEncumbranceChanged?.Invoke(player.GetComponent<EquipmentComponent>()?.TotalEncumbrance ?? 0.0);
            }

            // Update coins
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<CoinsChangedMessage>())
            {
                PlayerCoinsChanged?.Invoke(player.GetComponent<CoinPurseComponent>()?.Coins ?? 0);
            }

            // Update attack
            var attackComp = player.GetComponent<AttackComponent>();
            if (attackComp != null)
            {
                PlayerOffenseChanged?.Invoke(attackComp.RawDamage, attackComp.Cooldown.Duration, attackComp.Cooldown.Remaining);
            }

            // Update defenses
            var defenseComp = player.GetComponent<DefenseComponent>();
            if (defenseComp != null && (m_firstUpdate || coordinator.MessageTypeIsOnBoard<StatsUpdatedMessage>()))
            {
                PlayerDefenseChanged?.Invoke(defenseComp.Armor, defenseComp.Evasion, 
                    Functions.CalculateDefenseRating(defenseComp.Armor, defenseComp.Evasion, player.GetLevel()));
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
                    PlayerAbilitiesChanged?.Invoke(representations);
                }
            }

            // Update perks
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<PerkUpdatedMessage>())
            {
                var perkComp = player.GetComponent<PerksComponent>();
                if (perkComp != null)
                {
                    var representations = perkComp.GetPerks()
                        .Select(p => new PerkRepresentation(p.Name, p.Description, p.Modifiers.Select(m => m.ToPrettyString()).ToList()))
                        .ToList();
                    PlayerPerksChanged?.Invoke(representations);
                }
            }

            // Update modifiers
            if (coordinator.MessageTypeIsOnBoard<ModifiersUpdatedMessage>())
            {
                var modComp = player.GetComponent<ModifierComponent>();
                if (modComp != null)
                {
                    PlayerModifiersChanged?.Invoke(modComp.GetModifiers().Select(m => m?.ToPrettyString() ?? "??").ToList()); ;
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
                        MobChanged?.Invoke(rep);
                }
            }

            // Update area
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<AreaChangedMessage>())
            {
                var locationComp = player.GetComponent<LocationComponent>() ?? throw new Exception("Player has no LocationComponent");
                var zone = locationComp.GetCurrentZone(world.Map) ?? new();
                PlayerAreaChanged?.Invoke(zone.Name, zone.Level, locationComp.InDungeon);
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
                        if (!travelComp.AvailableDungeons.Contains(dungeon.Id))
                            continue;
                        dungeons.Add(new DungeonRepresentation(dungeon.Id, 
                            dungeon.Name,
                            dungeon.Level,
                            dungeon.Description.Replace("\\n", "\n"),
                            dungeon.Rarity
                        ));
                    }
                }
                AccessibleAreasChanged?.Invoke(maxWilderness, dungeons);
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
                            a.Status == Achievements.AchievementStatus.Awarded,
                            a.Perk?.Name ?? "")
                    ).ToList();
                    AchievementsChanged?.Invoke(achievements, achComp.Achievements.Count);
                }
            }

            // Update Statistics Report
            if (m_firstUpdate || m_statisticsUpdate.Update(dt) > 0)
            {
                var progComp = player.GetComponent<PlayerProgressComponent>();
                if (progComp != null)
                {
                    var report = progComp.Data.GetReport(world);
                    StatReportChanged?.Invoke(report);
                }
            }

            // Update time limit
            TimeLimitChanged?.Invoke(world.TimeLimit.Remaining, world.TimeLimit.Duration);

            // Update auto proceed
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<AutoProceedStatusMessage>())
            {
                var message = coordinator.FetchMessagesByType<AutoProceedStatusMessage>().LastOrDefault();
                PlayerAutoProceedStateChanged?.Invoke(message?.AutoProceed ?? false);
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
                    DisplayMessageReceived?.Invoke(title, text);
            }
            foreach (var tutorialMessage in tutorialMessages.Where(m => m.QuestMessage == null))
            {
                DisplayMessageReceived?.Invoke(tutorialMessage.Title, tutorialMessage.Text);
            }

            // Handle dialogue messages
            var dialogues = coordinator.FetchMessagesByType<DialogueMessage>();
            foreach (var dialogue in dialogues)
            {
                DialogueMessageReceived?.Invoke(dialogue);
            }

            // Enable previously unlocked features
            if (m_firstUpdate)
            {
                var allFeatures = Enum.GetValues(typeof(GameFeature));
                var availableFeatures = player.GetComponent<PlayerComponent>()?.AvailableFeatures ?? new();
                foreach (var anonymousFeature in Enum.GetValues(typeof(GameFeature)))
                {
                    var feature = (GameFeature)anonymousFeature;
                    FeatureAvailabilityChanged?.Invoke(feature, availableFeatures.Contains(feature));
                }
            }
            // React to feature state messages
            foreach (var featureMessage in coordinator.FetchMessagesByType<FeatureStateMessage>())
            {
                FeatureAvailabilityChanged?.Invoke(featureMessage.Feature, featureMessage.Enabled);
            }

            // Add log messages
            var relevantMessages = m_appModel?.GetRelevantMessagePriorties() ?? new();
            NewLogMessages?.Invoke(FilterMessages(relevantMessages, coordinator.FetchAllMessages()).Select(m => m.Message).ToList());

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
