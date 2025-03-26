using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;

namespace TheIdleScrolls_Core
{
    namespace Definitions
    {
        public static class Stats
        {
            public const double AttackBonusPerLevel = 0.02;
            public const double TimeShieldBonusPerLevel = 0.02;
            public const double AttackDamagePerAbilityLevel = 0.02;
            public const double AttackSpeedPerAbilityLevel = 0.005;
            public const double DualWieldAttackSpeedMulti = 0.2;
            public const double DefensePerAbilityLevel = 0.02;
            public const double MaxAttacksPerSecond = 10.0;

            public const double ArmorSlowdownPerPoint = 0.01;
            public const double EvasionBonusPerPoint = 0.01;
            public const double MaxEvasionChargeDuration = 5.0;
            public const double MaxEvasionEffectDuration = 1.0;
            public const double MaxResistanceFromArmor = 0.9;
            public const double MaxResistanceFromEvasion = 0.9;

            public const double ItemBaseValue = 5.0;
            public const double ItemValueQualityMultiplier = 1.35;

            public const double CraftingAbilityBonusPerLevel = 0.02;
            public const double CraftingBaseDuration = 15.0;
            public const double CraftingDurationPerMaterialTier = 10.0;
            public const double CraftingBaseCost = 12.0;
            public const double CraftingCostWeaponMultiplier = 2.0;
            public const double CraftingCostTierExponent = 0.7;
            public const double CraftingCostMaterialExponent = 0.7;

            public const int    MobBaseHp = 20;
            public const double EarlyHpScaling = 1.056;
            public const double LaterHpScaling = 1.035;
            public const int    ScalingSwitchLevel = Materials.LevelT4 + ItemTiers.LevelT3; // Level of highest item tier

            public const int    DefaultDropLevelRange = 20;
            public const int    DungeonDropLevelRange = 9;

            public const double QualityMultiplier   = 1.25;

            public const int    LevelsPerPerkPoint          = 5;
            public const int    PerkPointLevelLimit         = 200;
            public const double BasicDamageIncrease         = 0.10;
            public const double BasicAttackSpeedIncrease    = 0.05;
            public const double BasicDefenseIncrease        = 0.08;
            public const double BasicTimeIncrease           = 0.05;
            public const double BigPerkFactor               = 1.5;
            public const double MasterPerkMultiplier        = 0.1;
            public const double SavantXpMultiplier          = 0.3;
        }

        public static class DungeonIds
        {
            public const string DenOfRats = "RATDEN";
            public const string Crypt = "CRYPT";
            public const string Lighthouse = "LIGHTHOUSE";
            public const string Temple = "TEMPLE";
            public const string MercenaryCamp = "MERCCAMP";
            public const string CultistCastle = "CASTLE";
            public const string Labyrinth = "LABYRINTH";
            public const string ReturnToLighthouse = "LIGHTHOUSE2";
            public const string Threshold = "THRESHOLD";
            public const string Void = "VOID";
            public const string EndgameTechnology = "SUNKENCITY";
            public const string EndgameMagic = "ACADEMY";
            public const string EndgameDistantLands = "PAGODA";
            public const string EndgamePyramid = "PYRAMID";
            public const string EndgameAges = "AGES";
        }

        public static class DungeonLevels
        {
            public const int LevelVoidMax = 125;
        }

        public static class Tags
        {
            public const string Local = "Local";
            public const string Global = "Global";

            public const string Damage = "Damage";
            public const string AttackSpeed = "AttackSpeed";
            public const string Defense = "Defense";
            public const string ArmorRating = "ArmorRating";
            public const string EvasionRating = "EvasionRating";
            public const string TimeShield = "TimeShield";

            public const string TimeLoss = "TimeLoss";

            public const string QualityPrefix = "+";
            public const string HandSuffix = "H";
            public const string Weapon = "Weapon";
            public const string Armor = "Armor";
            public const string Shield = "Shield";
            
