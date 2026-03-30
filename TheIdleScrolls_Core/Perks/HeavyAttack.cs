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
    public static class HeavyAttack
    {
        public static readonly string BasePerkId = "HeavyAttack";
        public static readonly string BasePerkDmgModId = BasePerkId + "_dmg";
        public static readonly string BasePerkSpdModId = BasePerkId + "_spd";

        public static readonly string DualWieldingBonusPerkId = BasePerkId + "_DualWielding";
        public static readonly string DualWieldingBonusDmgModId = DualWieldingBonusPerkId + "_dmg";

        public static readonly string ShieldedBonusPerkId = BasePerkId + "_Shielded";
        public static readonly string ShieldedBonusDmgModId = ShieldedBonusPerkId + "_dmg";

        public static readonly string SingleHandedBonusPerkId = BasePerkId + "_SingleHanded";
        public static readonly string SingleHandedBonusDmgModId = SingleHandedBonusPerkId + "_dmg";

        public static readonly string TwoHandedBonusPerkId = BasePerkId + "_TwoHanded";
        public static readonly string TwoHandedBonusStunModId = TwoHandedBonusPerkId + "_stun";


        public static readonly Perk BasePerk = new(
            BasePerkId,
            "Heavy Attack",
            "A slow but powerful attack",
            [],
            (l, e, w, c) =>
            {
                double dmgBonus = 0.35 + 0.15 * l;
                double spdMalus = 0.2;
                return [
                    new Modifier(BasePerkDmgModId, ModifierType.More, dmgBonus,  [Tags.Damage], []),
                    new Modifier(BasePerkSpdModId, ModifierType.More, -spdMalus, [Tags.AttackSpeed], [])
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false,
            Skill = HeavyAttackSkill.Skill
        };

        public static readonly Perk DualWieldPerk = new(
            DualWieldingBonusPerkId,
            Properties.Skills.HeavyAttack_DualWield_Name,
            Properties.Skills.HeavyAttack_DualWield_Description,
            [UpdateTrigger.SkillStateChanged],
            (l, e, w, c) => 
            { 
                int uses = e.GetComponent<ActiveSkillComponent>()?.Skills
                    ?.FirstOrDefault(s => s.Id == DefaultAttack.SkillId)?.UseCount ?? 0;
                double multi = 0.005 * (l + 1);
                return [
                    new Modifier(DualWieldingBonusDmgModId, ModifierType.More, multi * uses, [Tags.Damage], [])
                ]; 
            }
        )
        {
            MaxLevel = 3,
            ApplyModifiersToOwner = false
        };

        public static readonly Perk ShieldedPerk = new(
            ShieldedBonusPerkId,
            Properties.Skills.HeavyAttack_Shielded_Name,
            Properties.Skills.HeavyAttack_Shielded_Description,
            [UpdateTrigger.EquipmentChanged, UpdateTrigger.SkillStateChanged, 
                UpdateTrigger.AbilityIncreased, UpdateTrigger.BattleStarted],
            (l, e, w, c) =>
            {
                double armor = e.GetComponent<DefenseComponent>()?.Armor ?? 0;
                double multi = 0.00005 * (l + 1);
                return [
                    new Modifier(ShieldedBonusDmgModId, ModifierType.Increase, multi * armor, [Tags.Damage], [])
                ];
            }
        )
        {
            MaxLevel = 3,
            ApplyModifiersToOwner = false
        };

        public static readonly Perk SingleHandedPerk = new(
            SingleHandedBonusPerkId,
            Properties.Skills.HeavyAttack_SingleHanded_Name,
            Properties.Skills.HeavyAttack_SingleHanded_Description,
            [],
            (l, e, w, c) =>
            {
                double multi = 0.25 * (l + 1);
                return [
                    new Modifier(SingleHandedBonusDmgModId, ModifierType.More, multi, [Tags.Damage], [Tags.VsLowLife])
                ];
            }
        )
        {
            MaxLevel = 3,
            ApplyModifiersToOwner = false
        };

        public static readonly Perk TwoHandedPerk = new(
            TwoHandedBonusPerkId,
            Properties.Skills.HeavyAttack_TwoHanded_Name,
            Properties.Skills.HeavyAttack_TwoHanded_Description,
            [],
            (l, e, w, c) =>
            {
                double duration = 0.5 * (l * 1);
                return [
                    new Modifier(TwoHandedBonusStunModId, ModifierType.AddBase, duration, [Tags.Stun], [])
                ];
            }
        )
        {
            MaxLevel = 3,
            ApplyModifiersToOwner = false
        };
    }
}
