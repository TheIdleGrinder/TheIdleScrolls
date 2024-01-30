using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Messages;

namespace TheIdleScrolls_Core
{
    public delegate void PlayerCharacterChangedHandler(CharacterRepresentation character);
    public delegate void CharacterXpChangedHandler(int xp, int target);
    public delegate void ItemsChangedHandler(List<ItemRepresentation> items);
    public delegate void EncumbranceChangedHandler(double encumbrance);
    public delegate void CoinsChangedHandler(int coins);
    public delegate void OffenseChangedHandler(double damage, double cooldown, double remainingCooldown);
    public delegate void DefenseChangedHandler(double armor, double evasion);
    public delegate void AbilitiesChangedHandler(List<AbilityRepresentation> abilities);
    public delegate void PerksChangedHandler(List<PerkRepresentation> perks);
    public delegate void ModifiersChangedHandler(List<string> modifiers);
    public delegate void MobChangedHandler(MobRepresentation mob);
    public delegate void AreaChangedHandler(string name, int level, bool isDungeon);
    public delegate void AutoProceedStateChangedHandler(bool autoProceed);
    public delegate void TimeLimitChangedHandler(double remaining, double duration);
    public delegate void FeatureAvailabilityChangedHandler(GameFeature feature, bool available);
    public delegate void AccessibleAreasChangedHandler(int maxWilderness, List<DungeonRepresentation> dungeons);
    public delegate void AchievementsChangedHandler(List<AchievementRepresentation> achievements, int achievementCount);
    public delegate void StatReportChangedHandler(string report);
    public delegate void DisplayMessageHandler(string title, string message);
    public delegate void DialogueMessageHandler(DialogueMessage message);
    public delegate void NewLogMessagesHandler(List<string> messages);

    public interface IGameEventEmitter
    {
        event PlayerCharacterChangedHandler PlayerCharacterChanged;
        event CharacterXpChangedHandler PlayerXpChanged;
        event ItemsChangedHandler PlayerInventoryChanged;
        event ItemsChangedHandler PlayerEquipmentChanged;
        event EncumbranceChangedHandler PlayerEncumbranceChanged;
        event CoinsChangedHandler PlayerCoinsChanged;
        event OffenseChangedHandler PlayerOffenseChanged;
        event DefenseChangedHandler PlayerDefenseChanged;
        event AbilitiesChangedHandler PlayerAbilitiesChanged;
        event PerksChangedHandler PlayerPerksChanged;
        event ModifiersChangedHandler PlayerModifiersChanged;
        event MobChangedHandler MobChanged;
        event AreaChangedHandler PlayerAreaChanged;
        event AutoProceedStateChangedHandler PlayerAutoProceedStateChanged;
        event TimeLimitChangedHandler TimeLimitChanged;
        event FeatureAvailabilityChangedHandler FeatureAvailabilityChanged;
        event AccessibleAreasChangedHandler AccessibleAreasChanged;
        event AchievementsChangedHandler AchievementsChanged;
        event StatReportChangedHandler StatReportChanged;
        event DisplayMessageHandler DisplayMessageReceived;
        event DialogueMessageHandler DialogueMessageReceived;
        event NewLogMessagesHandler NewLogMessages;
    }
}
