using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrollsApp
{
    internal class UpdateMainWindowSystem : AbstractSystem
    {
        WeakReference<MainWindow> _mainWindow;
        uint m_playerEntity = 0;
        bool m_firstUpdate = true; // Ensure that full update is executed on first iteration
        double m_abilitiesUpdateTimer = 0.0;

        public UpdateMainWindowSystem(MainWindow mainWindow)
        {
            _mainWindow = new WeakReference<MainWindow>(mainWindow);
        }

        public override void Update(World world, Coordinator coordinator, double dt)
        {
            MainWindow? mainWindow;
            if (!_mainWindow.TryGetTarget(out mainWindow) || mainWindow == null)
                return;

            if (m_playerEntity == 0)
                m_playerEntity = coordinator.GetEntities().Where(e => e.HasComponent<PlayerComponent>()).First().Id;
            Entity player = coordinator.GetEntity(m_playerEntity) ?? throw new Exception("Player not found!");
            mainWindow.SetCharacter(player.Id, player.GetName());

            // Update attack cooldown
            var attackComp = player.GetComponent<AttackComponent>();
            if (attackComp != null)
            {
                mainWindow.SetAttackCooldown(attackComp.Cooldown.Duration, attackComp.Cooldown.Remaining);
            }

            // Update level
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<LevelUpSystem.LevelUpMessage>())
            {
                var level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                mainWindow.SetCharacterLevel(level);
            }

            // Update XP
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<XpGainMessage>())
            {
                var level = player.GetComponent<LevelComponent>()?.Level ?? 0;
                var xpCurrent = player.GetComponent<XpGainerComponent>()?.Current ?? 0;
                var xpTarget = player.GetComponent<XpGainerComponent>()?.TargetFunction(level) ?? 0;
                mainWindow.SetCharacterXP(xpCurrent, xpTarget);
            }

            // Update stats
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<StatsUpdatedMessage>())
            {
                if (attackComp != null)
                {
                    var dps = attackComp.RawDamage / attackComp.Cooldown.Duration;
                    mainWindow.SetAttackDamage(attackComp.RawDamage, dps);
                }
            }

            // Update inventory and equipment
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<ItemMovedMessage>()
                || coordinator.MessageTypeIsOnBoard<ItemReceivedMessage>())
            {
                var inventoryComp = player.GetComponent<InventoryComponent>();
                var equipmentComp = player.GetComponent<EquipmentComponent>();
                if (inventoryComp != null && equipmentComp != null)
                {
                    var invItems = new List<InventoryItem>();
                    foreach (var item in inventoryComp.GetItems())
                    {
                        var invItem = InvItemFromEntity(item);
                        if (invItem != null)
                            invItems.Add(invItem);
                    }
                    mainWindow.SetInventory(invItems);

                    Dictionary<string, InventoryItem> equipment = new();
                    foreach (var item in equipmentComp.GetItems())
                    {
                        var invItem = InvItemFromEntity(item);
                        if (invItem != null)
                            equipment.Add(invItem.Slot, invItem);
                    }
                    mainWindow.SetEquipment(equipment);
                }
            }

            // Update abilities
            m_abilitiesUpdateTimer += dt;
            if (m_firstUpdate || m_abilitiesUpdateTimer > 1.0)
            {
                var abilitiesComp = player.GetComponent<AbilitiesComponent>();
                if (abilitiesComp != null)
                {
                    var abilities = abilitiesComp.GetAbilities();
                    var representations = abilities.Select(a => 
                        new AbilityRepresentation(a.Key, a.Name, a.Level, 
                            $"{(1.0 * a.XP / a.TargetXP):0 %}")
                    );
                    mainWindow.SetAbilities(representations.ToList());
                }
                m_abilitiesUpdateTimer = 0.0;
            }

            // React to tutorial messages
            if (m_firstUpdate || coordinator.MessageTypeIsOnBoard<TutorialMessage>())
            {
                mainWindow.SetAreaAvailable(MainWindow.Area.Inventory, player.HasComponent<InventoryComponent>());
            }

            mainWindow.SetAreaLevel(world.AreaLevel);

            Entity? mob = coordinator.GetEntities().Where(e => e.HasComponent<MobComponent>()).FirstOrDefault();
            if (mob != null)
            {
                var mobName = mob.GetComponent<NameComponent>()?.Name ?? "<error>";
                var mobLevel = mob.GetComponent<LevelComponent>()?.Level ?? 0;
                var mobHp = mob.GetComponent<LifePoolComponent>()?.Current ?? 0;
                var mobHpMax = mob.GetComponent<LifePoolComponent>()?.Maximum ?? 0;
                mainWindow.SetMob(mobName, mobLevel);
                mainWindow.SetMobHP(mobHp, mobHpMax);
            }

            m_firstUpdate = false;
        }

        static InventoryItem? InvItemFromEntity(Entity item)
        {
            var weaponComp = item.GetComponent<WeaponComponent>();
            if (weaponComp == null)
                return null;
            return new InventoryItem()
            {
                Id = item.Id,
                Slot = "Hand",
                Damage = weaponComp.Damage.ToString(),
                Speed = weaponComp.Cooldown.ToString(),
                Class = weaponComp.Class,
                Name = item.GetName()
            };
        }
    }
}
