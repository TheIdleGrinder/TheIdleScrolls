using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;

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
                || coordinator.MessageTypeIsOnBoard<PerkUpdatedMessage>();

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
                    var localTags = globalTags.Concat(item.GetTags()).ToList();

                    if (itemComp != null && weaponComp != null)
                    {
                        double localDmg = weaponComp.Damage;
                        double localCD = weaponComp.Cooldown;
                        weaponCount++;

                        if (modComp != null)
                        {
                            localDmg = modComp.ApplyApplicableModifiers(localDmg, localTags.Append(Definitions.Tags.Damage));
                            localCD = 1.0 / modComp.ApplyApplicableModifiers(1.0 / localCD, 
                                localTags.Append(Definitions.Tags.AttackSpeed)); // invert due to speed/cooldown mismatch
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
                            localArmor = modComp.ApplyApplicableModifiers(localArmor, 
                                localTags.Append(Definitions.Tags.Defense).Append(Definitions.Tags.ArmorRating));
                            localEvasion = modComp.ApplyApplicableModifiers(localEvasion, 
                                localTags.Append(Definitions.Tags.Defense).Append(Definitions.Tags.EvasionRating));
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
                rawDamage = modComp?.ApplyApplicableModifiers(rawDamage, globalTags.Append(Definitions.Tags.Damage)) ?? rawDamage;
                // invert attack speed due to speed/cooldown mismatch
                cooldown = 1.0 / modComp?.ApplyApplicableModifiers(1.0 / cooldown,
                    globalTags.Append(Definitions.Tags.AttackSpeed)) ?? cooldown;
            }

            if (armorCount == 0)
            {
                var tags = globalTags.Append(Definitions.Tags.Defense);
                armor = modComp?.ApplyApplicableModifiers(armor, tags.Append(Definitions.Tags.ArmorRating)) ?? armor;
                evasion = modComp?.ApplyApplicableModifiers(evasion, tags.Append(Definitions.Tags.EvasionRating)) ?? evasion;
            }

            var attackComp = player.GetComponent<AttackComponent>();
            if (attackComp != null)
            {
                cooldown *= 1.0 + encumbrance / 100.0; // Encumbrance slows attack speed multiplicatively
                
                attackComp.RawDamage = Math.Round(rawDamage);
                // CornerCut(?): This makes the cooldown not reset in case of a weapon swap.
                // Theoretically exploitable by switching between fast and slow weapons
                // Potential TODO: Detect weapon swap
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
            List<string> tags = new();

            var equipComp = player.GetComponent<EquipmentComponent>();
            if (equipComp != null)
            {
                var items = equipComp.GetItems();
                List<string> weapons = items.Where(i => i.IsWeapon()).Select(i => i.GetComponent<ItemComponent>()!.FamilyName).ToList();
                if (weapons.Count == 0)
                {
                    tags.Add(Definitions.Tags.Unarmed);
                }
                else if (weapons.Count >= 2)
                {
                    tags.Add(Definitions.Tags.DualWield);
                    if (weapons.Any(f => f != weapons[0])) // different weapons
                    {
                        tags.Add(Definitions.Tags.MixedWeapons);
                    }
                }

                HashSet<string> armors = items.Where(i => i.IsArmor()).Select(i => i.GetComponent<ItemComponent>()!.FamilyName).ToHashSet();
                if (armors.Count == 0)
                {
                    tags.Add(Definitions.Tags.Unarmored);
                }
                else if (armors.Count > 1)
                {
                    tags.Add(Definitions.Tags.MixedArmor);
                }
            }
            else // No equipment => unarmed, unarmored
            {
                tags.Add(Definitions.Tags.Unarmed);
                tags.Add(Definitions.Tags.Unarmored);
            }

            var comp = player.GetComponent<TagsComponent>();
            if (comp == null)
                player.AddComponent<TagsComponent>(new(tags));
            else
                player.GetComponent<TagsComponent>()?.Reset(tags);
        }
    }

    record AttackStats(double Damage, double Cooldwon, List<string> WeaponFamilyIds);

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
