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
    public static class BlazingWeapon
    {
        public static readonly string BasePerkId = "BlazingWeapon";
        public static readonly string SupportPerkId = BasePerkId + "_Duration";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.BlazingWeapon_Name,
            Properties.Skills.BlazingWeapon_Description,
            [UpdateTrigger.EquipmentChanged],
            (l, e, w, c) => 
            {
                var equipComp = e.GetComponent<EquipmentComponent>();
                var weapons = equipComp?.GetItems()?.Where(i => i.IsWeapon()).ToList();
                if (weapons is null || weapons.Count == 0)
                    return [new(BasePerkId, ModifierType.AddBase, 0.0, [Tags.Damage, DamageType.Fire.ToTag()], [])];
                double percentage = l * 0.1;
                List<Modifier> result = [];
                for (int i = 0; i < weapons.Count; i++)
                {
                    double fireDmg = percentage * weapons[i].GetComponent<WeaponComponent>()!.Damage.DamageOfType(DamageType.Physical);
                    string tag = i == 0 ? Tags.MainHand : Tags.OffHand;
                    result.Add(new(BasePerkId + $"_{i}", ModifierType.AddBase, fireDmg, [Tags.Damage, DamageType.Fire.ToTag(), tag], []));
                }
                return result;
            }
        )
        {
            MaxLevel = 8,
            ApplyModifiersToOwner = false,
            Skill = BlazingWeaponSkill.Skill
        };

        public static readonly Perk SupportPerk = new(
            SupportPerkId,
            Properties.Skills.BlazingWeaponSupport_Name,
            Properties.Skills.BlazingWeaponSupport_Description,
            [],
            (l, e, w, c) =>
            {
                double bonus = 0.2 * l;
                return [
                    new(SupportPerkId + "_duration", ModifierType.Increase, bonus, [Tags.Buff, Tags.Duration], [])
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false
        };
    }
}
