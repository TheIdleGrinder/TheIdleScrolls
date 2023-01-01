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
        bool m_firstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == 0)
                m_player = coordinator.GetEntities<PlayerComponent>().FirstOrDefault()?.Id ?? 0;

            bool doUpdate = m_firstUpdate
                || coordinator.MessageTypeIsOnBoard<LevelUpSystem.LevelUpMessage>()
                || coordinator.MessageTypeIsOnBoard<ItemMovedMessage>()
                || coordinator.MessageTypeIsOnBoard<AbilityImprovedMessage>();

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
                        combinedCD += weaponComp.Cooldown;
                        weaponCount++;

                        int abilityLvl = GetAbilityLevel(player, itemComp.FamilyName, world.ItemKingdom);
                        if (abilityLvl != -1)
                        {
                            apsMulti *= (0.5 + 0.02 * abilityLvl); // CornerCut: Make this less 'hidden'
                        }
                    }

                    if (itemComp != null && armorComp != null)
                    {
                        var localArmor = armorComp.Armor;
                        var localEvasion = armorComp.Evasion;
                        armorCount++;

                        int abilityLvl = GetAbilityLevel(player, itemComp.FamilyName, world.ItemKingdom);
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

                if (weaponCount > 0)
                {
                    baseDamage = (combinedDmg / weaponCount);
                    cooldown = (combinedCD / weaponCount);
                    if (weaponCount >= 2)
                    {
                        apsMulti *= 1.2; // 20% AS bonus for dual wielding, just a dummy value for now
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
            m_firstUpdate = false;
        }

        int GetAbilityLevel(Entity entity, string itemFamilyName, ItemKingdomDescription itemKingdom)
        {
            string? familyId = itemKingdom.GetFamilyIdFromItemFamilyName(itemFamilyName);
            if (familyId != null)
            {
                return entity.GetComponent<AbilitiesComponent>()?.GetAbility(familyId)?.Level ?? -1;
            }
            return -1;
        }
    }

    record AttackStats(double Damage, double Cooldwon, List<string> WeaponFamilyIds);

    public class StatsUpdatedMessage : IMessage
    {
        string IMessage.BuildMessage()
        {
            return $"Player stats updated";
        }
    }

}
