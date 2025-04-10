using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Properties;

namespace TheIdleScrolls_Core.Items
{
    public class ItemEntityWrapper : IItemEntity
    {
        public uint Id => Item.Id;
        public string Name => Item.GetName();
        public string Description => GenerateDescription();
        public List<EquipmentSlot> Slots => Item.GetRequiredSlots();
        public int Quality => Item.GetComponent<ItemQualityComponent>()?.Quality ?? 0;
        public int Value => Item.GetComponent<ItemValueComponent>()?.Value ?? 0;
        public (int Cost, double Duration) Refining => (Functions.CalculateCraftingCost(Item, Owner), 
                                                        Functions.CalculateRefiningDuration(Item, Owner));
        public bool Crafted => Item.GetComponent<ItemRefinableComponent>()?.Refined ?? false;

        public bool IsEquipped => Owner?.GetComponent<EquipmentComponent>()?.GetItems()?.Contains(Item) ?? false;
        
        
        private Entity Item { get; }
        private Entity? Owner { get; }

        public ItemEntityWrapper(Entity item, Entity? owner)
        {
            if (!item.IsItem())
            {
                throw new ArgumentException("Entity is not an item");
            }
            Item = item;
            Owner = owner;
        }

        private List<Entity> FindItemsToCompareTo()
        {
            if (IsEquipped)
            {
                return [];
            }

            bool IsComparable(Entity item)
            {
                if (item == Item)
                {
                    return false;
                }
                if (item.IsWeapon() != Item.IsWeapon()
                    || item.IsArmor() != Item.IsArmor()
                    || item.IsShield() != Item.IsShield())
                {
                    return false;
                }
                var itemSlots = item.GetRequiredSlots();
                return itemSlots.All(Slots.Contains) || Slots.All(itemSlots.Contains);
            }

            var items = Owner?.GetComponent<EquipmentComponent>()?.GetItems() ?? [];
            return items.Where(IsComparable)
                        .ToList();
        }

        private string GenerateDescription()
        {
            var comparableItems = FindItemsToCompareTo();
            string compToString(List<RelativeValue> results, bool higherIsBetter)
            {
                var relativeQualities = results.Select(r => r.ToRelativeQuality(higherIsBetter));
                if (results.Count == 0)
                {
                    return "";
                }
                return " [" + String.Join("", relativeQualities.Select(cr => cr switch 
                    { 
                        RelativeQuality.Better => '+', 
                        RelativeQuality.Worse => '-',
                        _ => '='
                    })) + "]";
            }

            var itemComp = Item.GetComponent<ItemComponent>()!;
            string description = $"Type: {itemComp.Blueprint.GetFamilyDescription().Name}";

            var equipComp = Item.GetComponent<EquippableComponent>();
            if (equipComp != null && equipComp.Slots.Count > 0)
            {
                List<string> slotStrings = new();
                var slotTypes = (EquipmentSlot[])Enum.GetValues(typeof(EquipmentSlot));
                foreach (var slot in slotTypes)
                {
                    int count = Slots.Count(s => s == slot);
                    if (count == 0)
                    {
                        continue;
                    }
                    slotStrings.Add((count > 1 ? $"{count}x " : "") + slot.ToString());
                }
                description += $"; Used Slot(s): {string.Join(", ", slotStrings)}";
            }

            description += $"; Skill: {itemComp.Blueprint.GetFamilyDescription().RelatedAbilityId.Localize()}";
            
            var levelComp = Item.GetComponent<LevelComponent>();
            if (levelComp != null)
            {
                description += $"; Drop Level: {levelComp.Level}";
            }
            description += "; ";

            var weaponComp = Item.GetComponent<WeaponComponent>();
            if (weaponComp != null)
            {
                description += $"; Damage: {weaponComp.Damage}{compToString(ItemComparator.CompareDamage(Item, comparableItems), true)}";
                description += $"; Attack Time: {weaponComp.Cooldown} s{compToString(ItemComparator.CompareCooldown(Item, comparableItems), false)}";
                description += $"; DPS: {(weaponComp.Damage / weaponComp.Cooldown):#.##}{compToString(ItemComparator.CompareDps(Item, comparableItems), true)}";
            }

            var armorComp = Item.GetComponent<ArmorComponent>();
            if (armorComp != null)
            {
                description += armorComp.Armor != 0.0 ? $"; Armor: {armorComp.Armor}{compToString(ItemComparator.CompareArmor(Item, comparableItems), true)}" : "";
                description += armorComp.Evasion != 0.0 ? $"; Evasion: {armorComp.Evasion}" : "";
            }

            if (equipComp != null)
            {
                description += equipComp.Encumbrance != 0.0 ? $"; Encumbrance: {equipComp.Encumbrance}%{compToString(ItemComparator.CompareEncumbrance(Item, comparableItems), false)}" : "";
            }

            var valueComp = Item.GetComponent<ItemValueComponent>();
            if (valueComp != null)
            {
                description += $"; ; Value: {valueComp.Value}c";
            }
            return description;
        }

    }
}
