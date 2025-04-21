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
        public string Type => Item.GetComponent<ItemComponent>()?.Blueprint.GetFamilyDescription().Name ?? "";
        public string RelatedAbility => Item.GetComponent<ItemComponent>()?.Blueprint.GetFamilyDescription().RelatedAbilityId.Localize() ?? "";
        public string Description => GenerateDescription();
        public List<EquipmentSlot> Slots => Item.GetRequiredSlots();
        public double Encumbrance => Item.GetComponent<EquippableComponent>()?.Encumbrance ?? 0.0;
        public int DropLevel => Item.GetLevel();
        public int Quality => Item.GetComponent<ItemQualityComponent>()?.Quality ?? 0;
        public int Value => Item.GetComponent<ItemValueComponent>()?.Value ?? 0;
        public (int Cost, double Duration) Refining => (Functions.CalculateCraftingCost(Item, Owner), 
                                                        Functions.CalculateRefiningDuration(Item, Owner));
        public bool Crafted => Item.GetComponent<ItemRefinableComponent>()?.Refined ?? false;
        public WeaponGenus? WeaponAspect 
        { 
            get 
            { 
                var weaponComp = Item.GetComponent<WeaponComponent>();
                return weaponComp != null ? new WeaponGenus(weaponComp.Damage, weaponComp.Cooldown) : null;
            } 
        }
        public ArmorGenus? ArmorAspect
        {
            get
            {
                var armorComp = Item.GetComponent<ArmorComponent>();
                return armorComp != null ? new ArmorGenus(armorComp.Armor, armorComp.Evasion) : null;
            }
        }

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

            var itemComp = Item.GetComponent<ItemComponent>()!;
            string description = $"Type: {Type}";

            if (Slots.Count > 0)
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

            description += $"; Skill: {RelatedAbility}";
            
            var levelComp = Item.GetComponent<LevelComponent>();
            if (levelComp != null)
            {
                description += $"; Drop Level: {DropLevel}";
            }
            description += "; ";

            if (WeaponAspect != null)
            {
                description += $"; Damage: {WeaponAspect.BaseDamage}";
                description += $"; Attack Time: {WeaponAspect.BaseCooldown} s";
                description += $"; DPS: {(WeaponAspect.BaseDamage / WeaponAspect.BaseCooldown):#.##}";
            }

            if (ArmorAspect != null)
            {
                description += ArmorAspect.BaseArmor != 0.0 ? $"; Armor: {ArmorAspect.BaseArmor}" : "";
                description += ArmorAspect.BaseEvasion != 0.0 ? $"; Evasion: {ArmorAspect.BaseEvasion}" : "";
            }

            description += Encumbrance != 0.0 ? $"; Encumbrance: {Encumbrance}%" : "";

            if (Item.GetComponent<ItemValueComponent>() != null)
            {
                description += $"; ; Value: {Value}c";
            }
            return description;
        }

        public List<ComparisonResult> CompareToEquipment(Func<Entity, Entity, ComparisonResult> comparator)
        {
            var items = FindItemsToCompareTo();
            if (items.Count == 0)
            {
                return [];
            }
            return items.Select(item => comparator(Item, item)).ToList();
        }
    }
}
