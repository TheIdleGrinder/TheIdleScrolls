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
        public HashSet<IMessage.PriorityLevel> GetRelevantMessagePriorties();
    }

    public enum GameFeature { Inventory, Armor, Abilities, Travel, Crafting }

    public record CharacterRepresentation(uint Id, string Name, string Class, int Level);
    public record ItemRepresentation(uint Id, string Name, string Description, 
                                        List<EquipmentSlot> Slots, int Rarity, int Value,
                                        int ReforgingCost, bool Crafted);
    public record AbilityRepresentation(string Key, string Name, int Level, int XP, int TargetXP);
    public record PerkRepresentation(string Name, string Description, List<string> Modifiers);
    public record MobRepresentation(uint Id, string Name, int Level, int HP, int HpMax);
    public record DungeonRepresentation(string Id, string Name, int Level, string Description, int Rarity);
    public record AchievementRepresentation(string Title, string Description, bool Earned);
    public record AreaRepresentation(string Name, int Level, bool IsDungeon);
}
