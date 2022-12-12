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

        public LoggerFlags GetLogSettings()
        {
            return 0;
        }

        public void SetArea(string name, int level) { }

        public void SetAutoProceedStatus(bool enabled) { }

        public void SetFeatureAvailable(GameFeature feature, bool available) { }

        public void SetMob(MobRepresentation mob) { }

        public void SetPlayerAbilities(List<AbilityRepresentation> abilities) { }

        public void SetPlayerAttack(double damage, double cooldown, double remainingCooldown) { }

        public void SetPlayerCharacter(uint id, string name) { }

        public void SetPlayerItems(List<ItemRepresentation> inventory, List<ItemRepresentation> equipment) { }

        public void SetPlayerLevel(int level, int currentXP, int targetXP) { }

        public void SetTimeLimit(double remaining, double duration) { }
    }
}
