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
        Hand
    }

    public class EquipmentComponent : IComponent
    {
        List<EquipmentSlot> m_freeSlots;
        List<Entity> m_items = new();

        public EquipmentComponent(List<EquipmentSlot> equipmentSlots)
        {
            m_freeSlots = equipmentSlots;
        }

        public EquipmentComponent()
        {
            m_freeSlots = new() { EquipmentSlot.Hand };
        }

        public bool EquipItem(Entity item)
        {
            if (!item.HasComponent<ItemComponent>())
            {
                throw new Exception($"Entity {item.GetName()} has no 'Item' component");
            }

            if (!CanEquipItem(item))
                return false;

            List<EquipmentSlot> requiredSlots = new() { EquipmentSlot.Hand }; // TODO: Hardcoded for now
            requiredSlots.ForEach(s => m_freeSlots.Remove(s));
            m_items.Add(item);
            return true;
        }

        public bool UnequipItem(Entity item)
        {
            bool removed = m_items.Remove(item);
            if (removed)
            {
                List<EquipmentSlot> requiredSlots = new() { EquipmentSlot.Hand }; // TODO: Hardcoded for now
                requiredSlots.ForEach(s => m_freeSlots.Add(s));
            }
            return removed;
        }

        public bool CanEquipItem(Entity item)
        {
            var itemComp = item.GetComponent<ItemComponent>();
            if (itemComp == null)
                return false;
            List<EquipmentSlot> requiredSlots = new() { EquipmentSlot.Hand }; // TODO: Hardcoded for now
            foreach (var slot in requiredSlots)
            {
                if (requiredSlots.Count(s => s == slot) > m_freeSlots.Count(s => s == slot))
                {
                    return false;
                }
            }
            return true;
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
