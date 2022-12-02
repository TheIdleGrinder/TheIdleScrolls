using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Systems
{
    public class StatUpdateSystem : AbstractSystem
    {
        uint m_player = 0;
        bool m_firstUpdate = true;

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            if (m_player == 0)
                m_player = coordinator.GetEntities().Where(e => e.HasComponent<PlayerComponent>()).FirstOrDefault()?.Id ?? 0;

            bool doUpdate = m_firstUpdate
                || coordinator.MessageTypeIsOnBoard<LevelUpSystem.LevelUpMessage>()
                || coordinator.MessageTypeIsOnBoard<ItemMovedMessage>()
                || coordinator.MessageTypeIsOnBoard<AbilityImprovedMessage>();

            if (!doUpdate)
                return;

            var player = coordinator.GetEntity(m_player);
            if (player == null)
                return;

            var attackComp = player.GetComponent<AttackComponent>();
            if (attackComp == null)
                return;

            int level = player.GetComponent<LevelComponent>()?.Level ?? 1;

            double baseDamage = 2.0;
            double cooldown = 1.0;

            double dmgMulti = 1.0;
            double apsMulti = 1.0;

            var equipComp = player.GetComponent<EquipmentComponent>();
            if (equipComp != null)
            {
                double combinedDmg = 0.0;
                double combinedCD = 0.0;
                int weaponCount = 0;
                foreach (var item in equipComp.GetItems())
                {
                    var weaponComp = item.GetComponent<WeaponComponent>();
                    if (weaponComp != null)
                    {
                        combinedDmg += weaponComp.Damage;
                        combinedCD += weaponComp.Cooldown;
                        weaponCount++;

                        string? classId = world.ItemKingdom.GetFamilyIdFromItemFamilyName(weaponComp.Family);
                        if (classId != null)
                        {
                            var abilityValue = player.GetComponent<AbilitiesComponent>()?.GetAbility(classId)?.Level ?? -1;
                            if (abilityValue != -1)
                            {
                                apsMulti *= (0.5 + 0.02 * abilityValue); // CornerCut: Make this less 'hidden'
                            }
                        }
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
      
            dmgMulti *= level; // level bonus

            var rawDamage = baseDamage * dmgMulti;
            cooldown /= apsMulti;
            attackComp.RawDamage = rawDamage;
            // CornerCut(?): Reset cooldown if the duration changed
            // implies weapon change => what if other items change the cooldown? A single "skipped" attack is probably fine
            if (cooldown != attackComp.Cooldown.Duration)
                attackComp.Cooldown = new Utility.Cooldown(cooldown);
            
            coordinator.PostMessage(this, new StatsUpdatedMessage());
            m_firstUpdate = false;
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
