using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;

using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Skills.Skills;

namespace TheIdleScrolls_Core.Systems
{
    public class StatUpdateSystem : AbstractSystem
    {
        int m_initialFullUpdates = 2; // CornerCut: Do a full update on the first two frames to give all other systems time to setup all components

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            // Handle changes in skill order here for now
            foreach (var message in coordinator.FetchMessagesByType<SkillOrderChangeRequest>())
            {
                var comp = coordinator.GetEntity(message.EntityId)?.GetComponent<ActiveSkillComponent>();
                var skill = comp?.Skills?.FirstOrDefault(s => s.Id == message.SkillId);
                if (skill is not null)
                {
                    if (message.MoveUp)
                        comp?.MoveSkillUp(skill);
                    else
                        comp?.MoveSkillDown(skill);
                }
            }

            // Same with requests to enable/disable skills
            foreach (var message in coordinator.FetchMessagesByType<SetSkillEnabledRequest>())
            {
                var comp = coordinator.GetEntity(message.EntityId)?.GetComponent<ActiveSkillComponent>();
                comp?.SetSkillEnabled(message.SkillId, message.Enabled);
            }

            bool doUpdate = m_initialFullUpdates > 0
                || coordinator.MessageTypeIsOnBoard<LevelUpSystem.LevelUpMessage>()
                || coordinator.MessageTypeIsOnBoard<ItemMovedMessage>()
                || coordinator.MessageTypeIsOnBoard<AbilityImprovedMessage>()
                || coordinator.MessageTypeIsOnBoard<AchievementStatusMessage>()
                || coordinator.MessageTypeIsOnBoard<PerkUpdatedMessage>()
                || coordinator.MessageTypeIsOnBoard<MobSpawnMessage>() // CornerCut: Force an update at the start of a battle and for new mobs
                || coordinator.MessageTypeIsOnBoard<SkillStateChangedMessage>()
                || coordinator.MessageTypeIsOnBoard<StatusEffectExpiredMessage>()
                || coordinator.MessageTypeIsOnBoard<PerkLevelChangedMessage>();

            if (!doUpdate)
                return;

            var entities = coordinator.GetEntities<PlayerComponent>()
                    .Concat(coordinator.GetEntities<MobComponent>());

            foreach (var entity in entities)
            {
                UpdateTags(entity);

                if (entity.IsPlayer())
                    UpdateLifePool(entity, Stats.BasePlayerHitPoints);
                UpdateDefenses(entity);



                var skillComp = entity.GetComponent<ActiveSkillComponent>();
                if (skillComp != null)
                {
                    DefaultAttack.SetupAttackComponent(entity);

                    foreach (var skill in skillComp.Skills)
                    {
                        skill.SetupForUser(entity);
                    }
                }
            }
			coordinator.PostMessage(this, new StatsUpdatedMessage());
            if (m_initialFullUpdates > 0)
                m_initialFullUpdates--;
        }

        public static void UpdateTags(Entity entity)
        {
            if (!entity.HasComponent<TagsComponent>())
            {
                entity.AddComponent<TagsComponent>(new());
            }
            TagsComponent comp = entity.GetComponent<TagsComponent>()!;
            comp.Reset(Array.Empty<string>());

            void AddOrRemoveTag(string tag, bool add)
            {
                if (add)
                {
                    comp.AddTag(tag);
                }
                else
                {
                    comp.RemoveTag(tag);
                }
            }

            var equipComp = entity.GetComponent<EquipmentComponent>();
            if (equipComp != null)
            {
                var items = equipComp.GetItems();

                HashSet<string> armors = items.Where(i => i.IsArmor())
                    .Select(i => i.GetComponent<ItemComponent>()!.Blueprint.GetRelatedAbilityId())
                    .ToHashSet();
                List<ItemBlueprint> weapons = items.Where(i => i.IsWeapon()).Select(i => i.GetComponent<ItemComponent>()!.Blueprint).ToList();
                List<string> weaponAbilities = weapons.Select(w => w.GetRelatedAbilityId()).ToList();
                AddOrRemoveTag(Tags.Unarmed, weapons.Count == 0);
                AddOrRemoveTag(Tags.MixedWeapons, weapons.Count > 1 && weaponAbilities.Any(f => f != weaponAbilities[0]));

                AddOrRemoveTag(Tags.DualWield, weapons.Count >= 2);
                AddOrRemoveTag(Tags.TwoHanded, weapons.Count == 1 && weapons[0].GetUsedSlots().Count > 1);

                AddOrRemoveTag(Tags.Unarmored, armors.Count == 0);
                AddOrRemoveTag(Tags.MixedArmor, armors.Count > 1);

                foreach (var item in items)
                {
                    comp.AddTags(item.GetTags());
                }

                bool usingShield = comp.HasTag(Tags.Shield);
                AddOrRemoveTag(Tags.Shielded, usingShield);
                AddOrRemoveTag(Tags.SingleHanded, weapons.Count == 1 
                                                  && weapons[0].GetUsedSlots().Count == 1
                                                  && !usingShield);
            }
            else // No equipment => unarmed, unarmored
            {
                comp.AddTag(Tags.Unarmed);
                comp.AddTag(Tags.Unarmored);
            }

            AddOrRemoveTag(Tags.FirstStrike, entity.GetComponent<BattlerComponent>()?.FirstStrike ?? false);
            AddOrRemoveTag(Tags.Evading, entity.GetComponent<EvaderComponent>()?.Active ?? false);
        }

