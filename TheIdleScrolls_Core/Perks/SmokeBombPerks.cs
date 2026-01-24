using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Skills.Skills;

namespace TheIdleScrolls_Core.Perks
{
    public static class SmokeBombPerks
    {
        public static readonly string BasePerkId = "SmokeBomb";
        public static readonly string DamageWhileActiveId = "SmokeBomb_DmgWhileActive";
        public static readonly string DamageOnActivityEndId = "SmokeBomb_DamageOnActivityEnd";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            "Smoke Bomb",
            "Gain the Smoke Bomb skill: Toss a small contraption that releases smoke, granting a bonus to evasion rating while the effect " +
            "is active. Duration increases with level.",
            [],
            (l, e, w, c) =>
            {
                return [new(BasePerkId, ModifierType.More, 0.2, [Tags.EvasionRating], [])];
            }
        )
        {
            MaxLevel = 3,
            ApplyModifiersToOwner = false,
            Skill = SmokeBombSkill.Skill
        };

        public static readonly Perk DamageWhileActive = new(
            DamageWhileActiveId,
            "Toxic Fumes",
            "Enemies suffer damage over time during the effect of Smoke Bomb",
            [],
            (l, e, w, c) =>
            {
                double dmg = l switch
                {
                    1 => 5.0,
                    2 => 12.0,
                    3 => 20.0,
                    4 => 30.0,
                    5 => 45.0,
                    _ => 0.0
                };
                return [new(DamageWhileActiveId, ModifierType.AddBase, dmg, [Tags.DamageOverTime], [])];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false
        };

        public static readonly Perk DamageOnActivityEnd = new(
            DamageOnActivityEndId,
            "Explosive Core",
            "Smoke bomb explodes at the end of its duration, dealing damage to the enemy.",
            [],
            (l, e, w, c) =>
            {
                double dmg = l switch
                {
                    1 => 15.0,
                    2 => 36.0,
                    3 => 60.0,
                    4 => 90.0,
                    5 => 150.0,
                    _ => 0.0
                };
                return [new(DamageOnActivityEndId, ModifierType.AddBase, dmg, [Tags.Damage], [])];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false
        };
    }
}
