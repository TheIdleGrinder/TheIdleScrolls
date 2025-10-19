using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Skills;

namespace TheIdleScrolls_Core.Perks
{
    public static class ExposeWeaknessPerks
    {
        public static readonly string BasePerkId = "ExposeWeakness";
        public static readonly string DmgTakenPerkId = "ExposeWeakness_DmgTaken";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            "Expose Weakness",
            "Lifts damage limit on affected opponent",
            [],
            (l, e, w, c) =>
            {
                return [new Modifier(BasePerkId + "_lessDefLayers", ModifierType.More, -l * 0.25, [Tags.DefensiveLayers], [])];
            }
        )
        {
            MaxLevel = 4,
            ApplyModifiersToOwner = false,
            Skill = ExposeWeaknessSkill.Skill
        };

        public static readonly Perk DamageTaken = new(
            DmgTakenPerkId,
            "Vital Points",
            "Enemies affected by Expose Weakness also take increased damage",
            [],
            (l, e, w, c) =>
            {
                return [new Modifier(DmgTakenPerkId + "_incDamage", ModifierType.Increase, l * 0.05, [Tags.DamageTaken], [])];
            }
        )
        {
            MaxLevel = 4,
            ApplyModifiersToOwner = false
        };
    }
}