        static void UpdateLifePool(Entity entity, int baseHitPoints)
        {
            var lifeComp = entity.GetComponent<LifePoolComponent>();
            if (lifeComp is null)
                return;
            double hp = entity.ApplyAllApplicableModifiers(baseHitPoints, [Tags.HitPoints], entity.GetTags());
            lifeComp.SetMaximum((int)Math.Ceiling(hp));
        }

        static void UpdateDefenses(Entity entity)
        {
            var statsComp = entity.GetComponent<BattleStatsComponent>();
            if (statsComp is null)
                return;

            double armor = 0.0;
            double evasion = 0.0;
            double encumbrance = 0.0;
            int armorCount = 0;

            var globalTags = entity.GetTags();
            var modComp = entity.GetComponent<ModifierComponent>();

            var equipComp = entity.GetComponent<EquipmentComponent>();
            if (equipComp != null)
            {
                foreach (var item in equipComp.GetItems())
                {
                    var itemComp = item.GetComponent<ItemComponent>();
                    var weaponComp = item.GetComponent<WeaponComponent>();
                    var armorComp = item.GetComponent<ArmorComponent>();
                    encumbrance += item.GetComponent<EquippableComponent>()?.Encumbrance ?? 0.0;
                    var localTags = item.GetTags();

                    // Add situational local tags 
                    var slots = item.GetRequiredSlots();
                    if (item.IsShield())
                    {
                        localTags.Add(Tags.OffHand);
                    }

                    if (itemComp != null && armorComp != null)
                    {
                        var localArmor = armorComp.Armor;
                        var localEvasion = armorComp.Evasion;
                        armorCount++;

                        if (modComp != null)
                        {
                            var tags = localTags.Append(Tags.Defense);
                            localArmor = modComp.ApplyApplicableModifiers(localArmor,
                                tags.Append(Tags.ArmorRating), globalTags);
                            localEvasion = modComp.ApplyApplicableModifiers(localEvasion,
                                tags.Append(Tags.EvasionRating), globalTags);
                        }

                        armor += localArmor;
                        evasion += localEvasion;
                    }
                }
            }

            // Handle global armor and evasion bonuses
            List<string> globalDefTags = [Tags.Global, Tags.Defense];
            if (entity.HasTag(Tags.Unarmored))
            {
                globalDefTags.Add(Abilities.Unarmored); // Bonuses from unarmored ability apply here if unarmored
            }
            armor += modComp?.ApplyApplicableModifiers(0.0, globalDefTags.Append(Tags.ArmorRating), globalTags) ?? 0.0;
            evasion += modComp?.ApplyApplicableModifiers(0.0, globalDefTags.Append(Tags.EvasionRating), globalTags) ?? 0.0;
            double moveSpeed = modComp?.ApplyApplicableModifiers(Stats.BaseMovementSpeed, [Tags.MovementSpeed], globalTags) ?? Stats.BaseMovementSpeed;

            double lowLifeLimit = 0.35;
            var lifeComp = player.GetComponent<BattlerComponent>()?.Battle?.Mob?.GetComponent<LifePoolComponent>();
            AddOrRemoveTag(Tags.FirstStrike, lifeComp?.IsFull ?? false);
            AddOrRemoveTag(Tags.VsLowLife, (lifeComp?.Percentage ?? 1.0) <= lowLifeLimit);
            AddOrRemoveTag(Tags.Evading, player.GetComponent<EvaderComponent>()?.Active ?? false);
            statsComp.Encumbrance = encumbrance;
            statsComp.Evasion = evasion / statsComp.EncumbranceSlowdown;
            statsComp.Armor = armor;
            statsComp.MovementSpeed = moveSpeed / statsComp.EncumbranceSlowdown;
        }
    }


    public class StatsUpdatedMessage : IMessage
    {
        string IMessage.BuildMessage() => $"Player stats updated";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }

    record SkillOrderChangeRequest(uint EntityId, string SkillId, bool MoveUp) : IMessage
    {
        string IMessage.BuildMessage() => $"Request to move skill {SkillId} {(MoveUp ? "up" : "down")} in entity {EntityId}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }

    record SetSkillEnabledRequest(uint EntityId, string SkillId, bool Enabled) : IMessage
    {
        string IMessage.BuildMessage() => $"Request to {(Enabled ? "en" : "dis")}able skill {SkillId} in entity {EntityId}";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.Debug;
    }
}
