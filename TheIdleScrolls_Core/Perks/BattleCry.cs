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
    public static class BattleCry
    {
        public static readonly string BasePerkId = "BattleCry";
        public static readonly string BuffPerkId = BasePerkId + "_buff";
        public static readonly string DebuffPerkId = BasePerkId + "_debuff";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.BattleCry_Name,
            Properties.Skills.BattleCry_Description,
            [],
            (l, e, w, c) =>
            {
                double speed = (l + 2) * 0.1;
                return 
                [
                    new(BasePerkId + "_speed", ModifierType.Increase, speed, [Tags.AttackSpeed], [])
                ];
            }
        )
        {
            MaxLevel = 6,
            ApplyModifiersToOwner = false,
            Skill = BattleCrySkill.Skill
        };

        public static readonly Perk BuffPerk = new(
            BuffPerkId,
            Properties.Skills.BattleCryBuff_Name,
            Properties.Skills.BattleCryBuff_Description,
            [],
            (l, e, w, c) =>
            {
                double defense = (l + 2) * 0.1;
                double stunRes = l * 0.15 + 0.05;
                return
                [
                    new(BuffPerkId + "_def", ModifierType.Increase, defense, [Tags.Defense], []),
                    new(BuffPerkId + "_res", ModifierType.AddBase, defense, [Tags.Stun, Tags.Resistance], [])
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false
        };

        public static readonly Perk DebuffPerk = new(
            DebuffPerkId,
            Properties.Skills.BattleCryDebuff_Name,
            Properties.Skills.BattleCryDebuff_Description,
            [],
            (l, e, w, c) =>
            {
                return [
                    new(DebuffPerkId + "_slow", ModifierType.AddBase, (l + 1) * 0.1, [Tags.Slow], []),
                ];
            }
        )
        {
            MaxLevel = 5,
            ApplyModifiersToOwner = false
        };
    }
}
