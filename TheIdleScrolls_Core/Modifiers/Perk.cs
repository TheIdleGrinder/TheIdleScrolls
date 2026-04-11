using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Skills;

namespace TheIdleScrolls_Core.Modifiers
{
    using ModifierGenerator = Func<int, Entity, World, Coordinator, List<Modifier>>;

    public enum UpdateTrigger
    {
        BattleStarted,
        BattleFinished,
        SkillStateChanged,
        LevelUp,
        AbilityIncreased,
        EquipmentChanged,
        AchievementUnlocked
    }

    // Perks are kind of a mess ever since I added levels to them: Before 'UpdateModifiers' would calculate the modifiers and store them in the perk.
    // That way, they would not have to be recalculated every time they are used. But now, the modifiers depend on the level of the perk, so they
    // become outdated at every level up. The Modifiers property can only return one set of modifiers, so how can one be sure that the modifiers
    // match the current level of the perk? So now the level is stored inside the perk as a reference, which is redudant with the level stored in
    // the PerksComponent. On the other hand, the PerksComponent has to differentiate between inactive and active perks, whereas the perk itself
    // should always store modifiers of atleast level 1. Otherwise, the modifiers that are displayed in the UI for inactive perks would be 0.
    public class Perk
    {
        public string Id { get; init; }
        public string Name { get; init; } = "??";
        public string Description { get; private set; } = "??";
        public List<string> Categories { get; set; } = [];
        public List<Modifier> Modifiers { get; private set; } = [];
        public HashSet<UpdateTrigger> UpdateTriggers { get; init; } = [];
        public bool Permanent { get; init; } = false;
        public int MaxLevel { get; init; } = 1;
        // Disable usage of modifiers for owner for perks that grant or modify active skills
        public bool ApplyModifiersToOwner { get; init; } = true;
        // Indicates that this perk is not supposed to show up in the GUI
        public bool Hidden { get; init; } = false;
        public ActiveSkillDefinition? Skill { get; init; } = null;

        // Corner cut: should this be part of the perk? Also, this is always at least 1, so that the modifiers are not 0
        public int CurrentLevel { get; private set; } = 1;
        public ModifierGenerator ModifiersFunc { get; private set; }
            = (int lvl, Entity e, World w, Coordinator c) => { return []; };
        public Func<int, IPerkCondition?> ConditionFunc { get; set; } = level => null;
        public IPerkCondition? ConditionForLevel(int level) => ConditionFunc(level);
        public Perk(string id, string name, string description, 
            HashSet<UpdateTrigger> updateTriggers,
            ModifierGenerator modifiersFunc)
        {
            Id = id;
            Name = name;
            Description = description;
            UpdateTriggers = updateTriggers;
            ModifiersFunc = modifiersFunc;
        }

        public void UpdateModifiers(int level, Entity owner, World world, Coordinator coordinator)
        {
            CurrentLevel = level;
            Modifiers = ModifiersFunc(level, owner, world, coordinator);
        }

        public Modifier? GetModifier(string modifierId)
        {
            return Modifiers.FirstOrDefault(m => m.Id == modifierId);
        }
    }
}
