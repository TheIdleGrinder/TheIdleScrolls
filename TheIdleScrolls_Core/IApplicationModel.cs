using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core
{
    public interface IApplicationModel
    {
        public void SetPlayerCharacter(uint id, string name);

        public void SetPlayerLevel(int level, int currentXP, int targetXP);

        public void SetPlayerItems(List<ItemRepresentation> inventory, List<ItemRepresentation> equipment);

        public void SetPlayerAttack(double damage, double cooldown, double remainingCooldown);

        public void SetPlayerAbilities(List<AbilityRepresentation> abilities);

        public void SetMob(MobRepresentation mob);

        public void SetArea(string name, int level);

        public void SetTimeLimit(double remaining, double duration);

        public void SetAutoProceedStatus(bool enabled);

        public void SetFeatureAvailable(GameFeature feature, bool available);

        public void AddLogMessages(List<string> messages);

        public LoggerFlags GetLogSettings();
    }

    [Flags]
    public enum LoggerFlags
    {
        NoDamage = 1 << 0,
        NoXp = 1 << 1,
    }

    public enum GameFeature { Inventory }

    public record ItemRepresentation(uint id, string name, string description, List<EquipmentSlot> slots);
    public record AbilityRepresentation(string key, string name, int level, int xp, int targetXp);
    public record MobRepresentation(uint id, string name, int level, int hp, int hpMax);
}
