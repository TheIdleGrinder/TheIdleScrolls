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
    public static class CrushingBlow
    {
        public static readonly string BasePerkId = "CrushingBlow";
        public static readonly string BasePerkDmgModId = BasePerkId + "_dmg";
        public static readonly string BasePerkAntiDefenseModId = BasePerkId + "_antidef";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.CrushingBlow_Name,
            Properties.Skills.CrushingBlow_Description,
            [UpdateTrigger.EquipmentChanged, UpdateTrigger.BattleStarted, UpdateTrigger.AttackPerformed, 
                UpdateTrigger.AbilityIncreased],
            (l, e, w, c) =>
            {
                double attackSpeed = e.GetComponent<AttackComponent>()?.AverageCooldown ?? 1.0;
                double multi = 0.7 + 0.3 * l;

                return [
                    new Modifier(BasePerkDmgModId, ModifierType.Increase, multi * attackSpeed,
                        [Tags.Damage, Tags.Melee, DamageTypeTags.Physical], []),
                    new Modifier(BasePerkAntiDefenseModId, ModifierType.More, -0.15 * l,
                        [Tags.DefensiveLayers], [])
                ];
            }
        )
        {
            MaxLevel = 6,
            ApplyModifiersToOwner = false,
            Skill = CrushingBlowSkill.Skill
        };
    }
}
