﻿using MiniECS;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Crafting;
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
        public event PerkUpdatedHandler? PerkUpdated;
        public event AvailablePerkPointsChangedHandler? AvailablePerkPointsChanged;
        public event MobChangedHandler? MobChanged;
        public event AreaChangedHandler? PlayerAreaChanged;
        public event AutoProceedStateChangedHandler? PlayerAutoProceedStateChanged;
        public event FeatureAvailabilityChangedHandler? FeatureAvailabilityChanged;
        public event AccessibleAreasChangedHandler? AccessibleAreasChanged;
        public event AvailableCraftingRecipesChangedHandler? AvailableCraftingRecipesChanged;
        public event CraftingBenchChangedHandler? CraftingBenchChanged;
        public event AchievementsChangedHandler? AchievementsChanged;
        public event StatReportChangedHandler? StatReportChanged;
        public event BountyStateChangedHander? BountyStateChanged;
        public event DisplayMessageHandler? DisplayMessageReceived;
        public event NewLogMessagesHandler? NewLogMessages;
        public event DialogueMessageHandler? DialogueMessageReceived;

        UInt64 _Frames = 0;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_appModel == null)
                return;
            if (m_playerId == 0)
                m_playerId = coordinator.GetEntities<PlayerComponent>().FirstOrDefault()?.Id ?? 0;

            _Frames++;

            var player = coordinator.GetEntity(m_playerId);
            if (player == null)
                return;
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<LevelUpSystem.LevelUpMessage>())
            {
                int level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                string @class = PlayerFactory.GetCharacterClass(player).Localize();
                PlayerCharacterChanged?.Invoke(new(m_playerId, player.GetName(), @class, level), player);
            }

            // Update XP
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<XpGainMessage>())
            {
                int level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                int xp = player.GetComponent<XpGainerComponent>()?.Current ?? 0;
                int target = player.GetComponent<XpGainerComponent>()?.TargetFunction(level) ?? 0;
                PlayerXpChanged?.Invoke(xp, target);
            }

            // Update items (update when perks change, because crafting cost/duration might change)
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<InventoryChangedMessage>()
                || coordinator.MessageTypeIsOnBoard<CraftingProcessFinished>()
                || coordinator.MessageTypeIsOnBoard<PerkUpdatedMessage>()
            )
            {
                var inventoryComp = player.GetComponent<InventoryComponent>();
                var equipmentComp = player.GetComponent<EquipmentComponent>();

                if (inventoryComp != null)
                {
                    List<IItemEntity> invItems = inventoryComp.GetItems()
                        .Select(i => new ItemEntityWrapper(i, player))
                        .ToList<IItemEntity>();
                    PlayerInventoryChanged?.Invoke(invItems);
                }

                if (equipmentComp != null)
                {
                    List<IItemEntity> equipItems = equipmentComp.GetItems()
                        .Select(i => new ItemEntityWrapper(i, player))
                        .ToList<IItemEntity>();
                    PlayerEquipmentChanged?.Invoke(equipItems);
                }
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
                        .Select(a => new AbilityRepresentation(a.Key, a.Key.Localize(), a.Level, a.XP, a.TargetXP))
                        .ToList();
                    PlayerAbilitiesChanged?.Invoke(representations);
                }
            }

            // Update perks
            if (m_firstUpdate 
                || coordinator.MessageTypeIsOnBoard<PerkPointLimitChanged>()
                || coordinator.MessageTypeIsOnBoard<PerkAddedMessage>()
                )
            {
                var perkComp = player.GetComponent<PerksComponent>();
                if (perkComp != null)
                {
                    PlayerPerksChanged?.Invoke([]); // Empty because the GUI now takes Perks directly from the component
                }
            }
            foreach (var message in coordinator.FetchMessagesByType<PerkUpdatedMessage>())
            {
                PerkUpdated?.Invoke(message.Perk.Id);
            }
            if (m_firstUpdate 
                || coordinator.MessageTypeIsOnBoard<PerkPointLimitChanged>()
                || coordinator.MessageTypeIsOnBoard<PerkLevelChangedMessage>())
            {
                var perkComp = player.GetComponent<PerksComponent>();
                if (perkComp != null)
                {
                    AvailablePerkPointsChanged?.Invoke(perkComp.GetAvailablePerkPoints());
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
                        int[] levels = travelComp.AvailableDungeons.GetValueOrDefault(dungeon.Id, []);
                        if (levels.Length == 0)
                            continue;
                        dungeons.Add(new DungeonRepresentation(dungeon.Id, 
                            dungeon.Name,
                            levels,
                            dungeon.Description.Replace("\\n", "\n"),
                            dungeon.Rarity
                        ));
                    }
                }
                AccessibleAreasChanged?.Invoke(maxWilderness, dungeons);
            }

            // Update available crafting recipes (update when perks change, because crafting cost/duration might change)
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<AvailableCraftsChanged>() 
                || coordinator.MessageTypeIsOnBoard<PerkUpdatedMessage>()
            )
            {
                var craftComp = player.GetComponent<CraftingBenchComponent>();
                if (craftComp != null)
                {
                    var prototypes = craftComp.AvailablePrototypes
                        .Select(p => new ItemEntityWrapper(p, player)) // Pass player as owner for modifiers to crafting cost, time
                        .ToList<IItemEntity>();
                    AvailableCraftingRecipesChanged?.Invoke(prototypes);
                }
            }

            // Update crafting processes
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<CraftingUpdateMessage>())
            {
                var craftComp = player.GetComponent<CraftingBenchComponent>();
                if (craftComp != null)
                {
                    var representations = craftComp.ActiveCrafts
                        .Select(c => GenerateCraftRepresentation(c))
                        .ToList();
                    CraftingBenchRepresentation bench = new(craftComp.MaxCraftingLevel, 
                                                            craftComp.CraftingSlots, 
                                                            craftComp.MaxActiveCrafts, 
                                                            representations);
                    CraftingBenchChanged?.Invoke(bench);
                }
            }

            // Update achievements
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<AchievementStatusMessage>())
            {
                var achComp = coordinator.GetEntities<AchievementsComponent>().FirstOrDefault()?.GetComponent<AchievementsComponent>();
                if (achComp != null)
                {
                    const string hiddenInfo = "?????????? (Secret achievement)";
                    List<AchievementRepresentation> achievements = achComp.Achievements
                        .Where(a => a.Status != AchievementStatus.Unavailable)
                        .Select(a => new AchievementRepresentation(
                            a.Title,
                            (a.Hidden && a.Status != AchievementStatus.Awarded) ? hiddenInfo : a.Description, 
                            a.Status == AchievementStatus.Awarded,
                            a.Reward?.Description ?? "")
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

            // Update Bounty State
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<BountyMessage>() || coordinator.MessageTypeIsOnBoard<DeathMessage>())
            {
                var bountyComp = player.GetComponent<BountyHunterComponent>();
                if (bountyComp != null)
                {
                    int level = bountyComp.CurrentHuntLevel;
                    int maxLevel = bountyComp.HighestCollected;
                    int anchor = bountyComp.CurrentHuntAnchorLevel;
                    var state = new BountyStateRepresentation(maxLevel, level, anchor,
                                                              bountyComp.CurrentHuntCount, BountySystem.EnemiesPerHunt,
                                                              BountySystem.CalculateBountyReward(level, anchor));
                    BountyStateChanged?.Invoke(state);
                }
            }

            // Update time limit
            var shieldComp = player.GetComponent<TimeShieldComponent>();
            if (shieldComp != null)
            {
                TimeLimitChanged?.Invoke(shieldComp.Remaining, shieldComp.Maximum);
            }

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
                string title = questMessage.MessageTitle ?? questMessage.Quest.ToString();
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
            var mobId = mob.GetComponent<MobComponent>()?.Id ?? "??";
            var mobName = mob.GetComponent<NameComponent>()?.Name ?? "<error>";
            var mobLevel = mob.GetComponent<LevelComponent>()?.Level ?? 0;
            var mobHp = mob.GetComponent<LifePoolComponent>()?.Current ?? 0;
            var mobHpMax = mob.GetComponent<LifePoolComponent>()?.Maximum ?? 0;
            var mobDamage = mob.GetComponent<MobDamageComponent>()?.Multiplier ?? 0.0;
            return new MobRepresentation(mob.Id, mobId, mobName, mobLevel, mobHp, mobHpMax, mobDamage);
        }

        static CraftingProcessRepresentation GenerateCraftRepresentation(CraftingProcess craft)
        {
            var item = new ItemEntityWrapper(craft.TargetItem, null);
            return new CraftingProcessRepresentation(craft.Type, item, craft.Duration.Duration, craft.Duration.Remaining, craft.CoinsPaid);
        }

        static List<IMessage> FilterMessages(HashSet<IMessage.PriorityLevel> relevantMessages, List<IMessage> messages)
        {
            return messages.Where(m => relevantMessages.Contains(m.Priority)).ToList();
        }
    }
}
