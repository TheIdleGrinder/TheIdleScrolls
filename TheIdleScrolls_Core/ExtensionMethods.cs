using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core
{
    public static class EntityExtensions
    {
        public static string GetName(this Entity entity)
        {
            return entity.GetComponent<NameComponent>()?.Name ?? $"#{entity.Id}";
        }

        public static bool IsPlayer(this Entity entity)
        {
            return entity.HasComponent<PlayerComponent>();
        }

        public static bool IsMob(this Entity entity)
        {
            return entity.HasComponent<MobComponent>();
        }

        public static int GetLevel(this Entity entity)
        {
            return entity.GetComponent<LevelComponent>()?.Level ?? 0;
        }

        public static bool IsItem(this Entity entity)
        {
            return entity.HasComponent<ItemComponent>();
        }

        public static bool IsWeapon(this Entity entity)
        {
            return entity.HasComponent<WeaponComponent>();
        }

        public static bool IsArmor(this Entity entity)
        {
            return entity.HasComponent<ArmorComponent>();
        }

        public static List<EquipmentSlot> GetRequiredSlots(this Entity entity)
        {
            var equipComp = entity.GetComponent<EquippableComponent>();
            if (equipComp == null)
                return new();
            return new() { equipComp.Slot };
        }
    }
}