            public const string Melee = "Melee";
            public const string Ranged = "Ranged";

            public const string Unarmed = "Unarmed";
            public const string Unarmored = "Unarmored";
            public const string DualWield = "DualWield";
            public const string TwoHanded = "TwoHanded";
            public const string Shielded = "Shielded";
            public const string SingleHanded = "SingleHanded";
            public const string MixedWeapons = "MixedWeapons";
            public const string MixedArmor = "MixedArmor";
            public const string FirstStrike = "FirstStrike";
            public const string Evading = "Evading";

            public const string CharacterXpGain = "CharacterXpGain";
            public const string AbilityXpGain = "AbilityXpGain";

            public const string CraftingSlots = "CraftingSlot";
            public const string ActiveCrafts = "ActiveCraftingSlot";
            public const string CraftingSpeed = "CraftingSpeed";
            public const string CraftingCost = "CraftingCost";
        }

        public static class DropRestrictions
        {
            public const string MaterialT4 = "MaterialT4";
        }
    }

    public static class Functions
    {
        public static double CalculateAbilityAttackSpeedBonus(int abilityLevel)
        {
            return Stats.AttackSpeedPerAbilityLevel * abilityLevel;
        }

        public static double CalculateAbilityAttackDamageBonus(int abilityLevel)
        {
            return Stats.AttackDamagePerAbilityLevel * abilityLevel;
        }

        public static double CalculateAbilityDefenseBonus(int abilityLevel)
        {
            return Stats.DefensePerAbilityLevel * abilityLevel;
        }

        // 4 Material tiers above training equipment, 1.5 multiplier per tier
        private static double MaterialBonusPerLevel => Math.Pow(1.5, 4.0 / Stats.ScalingSwitchLevel);

        private static double QualityBonusAtLevel(int level)
        {
            // Linear to +4 at level 150, slightly exponential afterwards
            if (level <= 150)
            {
                double perLevel = (Math.Pow(1.25, 4) - 1) / 150;
                return 1.0 + level * perLevel;
            }
            else
            {
                double bonusBase = Math.Pow(1.25, 1.0 / 37.5);
                return Math.Pow(bonusBase, level);
            }
        }

        public static double CalculateAssumedPlayerDamageMultiplier(int level)
        {
            var maxGearLevel = Stats.ScalingSwitchLevel;

            // Assumption: Ability levels somewhat align with character level
            return (1.0 + CalculateAbilityAttackDamageBonus(level))                 // Ability damage bonus
                * (1.0 + CalculateAbilityAttackSpeedBonus(level))                   // Ability attack speed bonus
                * (1.0 + 2 * Stats.AttackBonusPerLevel * (level - 1))               // Level scaling (x2 for perks)
                * Math.Pow(MaterialBonusPerLevel, Math.Min(level, maxGearLevel))    // Material scaling (4 tiers)
                * (1.0 + (0.2 / maxGearLevel * Math.Min(maxGearLevel, level)))      // Smooth transition to highest tier of weapons
                * QualityBonusAtLevel(level)
                ;
        }

        public static double CalculateAssumedPlayerDefenseMultiplier(int level)
        {
            var maxGearLevel = Stats.ScalingSwitchLevel;
            return (1.0 + CalculateAbilityDefenseBonus(level))                      // Ability defense bonus
				* Math.Pow(MaterialBonusPerLevel, Math.Min(level, maxGearLevel))    // Material scaling (4 tiers)
				* (1.0 + (0.2 / maxGearLevel * Math.Min(maxGearLevel, level)))      // Smooth transition to highest tier of armor
				* QualityBonusAtLevel(level)
                * (1.0 + 2 * (level - 1) * Stats.TimeShieldBonusPerLevel)           // Account for time shield bonus from levelling (x2 for perks)
                ;
        }

