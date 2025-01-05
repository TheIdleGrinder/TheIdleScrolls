using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Modifiers
{
    public enum UpdateTrigger
    {
        BattleStarted,
        BattleFinished,
        AttackPerformed,
        LevelUp,
        AbilityIncreased,
        EquipmentChanged,
        AchievementUnlocked
    }

    public class Perk
    {
        public string Id { get; init; }
        public string Name { get; init; } = "??";
        public string Description { get; private set; } = "??";
        public List<Modifier> Modifiers { get; private set; } = [];
        public HashSet<UpdateTrigger> UpdateTriggers { get; init; } = [];
        public bool Permanent { get; init; } = false;
        public Func<Entity, World, Coordinator, List<Modifier>> ModifiersFunc { get; private set; } 
            = (Entity e, World w, Coordinator c) => { return []; };

        public Perk(string id, string name, string description, 
            HashSet<UpdateTrigger> updateTriggers, 
            Func<Entity, World, Coordinator, List<Modifier>> modifiersFunc)
        {
            Id = id;
            Name = name;
            Description = description;
            UpdateTriggers = updateTriggers;
            ModifiersFunc = modifiersFunc;
        }

        public void UpdateModifiers(Entity owner, World world, Coordinator coordinator)
        {
            Modifiers = ModifiersFunc(owner, world, coordinator);
        }
    }
}
