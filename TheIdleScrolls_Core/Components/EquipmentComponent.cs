using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;

namespace TheIdleScrolls_Core.Components
{
    public static class EquipSlot
    {
        public static EquipmentSlot Parse(string serialized)
        {
            return (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), serialized, true);
        }
    }

    public class EquipmentComponent : IComponent
    {
        const uint FreeSlotId = uint.MaxValue;

        readonly Dictionary<EquipmentSlot, uint[]> SlotUsage = [];
        readonly Dictionary<uint, Entity> Items = [];
        double Encumbrance = 0.0;

        public EquipmentComponent(List<EquipmentSlot> equipmentSlots)
        {
            SetupSlots(equipmentSlots);
        }

        public EquipmentComponent()
        {
            SetupSlots([
                EquipmentSlot.Hand, 
                EquipmentSlot.Hand, 
                EquipmentSlot.Chest, 
                EquipmentSlot.Head, 
                EquipmentSlot.Arms, 
                EquipmentSlot.Legs 
            ]);
        }

        void SetupSlots(List<EquipmentSlot> equipmentSlots)
        {
            SlotUsage.Clear();
            foreach (var slot in equipmentSlots.ToHashSet())
            {
                SlotUsage[slot] = Enumerable.Repeat(FreeSlotId, equipmentSlots.Count(s => s == slot)).ToArray();
            }
        }

        public List<EquipmentSlot> FreeSlots { get 
            {
                List<EquipmentSlot> result = [];
                foreach (var key in SlotUsage.Keys)
                {
                    foreach (var slot in SlotUsage[key])
                    {
                        if (slot == FreeSlotId)
                            result.Add(key);
                    }
                }
                return result;
            } 
        }

        public double TotalEncumbrance => Encumbrance;

        public bool EquipItem(Entity item)
        {
            if (!item.HasComponent<ItemComponent>())
                throw new Exception($"Entity {item.GetName()} is not an item");

            if (!item.HasComponent<EquippableComponent>())
                throw new Exception($"Entity {item.GetName()} is not equippable");

            if (!CanEquipItem(item))
                return false;

            List<EquipmentSlot> requiredSlots = item.GetRequiredSlots();
            
            Items[item.Id] = item;

            foreach (var slot in requiredSlots)
            {
                if (SlotUsage.TryGetValue(slot, out uint[]? slots))
                {
                    if (TakesSlotsBackwards(item))
                    {
                        for (int i = slots.Length - 1; i >= 0; i--)
                        {
                            if (slots[i] == FreeSlotId)
                            {
                                slots[i] = item.Id;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (slots[i] == FreeSlotId)
                            {
                                slots[i] = item.Id;
                                break;
                            }
                        }
                    }
                }
            }

            UpdateEncumbrance();
            return true;
        }

        public bool UnequipItem(Entity item, bool moveItemsUp = true)
        {
            bool removable = Items.ContainsKey(item.Id);
            if (removable)
            {
                foreach (var slot in item.GetRequiredSlots())
                {
                    if (SlotUsage.TryGetValue(slot, out uint[]? slots))
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (slots[i] == item.Id)
                            {
                                slots[i] = FreeSlotId;
                                if (!moveItemsUp)
                                    break;
                                for (int j = i + 1; j < slots.Length; j++)
                                {
                                    if (slots[j] != FreeSlotId && !TakesSlotsBackwards(GetItemInSlot(slot, j)!))
                                    {
                                        slots[i] = slots[j];
                                        slots[j] = FreeSlotId;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            Items.Remove(item.Id);
            UpdateEncumbrance();
            return removable;
        }

        public List<EquipmentSlot> GetMissingEquipmentSlotsForItem(Entity item)
        {
            List<EquipmentSlot> result = [];
            List<EquipmentSlot> requiredSlots = item.GetRequiredSlots();
            foreach (var slot in requiredSlots.ToHashSet())
            {
                int missing = requiredSlots.Count(s => s == slot) - FreeSlots.Count(s => s == slot);
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
            if (item.IsShield() && Items.Values.Any(i => i.IsShield()))
                return false;

            List<EquipmentSlot> requiredSlots = item.GetRequiredSlots();
            foreach (var slot in requiredSlots)
            {
                if (requiredSlots.Count(s => s == slot) > FreeSlots.Count(s => s == slot))
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
        public Entity? GetItemInSlot(EquipmentSlot slotType)
        {
            if (!SlotUsage.TryGetValue(slotType, out uint[]? slots))
                return null;
            foreach (var itemId in slots)
            {
                if (itemId != FreeSlotId)
                    return Items[itemId];
            }
            return null;
        }

        public List<Entity> GetItems()
        {
            List<uint> ids = [];
            foreach (var slot in SlotUsage.Keys)
            {
                foreach (var itemId in SlotUsage[slot])
                {
                    if (itemId != FreeSlotId && !ids.Contains(itemId))
                        ids.Add(itemId);
                }
            }
            return ids.Select(id => Items[id]).ToList();
        }

        public Entity? GetItemInSlot(EquipmentSlot slot, int index)
        {
            return SlotUsage.TryGetValue(slot, out uint[]? slots) && index < slots.Length
                ? (Items.TryGetValue(slots[index], out Entity? item) ? item : null)
                : null;
        }

        public List<EquipmentSlot> GetFreeSlots()
        {
            return FreeSlots;
        }

        void UpdateEncumbrance()
        {
            Encumbrance = Items.Values
                .Select(i => i.GetComponent<EquippableComponent>()?.Encumbrance ?? 0.0)
                .Sum();
        }

        static bool TakesSlotsBackwards(Entity item)
        {
            return item.IsShield(); // Shields occupy the last available slot first
        }
    }
}
