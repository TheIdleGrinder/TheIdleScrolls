using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core;

namespace ConsoleRunner
{
    internal class ConsoleUpdater : IApplicationModel
    {
        public void AddLogMessages(List<string> messages)
        {
            messages.ForEach(m => Console.WriteLine(m));
        }
        public HashSet<IMessage.PriorityLevel> GetRelevantMessagePriorties()
        {
            return new()
            {
                IMessage.PriorityLevel.VeryHigh,
                IMessage.PriorityLevel.High,
                IMessage.PriorityLevel.Medium,
                IMessage.PriorityLevel.Low,
                IMessage.PriorityLevel.VeryLow,
                IMessage.PriorityLevel.Debug
            };
        }

        public void SetArea(string name, int level, bool isDungeon) { }

        public void SetAutoProceedStatus(bool enabled) { }

        public void SetFeatureAvailable(GameFeature feature, bool available) { }

        public void SetMob(MobRepresentation mob) { }

        public void SetPlayerAbilities(List<AbilityRepresentation> abilities) { }

        public void SetPlayerAttack(double damage, double cooldown, double remainingCooldown) { }

        public void SetPlayerCharacter(CharacterRepresentation character) { }

        public void SetPlayerDefense(double armor, double evasion) { }

        public void SetPlayerItems(List<ItemRepresentation> inventory, List<ItemRepresentation> equipment) { }

        public void SetPlayerXP(int currentXP, int targetXP) { }

        public void SetTimeLimit(double remaining, double duration) { }

        public void DisplayMessage(string title, string message) { }

        public void SetAccessibleAreas(int maxWilderness, List<DungeonRepresentation> dungeons) { }

        public void SetAchievements(List<AchievementRepresentation> achievements, int count) { }

        public void SetStatisticsReport(string report) { }

        public void SetPlayerCoins(int coins) { }
    }
}
