using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;

using TheIdleScrolls_Core.Definitions;

namespace TheIdleScrolls_Core.Systems
{
    public class StatUpdateSystem : AbstractSystem
    {
        uint m_player = 0;
        int m_initialFullUpdates = 2; // CornerCut: Do a full update on the first two frames to give all other systems time to setup all components

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == 0)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault()?.Id ?? 0;

            bool doUpdate = m_initialFullUpdates > 0
                || coordinator.MessageTypeIsOnBoard<LevelUpSystem.LevelUpMessage>()
                || coordinator.MessageTypeIsOnBoard<ItemMovedMessage>()
                || coordinator.MessageTypeIsOnBoard<AbilityImprovedMessage>()
                || coordinator.MessageTypeIsOnBoard<AchievementStatusMessage>()
                || coordinator.MessageTypeIsOnBoard<PerkUpdatedMessage>()
                || coordinator.MessageTypeIsOnBoard<TextMessage>() // CornerCut: This is a hack to force an update at the start of a battle
                || coordinator.MessageTypeIsOnBoard<DamageDoneMessage>()
                || coordinator.MessageTypeIsOnBoard<PerkLevelChangedMessage>();

            if (!doUpdate)
                return;

            var player = coordinator.GetEntity(m_player);
            if (player == null)
                return;

            UpdatePlayerTags(player);

            var equipComp = player.GetComponent<EquipmentComponent>();

            double armor = 0.0;
            double evasion = 0.0;
            double encumbrance = 0.0;
            int armorCount = 0;

            double rawDamage = 2.0;
            double cooldown = 1.0;
            int weaponCount = 0;

            var globalTags = player.GetTags();
            var modComp = player.GetComponent<ModifierComponent>();

            if (equipComp != null)
            {
                double combinedDmg = 0.0;
                double combinedCD = 0.0;

                foreach (var item in equipComp.GetItems())
                {
                    var itemComp = item.GetComponent<ItemComponent>();
                    var weaponComp = item.GetComponent<WeaponComponent>();
                    var armorComp = item.GetComponent<ArmorComponent>();
                    encumbrance += item.GetComponent<EquippableComponent>()?.Encumbrance ?? 0.0;
                    var localTags = item.GetTags();

                    if (itemComp != null && weaponComp != null)
                    {
                        double localDmg = weaponComp.Damage;
                        double localCD = weaponComp.Cooldown;
                        weaponCount++;

                        if (modComp != null)
                        {
                            localDmg = modComp.ApplyApplicableModifiers(localDmg, localTags.Append(Tags.Damage), globalTags);
                            localCD = 1.0 / modComp.ApplyApplicableModifiers(1.0 / localCD, 
                                localTags.Append(Tags.AttackSpeed),  // invert due to speed/cooldown mismatch
                                globalTags);
                        }

                        combinedDmg += localDmg;
                        combinedCD += localCD;
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

                if (weaponCount > 0)
                {
                    rawDamage = (combinedDmg / weaponCount);
                    cooldown = (combinedCD / weaponCount);
                }                
            }

            if (weaponCount == 0)
            {
                rawDamage = modComp?.ApplyApplicableModifiers(rawDamage, 
                    [Tags.Damage, Abilities.Unarmed], globalTags) ?? rawDamage;
                // invert attack speed due to speed/cooldown mismatch
                cooldown = 1.0 / modComp?.ApplyApplicableModifiers(1.0 / cooldown,
                    [Tags.AttackSpeed, Abilities.Unarmed], globalTags) ?? cooldown;
            }

            if (armorCount == 0)
            {
                var tags = globalTags.Concat([Tags.Defense, Abilities.Unarmored]);
                armor = modComp?.ApplyApplicableModifiers(armor, tags.Append(Tags.ArmorRating), globalTags) ?? armor;
                evasion = modComp?.ApplyApplicableModifiers(evasion, tags.Append(Tags.EvasionRating), globalTags) ?? evasion;
            }

            // Handle global armor and evasion bonuses
            armor   += modComp?.ApplyApplicableModifiers(0.0, [Tags.ArmorRating,   Tags.Defense, Tags.Global], globalTags) ?? 0.0;
            evasion += modComp?.ApplyApplicableModifiers(0.0, [Tags.EvasionRating, Tags.Defense, Tags.Global], globalTags) ?? 0.0;

            var attackComp = player.GetComponent<AttackComponent>();
            if (attackComp != null)
            {
                attackComp.RawDamage = Math.Round(rawDamage);

                cooldown *= 1.0 + encumbrance / 100.0; // Encumbrance slows attack speed multiplicatively
                cooldown = Math.Max(cooldown, 1.0 / Stats.MaxAttacksPerSecond); // Cap attack speed
                if (cooldown != attackComp.Cooldown.Duration)
                    attackComp.Cooldown.ChangeDuration(cooldown, true);
            }

            var defenseComp = player.GetComponent<DefenseComponent>();
            if (defenseComp != null)
            {
                defenseComp.Evasion = evasion; 
                defenseComp.Armor = armor;
            }
            
            coordinator.PostMessage(this, new StatsUpdatedMessage());
            if (m_initialFullUpdates > 0)
                m_initialFullUpdates--;
        }

        public static void UpdatePlayerTags(Entity player)
        {
            if (!player.HasComponent<TagsComponent>())
            {
                player.AddComponent<TagsComponent>(new());
            }
            TagsComponent comp = player.GetComponent<TagsComponent>()!;
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

            var equipComp = player.GetComponent<EquipmentComponent>();
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

            AddOrRemoveTag(Tags.FirstStrike, player.GetComponent<BattlerComponent>()?.FirstStrike ?? false);
            AddOrRemoveTag(Tags.Evading, player.GetComponent<EvaderComponent>()?.Active ?? false);
        }
    }

    record AttackStats(double Damage, double Cooldown, List<string> WeaponFamilyIds);

    public class StatsUpdatedMessage : IMessage
    {
        string IMessage.BuildMessage()
        {
            return $"Player stats updated";
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Debug;
        }
    }

}
