using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrollsApp
{
    internal class MainWindowUpdater : IApplicationModel
    {
        readonly WeakReference<MainWindow> _mainWindow;

        public MainWindowUpdater(MainWindow mainWindow)
        {
            _mainWindow = new WeakReference<MainWindow>(mainWindow);
        }

        MainWindow? GetTargetOrNull()
        {
            _mainWindow.TryGetTarget(out MainWindow? mainWindow);
            return mainWindow;
        }

        public void SetPlayerCharacter(uint id, string name)
        {
            GetTargetOrNull()?.SetCharacter(id, name);
        }

        public void SetPlayerLevel(int level, int currentXP, int targetXP)
        {
            GetTargetOrNull()?.SetCharacterLevel(level);
            GetTargetOrNull()?.SetCharacterXP(currentXP, targetXP);
        }

        public void SetPlayerItems(List<ItemRepresentation> inventory, List<ItemRepresentation> equipment)
        {
            GetTargetOrNull()?.SetEquipment(equipment);
            GetTargetOrNull()?.SetInventory(inventory);
        }

        public void SetPlayerAttack(double damage, double cooldown, double remainingCooldown)
        {
            GetTargetOrNull()?.SetAttackDamage(damage, damage / cooldown);
            GetTargetOrNull()?.SetAttackCooldown(cooldown, remainingCooldown);
        }

        public void SetPlayerAbilities(List<TheIdleScrolls_Core.AbilityRepresentation> abilities)
        {
            var reps = abilities.Select(a => new AbilityRepresentation(a.Key, 
                a.Name, 
                a.Level, 
                $"{(1.0 * a.XP / a.TargetXP):0 %}")).ToList();
            GetTargetOrNull()?.SetAbilities(reps);
        }

        public void SetMob(MobRepresentation mob)
        {
            GetTargetOrNull()?.SetMob(mob.Name, mob.Level);
            GetTargetOrNull()?.SetMobHP(mob.HP, mob.HpMax);
        }

        public void SetArea(string name, int level)
        {
            GetTargetOrNull()?.SetAreaLevel(level);
        }

        public void SetTimeLimit(double remaining, double duration)
        {
            GetTargetOrNull()?.UpdateTimeLimit(remaining, duration);
        }

        public void SetAutoProceedStatus(bool enabled)
        {
            GetTargetOrNull()?.SetAutoProceed(enabled);
        }

        public void SetFeatureAvailable(GameFeature feature, bool available)
        {
            GetTargetOrNull()?.SetFeatureAvailable(feature, available);
        }

        public void AddLogMessages(List<string> messages)
        {
            ;
        }

        public LoggerFlags GetLogSettings()
        {
            return LoggerFlags.NoDamage | LoggerFlags.NoXp;
        }

        public void SetPlayerDefense(double armor, double evasion)
        {
            GetTargetOrNull()?.SetDefenses(armor, evasion);
        }

        public void DisplayMessage(string title, string message)
        {
            GetTargetOrNull()?.ShowMessageBox(title, message);
        }
    }
}
