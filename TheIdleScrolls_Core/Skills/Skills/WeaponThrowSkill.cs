using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.StatusEffects;
using TheIdleScrolls_Core.Utility;
using static System.Net.Mime.MediaTypeNames;

namespace TheIdleScrolls_Core.Skills.Skills
{
    public static class WeaponThrow
    {
        public const string BasePerkId = "WeaponThrow";
        public const string FirstModPerkId = BasePerkId + "_mod1";

        public static readonly Perk BasePerk = new(
            BasePerkId,
            Properties.Skills.WeaponThrow_Name,
            Properties.Skills.WeaponThrow_Description,
            [],
            (l, e, w, c) =>
            {
                double dmgMult = 0.65 + 0.07 * l;
                return [
                    new($"{BasePerkId}_dmg", ModifierType.More, dmgMult - 1.0, [Tags.Damage], []),
                    new($"{BasePerkId}_range", ModifierType.AddBase, 6.0, [Tags.Range], [])
                ];
            }
        )
        {
            MaxLevel = 6,
            ApplyModifiersToOwner = false,
            Skill = new WeaponThrowSkill(),
            Categories = [Properties.Skills.WeaponThrow_Name],
            ConditionFunc = PerkExtensions.FlatPerkLevelCondition(PerkIds.Dexterity, 5)
        };

        //public static readonly Perk FirstModPerk = new(
        //    FirstModPerkId,
        //    Properties.Skills.HerbalMedicineFirstMod_Name,
        //    Properties.Skills.HerbalMedicineFirstMod_Description,
        //    [],
        //    (l, e, w, c) =>
        //    {
        //        return [
        //            new($"{FirstModPerkId}_poisonresist", ModifierType.AddBase, 0.08 * (l + 2), [Tags.Resistance, DamageType.Poison.ToTag()], [])
        //            {
        //                AlwaysPercentage = true
        //            }
        //        ];
        //    }
        //)
        //{
        //    MaxLevel = 5,
        //    ApplyModifiersToOwner = false,
        //    Categories = [Properties.Skills.HerbalMedicine_Name],
        //    ConditionFunc = PerkExtensions.FlatPerkLevelCondition(BasePerkId, 1)
        //};
    }

    public class WeaponThrowSkill : ActiveSkill
    {
        public override string Id => WeaponThrow.BasePerkId;

        public override string Name => Properties.Skills.WeaponThrow_Name;
        public override bool IsAvailableTo(Entity user)
        {
            return HasPerkActive(user, WeaponThrow.BasePerkId);
        }

        public override (UsePrevention prevention, string details) IsUsableBy(Entity user)
        {
            if (!IsAvailableTo(user))
                return (UsePrevention.MissingPerk, "You haven't unlocked this skill yet.");
            if (!user.IsInBattle())
                return (UsePrevention.NotInBattle, "You can only use this skill in battle.");
            var mainHandItem = user.GetComponent<EquipmentComponent>()?.GetItemInSlot(EquipmentSlot.Hand);
            if (mainHandItem is null)
                return (UsePrevention.WrongEquipment, "You need a weapon equipped in your main hand to use this skill.");
            var family = mainHandItem.GetComponent<ItemComponent>()?.Blueprint?.FamilyId ?? "";
            if (family != ItemFamilies.Dagger && family != ItemFamilies.OneHandedAxe)
                return (UsePrevention.WrongEquipment, "You need a dagger or one-handed axe equipped in your main hand to use this skill.");

            double minRange = user.GetComponent<BattleStatsComponent>()?.AttackVectors[0]?.Range ?? 0.0;
            double maxRange = minRange + 6.0;
            if (ActiveSkill.GetEnemiesInRange(user, maxRange).Count == 0)
                return (UsePrevention.NoTargetInRange, "No enemies in range");
            if (ActiveSkill.GetEnemiesInRange(user, minRange).Count > 0)
                return (UsePrevention.WrongEquipment, "Target is too close");
            return (UsePrevention.None, "");
        }

        protected override void SetupStats(Entity user)
        {
            SkillTags = [Tags.AttackSkill, Tags.Ranged];

            this.Range = user.GetComponent<BattleStatsComponent>()?.AttackVectors[0]?.Range ?? 0.0;

            var battleStatsComp = user.GetComponent<BattleStatsComponent>();
            DamageCluster baseDamage = battleStatsComp?.AttackVectors[0].RawDamage ?? new(DamageType.Physical, 2.0);
            DamageCluster damage = baseDamage * ((GetPerk(WeaponThrow.BasePerkId)?.Modifiers[0]?.Value ?? 0.0) + 1.0);
            var effects = DefaultAttack.CreateDefaultSkillEffectsForDamage(damage, [.. SkillTags]);
            ActivityStartEffects = [new(effects, new(TargetType.Enemy), SkillTags)];
            ChargingTime = battleStatsComp?.AttackVectors[0]?.AttackTime ?? 1.0;
        }
    }
}
