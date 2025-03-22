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

        private string GenerateDescription()
        {
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
                description += $"; Damage: {weaponComp.Damage} [{(weaponComp.Damage / weaponComp.Cooldown):#.##} DPS]; ";
                description += $"Attack Time: {weaponComp.Cooldown} s";
            }

            var armorComp = Item.GetComponent<ArmorComponent>();
            if (armorComp != null)
            {
                description += armorComp.Armor != 0.0 ? $"; Armor: {armorComp.Armor}" : "";
                description += armorComp.Evasion != 0.0 ? $"; Evasion: {armorComp.Evasion}" : "";
            }

            if (equipComp != null)
            {
                description += equipComp.Encumbrance != 0.0 ? $"; Encumbrance: {equipComp.Encumbrance}%" : "";
            }

            var valueComp = Item.GetComponent<ItemValueComponent>();
            if (valueComp != null)
            {
                description += $"; ; Value: {valueComp.Value}c";
            }
            return description;
        }

        private string GetItemTypeName()
        {
            if (Item.IsWeapon())
                return LocalizedStrings.Equip_Weapon;
            if (Item.IsShield())
                return LocalizedStrings.Equip_Shield;
            if (Slots.Count == 0 || !Item.IsArmor()) // Should not happen with the current item kingdom
                return "??";

            if (Slots.Count > 1)
                return "Custom Gear"; // Does not currently exist
            return Slots[0] switch
            {
                EquipmentSlot.Head => LocalizedStrings.Equip_HeadArmor,
                EquipmentSlot.Chest => LocalizedStrings.Equip_ChestArmor,
                EquipmentSlot.Arms => LocalizedStrings.Equip_ArmArmor,
                EquipmentSlot.Legs => LocalizedStrings.Equip_LegArmor,
                _ => "??",
            };
        }
    }
}
