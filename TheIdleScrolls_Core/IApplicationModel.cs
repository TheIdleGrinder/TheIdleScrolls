using MiniECS;
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

        public void SetPlayerDefense(double armor, double evasion);

        public void SetPlayerAbilities(List<AbilityRepresentation> abilities);

        public void SetMob(MobRepresentation mob);

        public void SetArea(string name, int level);

        public void SetTimeLimit(double remaining, double duration);

        public void SetAutoProceedStatus(bool enabled);

        public void SetFeatureAvailable(GameFeature feature, bool available);

        public void SetAccessibleAreas(int maxWilderness, List<DungeonRepresentation> dungeons);

        public void SetAchievements(List<AchievementRepresentation> visibleAchievements, int achievementCount);

        public void SetStatisticsReport(string report);

        public void DisplayMessage(string title, string message);

        public void AddLogMessages(List<string> messages);

        public HashSet<IMessage.PriorityLevel> GetRelevantMessagePriorties();
    }

    [Flags]
    public enum LoggerFlags
    {
        NoDamage = 1 << 0,
        NoXp = 1 << 1,
    }

    public enum GameFeature { Inventory, Armor, Abilities, Travel }

    public record ItemRepresentation(uint Id, string Name, string Description, List<EquipmentSlot> Slots);
    public record AbilityRepresentation(string Key, string Name, int Level, int XP, int TargetXP);
    public record MobRepresentation(uint Id, string Name, int Level, int HP, int HpMax);
    public record DungeonRepresentation(string Id, string Name, int Level, string Description);
    public record AchievementRepresentation(string Title, string Description, bool Earned);
}
