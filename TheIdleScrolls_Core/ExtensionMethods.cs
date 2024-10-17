using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;

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

        public static string GetBlueprintCode(this Entity entity)
        {
            return entity.GetComponent<ItemComponent>()?.Blueprint.ToString() ?? String.Empty;
        }

        public static ItemBlueprint? GetBlueprint(this Entity entity)
        {
            return entity.GetComponent<ItemComponent>()?.Blueprint;
        }

        public static bool IsWeapon(this Entity entity)
        {
            return entity.HasComponent<WeaponComponent>();
        }

        public static bool IsArmor(this Entity entity)
        {
            return entity.HasComponent<ArmorComponent>();
        }

        public static bool IsShield(this Entity entity)
        {
            return entity.IsArmor() 
                && entity.GetRequiredSlots().Contains(EquipmentSlot.Hand);
        }

        public static List<EquipmentSlot> GetRequiredSlots(this Entity entity)
        {
            var equipComp = entity.GetComponent<EquippableComponent>();
            if (equipComp == null)
                return new();
            return equipComp.Slots;
        }

        public static bool HasTag(this Entity entity, string tag)
        {
            return entity.GetComponent<TagsComponent>()?.HasTag(tag) ?? false;
        }

        public static List<string> GetTags(this Entity entity)
        {
            return entity.GetComponent<TagsComponent>()?.ListTags() ?? new();
        }

        public static double ApplyAllApplicableModifiers(this Entity entity, double baseValue, IEnumerable<string> tags)
        {
            return entity.GetComponent<ModifierComponent>()?.ApplyApplicableModifiers(baseValue, tags) ?? baseValue;
        }
    }

    public static class StringExtensions
    {
        public static string Localize(this string key)
        {
            return Properties.LocalizedStrings.ResourceManager.GetString(key) ?? key;
        }
    }
}
