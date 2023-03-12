using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Components
{
    public enum EquipmentSlot
    {
        Hand, Chest, Head, Arms, Legs
    }

    public static class EquipSlot
    {
        public static EquipmentSlot Parse(string serialized)
        {
            return (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), serialized, true);
        }
    }

    public class EquipmentComponent : IComponent
    {
        readonly List<EquipmentSlot> m_freeSlots;
        readonly List<Entity> m_items = new();

        public EquipmentComponent(List<EquipmentSlot> equipmentSlots)
        {
            m_freeSlots = equipmentSlots;
        }

        public EquipmentComponent()
        {
            m_freeSlots = new() { 
                EquipmentSlot.Hand, 
                EquipmentSlot.Hand, 
                EquipmentSlot.Chest, 
                EquipmentSlot.Head, 
                EquipmentSlot.Arms, 
                EquipmentSlot.Legs 
            };
        }

        public bool EquipItem(Entity item)
        {
            if (!item.HasComponent<ItemComponent>())
                throw new Exception($"Entity {item.GetName()} is not an item");

            if (!item.HasComponent<EquippableComponent>())
                throw new Exception($"Entity {item.GetName()} is not equippable");

            if (!CanEquipItem(item))
                return false;

            List<EquipmentSlot> requiredSlots = item.GetRequiredSlots();
            requiredSlots.ForEach(s => m_freeSlots.Remove(s));
            m_items.Add(item);
            return true;
        }

        public bool UnequipItem(Entity item)
        {
            bool removed = m_items.Remove(item);
            if (removed)
            {
                List<EquipmentSlot> requiredSlots = item.GetRequiredSlots();
                requiredSlots.ForEach(s => m_freeSlots.Add(s));
            }
            return removed;
        }

        public bool UnequipItemFromSlot(EquipmentSlot slot)
        {
            var item = GetItemInSlot(slot);
            if (item != null)
            {
                return UnequipItem(item);
            }
            else
            {
                return false;
            }
        }

        public List<EquipmentSlot> GetMissingEquipmentSlotsForItem(Entity item)
        {
            List<EquipmentSlot> result = new();
            List<EquipmentSlot> requiredSlots = item.GetRequiredSlots();
            foreach (var slot in requiredSlots)
            {
                int missing = requiredSlots.Count(s => s == slot) - m_freeSlots.Count(s => s == slot);
                for (int i = 0; i < missing; i++)
                    result.Add(slot);
            }
            return result;
        }

        public bool CanEquipItem(Entity item)
        {
            var itemComp = item.GetComponent<ItemComponent>();
            if (itemComp == null)
                return false;

            // Block second shield
            // CornerCut: allow more shields for characters with more arms
            if (item.IsShield() && m_items.Any(i => i.IsShield()))
                return false;

            List<EquipmentSlot> requiredSlots = item.GetRequiredSlots();
            foreach (var slot in requiredSlots)
            {
                if (requiredSlots.Count(s => s == slot) > m_freeSlots.Count(s => s == slot))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Find first item in a slot of the give type
        /// </summary>
        /// <param name="slot">Equipment slot to look in</param>
        /// <returns>Item if any is eqipped in slot</returns>
        public Entity? GetItemInSlot(EquipmentSlot slot)
        {
            foreach (var item in m_items)
            {
                if (item.GetComponent<EquippableComponent>()?.Slots.Contains(slot) ?? false)
                {
                    return item;
                }
            }
            return null;
        }

        public List<Entity> GetItems()
        {
            return m_items.ToList();
        }

        public List<EquipmentSlot> GetFreeSlots()
        {
            return m_freeSlots.ToList();
        }
    }
}
