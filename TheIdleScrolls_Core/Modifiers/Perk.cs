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
        public string Id { get; set; }
        public string Name { get; set; } = "??";
        public string Description { get; set; } = "??";
        public List<Modifier> Modifiers { get; private set; } = new();
        public HashSet<UpdateTrigger> UpdateTriggers { get; private set; } = new();
        public Func<Entity, World, Coordinator, List<Modifier>> ModifiersFunc { get; private set; } 
            = (Entity e, World w, Coordinator c) => { return new(); };

        public Perk(string id, string name, string description, 
            List<Modifier> modifiers, 
            HashSet<UpdateTrigger> updateTriggers, 
            Func<Entity, World, Coordinator, List<Modifier>> modifiersFunc)
        {
            Id = id;
            Name = name;
            Description = description;
            Modifiers = modifiers;
            UpdateTriggers = updateTriggers;
            ModifiersFunc = modifiersFunc;
        }

        public void UpdateModifiers(Entity owner, World world, Coordinator coordinator)
        {
            Modifiers = ModifiersFunc(owner, world, coordinator);
        }
    }
}
