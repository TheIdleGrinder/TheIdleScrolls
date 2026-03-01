using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Skills.Skills;

namespace TheIdleScrolls_Core.Perks
{
    public static class ShieldCharge
    {
        public static readonly string BasePerkId = "ShieldCharge";
        public static readonly string BasePerkDmgModId = BasePerkId + "_dmg";
        public static readonly string BasePerkArmorModId = BasePerkId + "_armor";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.ShieldCharge_Name,
            Properties.Skills.ShieldCharge_Description,
            [],
            (l, e, w, c) =>
            {
                double shieldArmour = e.GetComponent<EquipmentComponent>()
                                    ?.GetItems()?.FirstOrDefault(i => i.IsShield())
                                    ?.GetComponent<ArmorComponent>()?.Armor ?? 0.0;
                double multi = 0.1 + 0.1 * l;
                return [
                    new Modifier(BasePerkDmgModId, ModifierType.AddBase, multi * shieldArmour, 
                        [Tags.Damage, Tags.Melee, DamageTypeTags.Physical], []),
                    new Modifier(BasePerkArmorModId, ModifierType.Increase, 1.5 * multi,
                        [Tags.ArmorRating, Tags.Shield], []),
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false,
            Skill = ShieldChargeSkill.Skill
        };
    }
}