        public static double CalculateDefenseRating(double armor, double evasion, int level)
        {
            double damage = 1.0; // use default damage for calculation
            double accuracy = CalculateMobAccuracy(level);
            double multiplier = CalculateArmorBonusMultiplier(armor, level, damage) * CalculateEvasionBonusMultiplier(evasion, accuracy);
            return 1.0 - (1.0 / multiplier);
        }

        public static double CalculateEvasionBonusMultiplier(double evasion, double enemyAccuracy = 1.0)
        {
            if (enemyAccuracy == 0.0)
                enemyAccuracy = 1.0;
            double effectiveEvasion = evasion / enemyAccuracy;
            return Math.Min(1.0 + effectiveEvasion * Stats.EvasionBonusPerPoint, 1.0 / (1.0 - Stats.MaxResistanceFromEvasion));
        }

        public static double CalculateArmorBonusMultiplier(double armor, int enemyLevel, double incomingDamage = 1.0)
        {
            if (incomingDamage == 0.0)
                incomingDamage = 1.0;
            double effectiveArmor = armor / CalculateMobArmorPierce(enemyLevel, incomingDamage);
            return Math.Min(1.0 + effectiveArmor * Stats.ArmorSlowdownPerPoint, 1.0 / (1.0 - Stats.MaxResistanceFromArmor));
        }

        public static int CalculateMobHp(int mobLevel, double multiplier = 1.0)
        {
            double mobBaseHpMultiplier = 0.9;
            return (int) Math.Min(1_000_000_000,
                Stats.MobBaseHp * multiplier
                * CalculateAssumedPlayerDamageMultiplier(mobLevel)
                * (mobBaseHpMultiplier + 0.01 * (mobLevel - 1))
            );
        }

        public static double CalculateMobArmorPierce(int mobLevel, double multiplier = 1.0)
        {
            return multiplier
                * Math.Sqrt(CalculateAssumedPlayerDefenseMultiplier(mobLevel));
        }

        public static double CalculateMobAccuracy(int mobLevel)
        {
            // First implementation: Accuracy rating is identical to default armor pierce
            return CalculateMobArmorPierce(mobLevel, 1.0);
        }

        public static double CalculateBaseTimeLimit(int playerLevel, int areaLevel)
        {
            if (areaLevel == 0)
                return 0.0;
            return 10.0 / CalculateMobArmorPierce(areaLevel);
        }

        public static double CalculateRefiningSuccessRate(int abilityLevel, int currentQuality)
        {
            return abilityLevel / (abilityLevel + Math.Pow(currentQuality + 1, 2) * 10);
        }

        public static double CalculateRefiningDuration(Entity item, Entity? crafter)
        {
            double matMulti = Math.Pow(item.GetBlueprint()!.GetMaterial().PowerMultiplier, 0.2);
            double tierMulti = Math.Pow(item.GetBlueprint()!.GetDropLevel() / 10, 0.2);
            double typeMulti = item.IsWeapon() ? Stats.CraftingCostWeaponMultiplier : 1.0;

            double baseDuration = Stats.CraftingBaseDuration * matMulti * tierMulti * typeMulti;
            double speed = crafter?.ApplyAllApplicableModifiers(1.0, 
                [Tags.CraftingSpeed], 
                crafter.GetTags()) ?? 1.0;
            
            // CornerCut: Minimum speed of 1% to prevent eternal crafts, realistically will never be below 1.0
            return Math.Ceiling(baseDuration / Math.Max(speed, 0.01));
        }

        public static int CalculateCraftingCost(Entity item, Entity? crafter)
        {
            int baseCost = item.GetComponent<ItemRefinableComponent>()?.Cost ?? 100;
            double cost = crafter?.ApplyAllApplicableModifiers(baseCost, 
                [Tags.CraftingCost],
                crafter.GetTags()) ?? baseCost;
            return (int)Math.Ceiling(Math.Max(cost, 1.0));
        }
    }
}
