﻿using MiniECS;
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

        public void SetPlayerCharacter(CharacterRepresentation character)
        {
            GetTargetOrNull()?.SetCharacter(character.Id, character.Name, character.Class, character.Level);
        }

        public void SetPlayerXP(int currentXP, int targetXP)
        {
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

        public void SetArea(string name, int level, bool isDungeon)
        {
            GetTargetOrNull()?.SetArea(name, level, isDungeon);
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
            GetTargetOrNull()?.AddLogMessages(messages);
        }

        public HashSet<IMessage.PriorityLevel> GetRelevantMessagePriorties()
        {
            return new() { IMessage.PriorityLevel.VeryHigh, IMessage.PriorityLevel.High, IMessage.PriorityLevel.Medium };
        }

        public void SetPlayerDefense(double armor, double evasion)
        {
            GetTargetOrNull()?.SetDefenses(armor, evasion);
        }

        public void DisplayMessage(string title, string message)
        {
            GetTargetOrNull()?.ShowMessageBox(title, message);
        }

        public void SetAccessibleAreas(int maxWilderness, List<DungeonRepresentation> dungeons)
        {
            GetTargetOrNull()?.SetAccessibleAreas(maxWilderness, dungeons);
        }

        public void SetAchievements(List<AchievementRepresentation> visibleAchievements, int achievementCount)
        {
            GetTargetOrNull()?.SetAchievements(visibleAchievements, achievementCount);
        }

        public void SetStatisticsReport(string report)
        {
            GetTargetOrNull()?.SetStatisticsReport(report);
        }

        public void SetPlayerCoins(int coins)
        {
            GetTargetOrNull()?.SetPlayerCoins(coins);
        }
    }
}
