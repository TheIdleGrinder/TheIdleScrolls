using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
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
                || coordinator.MessageTypeIsOnBoard<AchievementStatusMessage>();

            if (!doUpdate)
                return;

            var player = coordinator.GetEntity(m_player);
            if (player == null)
                return;

            int level = player.GetComponent<LevelComponent>()?.Level ?? 1;

            var equipComp = player.GetComponent<EquipmentComponent>();

            double armor = 0.0;
            double evasion = 0.0;
            double encumbrance = 0.0;

            double baseDamage = 2.0;
            double cooldown = 1.0;

            double dmgMulti = 1.0;
            double apsMulti = 1.0;


            if (equipComp != null)
            {
                double combinedDmg = 0.0;
                double combinedCD = 0.0;
                int weaponCount = 0;
                int armorCount = 0;

                foreach (var item in equipComp.GetItems())
                {
                    var itemComp = item.GetComponent<ItemComponent>();
                    var weaponComp = item.GetComponent<WeaponComponent>();
                    var armorComp = item.GetComponent<ArmorComponent>();
                    encumbrance += item.GetComponent<EquippableComponent>()?.Encumbrance ?? 0.0;

                    if (itemComp != null && weaponComp != null)
                    {
                        combinedDmg += weaponComp.Damage;
                        double localCD = weaponComp.Cooldown;
                        weaponCount++;

                        int abilityLvl = GetAbilityLevel(player, itemComp.Code.FamilyId);
                        if (abilityLvl > 0)
                        {
                            localCD /= (0.5 + 0.02 * abilityLvl); // CornerCut: Make this less 'hidden'
                        }

                        combinedCD += localCD;
                    }

                    if (itemComp != null && armorComp != null)
                    {
                        var localArmor = armorComp.Armor;
                        var localEvasion = armorComp.Evasion;
                        armorCount++;

                        int abilityLvl = GetAbilityLevel(player, itemComp.Code.FamilyId);
                        if (abilityLvl != -1)
                        {
                            var multi = 0.5 + 0.02 * abilityLvl;
                            localArmor *= multi;
                            localEvasion *= multi;
                        }

                        armor += localArmor;
                        evasion += localEvasion;
                    }
                }

                if (armorCount == 0)
                {
                    var achComp = coordinator.GetEntities<AchievementsComponent>()?.FirstOrDefault()?.GetComponent<AchievementsComponent>();
                    if (achComp != null && level >= 7) // CornerCut: Hardcoded level and names
                    {
                        // Add 1 evasion per level for each earned achievement in the Kensai-line
                        int kensais = achComp.Achievements.Count(a => a.Id.Contains("NOARMOR") && a.Status == Achievements.AchievementStatus.Awarded);
                        evasion += level * kensais * 0.5;
                    }
                }

                if (weaponCount > 0)
                {
                    baseDamage = (combinedDmg / weaponCount);
                    cooldown = (combinedCD / weaponCount);
                    if (weaponCount >= 2)
                    {
                        apsMulti *= 1.2; // 20% AS bonus for dual wielding, just a dummy value for now
                    }
                }
                else
                {
                    var achComp = coordinator.GetEntities<AchievementsComponent>()?.FirstOrDefault()?.GetComponent<AchievementsComponent>();
                    if (achComp != null) // CornerCut: Hardcoded level and names
                    {
                        // Add 1 evasion per level for each earned achievement in the Kensai-line
                        int monks = achComp.Achievements.Count(a => a.Id.Contains("NOWEAPON") && a.Status == Achievements.AchievementStatus.Awarded);
                        baseDamage += level * monks * 0.05;
                    }
                }
            }

            var attackComp = player.GetComponent<AttackComponent>();
            if (attackComp != null)
            {
                dmgMulti *= level; // level bonus

                var rawDamage = baseDamage * dmgMulti;
                cooldown /= apsMulti;
                cooldown *= 1.0 + encumbrance / 100.0; // Encumbrance slows attack speed multiplicatively
                
                attackComp.RawDamage = rawDamage;
                // CornerCut(?): This makes the cooldown not reset in case of a weapon swap.
                // Theoretically exploitable by switching between fast and slow weapons
                // Potential TODO: Detect weapon swap
                if (cooldown != attackComp.Cooldown.Duration)
                    attackComp.Cooldown.ChangeDuration(cooldown, true);
            }

            var defenseComp = player.GetComponent<DefenseComponent>();
            if (defenseComp != null)
            {
                if (equipComp != null)
                {
                    defenseComp.Evasion = evasion; 
                    defenseComp.Armor = armor;
                }
            }
            
            coordinator.PostMessage(this, new StatsUpdatedMessage());
            if (m_initialFullUpdates > 0)
                m_initialFullUpdates--; 
        }

        static int GetAbilityLevel(Entity entity, string familyId)
        {
            return entity.GetComponent<AbilitiesComponent>()?.GetAbility(familyId)?.Level ?? -1;
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
