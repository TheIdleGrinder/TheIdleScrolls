﻿using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Web.CoreWrapper
{
    public class TimeLimit
    {
        public double Remaining { get; set; } = 0.0;
        public double Maximum { get; set; } = 0.0;
    }

    public class AccessibleAreas
    {
        public int MaxWilderness { get; set; } = 0;
        public List<DungeonRepresentation> Dungeons { get; set; } = new();
    }

    public class CharacterStats
    {
        public double Damage { get; set; } = 0.0;
        public double CooldownRemaining { get; set; } = 0.0;
        public double Cooldown { get; set; } = 0.0;
        public double Armor { get; set; } = 0.0;
        public double Evasion { get; set; } = 0.0;
        public double DefenseRating { get; set; } = 0.0;
        public double Encumbrance { get; set; } = 0.0;
    }

    public enum ExactEquipSlot { MainHand, OffHand, Chest, Head, Arms, Legs }

    public class Equipment
    {
        public IItemEntity? Hand { get; set; }
        public IItemEntity? OffHand { get; set; }
        public IItemEntity? Chest { get; set; }
        public IItemEntity? Head { get; set; }
        public IItemEntity? Arms { get; set; }
        public IItemEntity? Legs { get; set; }

        public List<IItemEntity> Items { get; private set; } = new();

        public Equipment()
        {
            Clear();
        }

        public void Clear()
        {
            Hand = OffHand = Chest = Head = Arms = Legs = null;
            Items = new();
        }

        public IItemEntity? GetItem(ExactEquipSlot slot)
        {
            return slot switch
            {
                ExactEquipSlot.MainHand => Hand,
                ExactEquipSlot.OffHand => OffHand,
                ExactEquipSlot.Chest => Chest,
                ExactEquipSlot.Head => Head,
                ExactEquipSlot.Arms => Arms,
                ExactEquipSlot.Legs => Legs,
                _ => null
            };
        }

        public void SetItems(List<IItemEntity> items)
        {
            Clear();
            foreach (var item in items)
            {
                bool firstSlot = true;
                foreach (var slot in item.Slots)
                {
                    var displayItem = item;// firstSlot ? item : item with { Rarity = -1 };
                    switch (slot)
                    {
                        case EquipmentSlot.Hand:
                            if (Hand == null)
                                Hand = displayItem;
                            else
                                OffHand = displayItem;
                            break;
                        case EquipmentSlot.Chest: Chest = displayItem; break;
                        case EquipmentSlot.Head: Head = displayItem; break;
                        case EquipmentSlot.Arms: Arms = displayItem; break;
                        case EquipmentSlot.Legs: Legs = displayItem; break;
                    }
                    firstSlot = false;
                }
            }
            Items = items;
        }
    }
}
