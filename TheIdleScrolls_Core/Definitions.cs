using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core
{
    namespace Definitions
    {
        public static class Stats
        {
            public const double AttackBonusPerLevel = 0.1;
            public const double AttackDamagePerAbilityLevel = 0.02;
            public const double AttackSpeedPerAbilityLevel = 0.01;
            public const double DualWieldAttackSpeedMulti = 0.2;
            public const double DefensePerAbilityLevel = 0.02;

            public const double ArmorSlowdownPerPoint = 0.01;
            public const double EvasionBonusPerPoint = 0.01;
            public const double MaxEvasionChargeDuration = 5.0;
            public const double MaxEvasionEffectDuration = 1.0;

            public const double CraftingAbilityBonusPerLevel = 0.02;
            public const double ReforgingBaseDuration = 30.0;
            public const double ReforgingDurationPerMaterialTier = 10.0;

            public const int MobBaseHp = 20;
            public const double EarlyHpScaling = 1.056;
            public const double LaterHpScaling = 1.035;
            public const int ScalingSwitchLevel = 70;
        }

        public static class Abilities
        {
            public static readonly string[] Weapons = [ Properties.Constants.Key_Ability_Axe,
                                                              Properties.Constants.Key_Ability_Blunt,
                                                              Properties.Constants.Key_Ability_LongBlade,
                                                              Properties.Constants.Key_Ability_Polearm,
                                                              Properties.Constants.Key_Ability_ShortBlade
            ];

            public static readonly string[] Armors = [ Properties.Constants.Key_Ability_LightArmor,
                                                             Properties.Constants.Key_Ability_HeavyArmor
            ];

            public static readonly string Crafting = Properties.Constants.Key_Ability_Crafting;
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
            public const string EndgameTechnology = "SUNKENCITY";
            public const string EndgameMagic = "ACADEMY";
            public const string EndgameDistantLands = "PAGODA";
            public const string EndgamePyramid = "PYRAMID";
            public const string EndgameAges = "AGES";
        }

        public static class Tags
        {
            public const string RarityPrefix = "+";
            public const string HandSuffix = "H";
            public const string Damage = "Damage";
            public const string AttackSpeed = "AttackSpeed";
            public const string Defense = "Defense";
            public const string ArmorRating = "ArmorRating";
            public const string EvasionRating = "EvasionRating";
            public const string Weapon = "Weapon";
            public const string Armor = "Armor";
            public const string Shield = "Shield";
            public const string Melee = "Melee";
            public const string Ranged = "Ranged";
            public const string Unarmed = "Unarmed";
            public const string Unarmored = "Unarmored";
            public const string DualWield = "DualWield";
            public const string MixedWeapons = "MixedWeapons";
            public const string MixedArmor = "MixedArmor";
            public const string FirstStrike = "FirstStrike";

            public const string CharacterXpGain = "CharacterXpGain";
            public const string AbilityXpGain = "AbilityXpGain";

            public const string CraftingSlots = "CraftingSlot";
            public const string ActiveCrafts = "ActiveCraftingSlot";
            public const string CraftingSpeed = "CraftingSpeed";
            public const string CraftingCost = "CraftingCost";
        }
    }

    public static class Functions
    {
        public static double CalculateAbilityAttackSpeedBonus(int abilityLevel)
        {
            //return Math.Pow(1.0 + Definitions.Stats.AttackSpeedPerAbilityLevel, abilityLevel) - 1.0;
            return Definitions.Stats.AttackSpeedPerAbilityLevel * abilityLevel;
        }

        public static double CalculateAbilityAttackDamageBonus(int abilityLevel)
        {
            //return Math.Pow(1.0 + Definitions.Stats.AttackDamagePerAbilityLevel, abilityLevel) - 1.0;
            return Definitions.Stats.AttackDamagePerAbilityLevel * abilityLevel;
        }

        public static double CalculateAbilityDefenseBonus(int abilityLevel)
        {
            //return Math.Pow(1.0 + Definitions.Stats.DefensePerAbilityLevel, abilityLevel) - 1.0;
            return Definitions.Stats.DefensePerAbilityLevel * abilityLevel;
        }

        public static double CalculateAssumedPlayerDamageMultiplier(int level)
        {
            var maxGearLevel = Definitions.Stats.ScalingSwitchLevel;
            var rarityBonusPerLevel = (Math.Pow(1.25, 4) - 1) / 150; // Smooth transition to +4 rarity at level 150
            var materialBonusPerLevel = Math.Pow(1.5, 1.0 / (maxGearLevel / 3.0));

            // Assumption: Ability levels somewhat align with character level
            return (1.0 + CalculateAbilityAttackDamageBonus(level))                 // Ability damage bonus
                * (1.0 + CalculateAbilityAttackSpeedBonus(level))                   // Ability attack speed bonus
                * (1.0 + Definitions.Stats.AttackBonusPerLevel * (level - 1))       // Level scaling
                * Math.Pow(materialBonusPerLevel, Math.Min(level, maxGearLevel))    // Material scaling (3 tiers)
                * (1.0 + (0.2 / maxGearLevel * Math.Min(maxGearLevel, level)))      // Smooth transition to highest tier of weapons
                * (1.0 + level * rarityBonusPerLevel)                               // Smooth transition to +4 rarity at level 150
                ;
        }

        public static double CalculateAssumedPlayerDefenseMultiplier(int level)
        {
            var maxGearLevel = Definitions.Stats.ScalingSwitchLevel;
            var rarityBonusPerLevel = (Math.Pow(1.25, 4) - 1) / 150; // Smooth transition to +4 rarity at level 150
            var materialBonusPerLevel = Math.Pow(1.5, 1.0 / (maxGearLevel / 3.0));
            return (1.0 + CalculateAbilityDefenseBonus(level))                    // Ability defense bonus
				* Math.Pow(materialBonusPerLevel, Math.Min(level, maxGearLevel))  // Material scaling (3 tiers)
				* (1.0 + (0.2 / maxGearLevel * Math.Min(maxGearLevel, level)))    // Smooth transition to highest tier of armor
				* (1.0 + level * rarityBonusPerLevel)                             // Smooth transition to +4 rarity at level 150
				;
        }

        public static double CalculateDefenseRating(double armor, double evasion, int level)
        {
            double damage = CalculateMobDamage(level);
            double accuracy = CalculateMobAccuracy(level);
            double multiplier = CalculateArmorBonusMultiplier(armor, damage) * CalculateEvasionBonusMultiplier(evasion, accuracy);
            return (multiplier - 1.0) * 100.0;
        }

        public static double CalculateEvasionBonusMultiplier(double evasion, double enemyAccuracy = 1.0)
        {
            if (enemyAccuracy == 0.0)
                enemyAccuracy = 1.0;
            double effectiveEvasion = evasion / enemyAccuracy;
            return 1.0 + effectiveEvasion * Definitions.Stats.EvasionBonusPerPoint;
        }

        public static double CalculateArmorBonusMultiplier(double armor, double incomingDamage = 1.0)
        {
            if (incomingDamage == 0.0)
                incomingDamage = 1.0;
            double effectiveArmor = armor / incomingDamage;
            return 1.0 + effectiveArmor * Definitions.Stats.ArmorSlowdownPerPoint;
        }

        public static int CalculateMobHp(int mobLevel, double multiplier = 1.0)
        {
            // Old calculation, kept for future reference
            //return (int) Math.Min(1_000_000_000, 
            //    Definitions.Stats.MobBaseHp * multiplier
            //    * Math.Pow(Definitions.Stats.EarlyHpScaling, Math.Min(mobLevel, Definitions.Stats.ScalingSwitchLevel))
            //    * Math.Pow(Definitions.Stats.LaterHpScaling, Math.Max(mobLevel - Definitions.Stats.ScalingSwitchLevel, 0))
            //    * (1.0 + Definitions.Stats.AttackBonusPerLevel * (mobLevel - 1))
            //);
            double mobBaseHpMultiplier = 0.9;
            return (int) Math.Min(1_000_000_000,
                Definitions.Stats.MobBaseHp * multiplier
                * CalculateAssumedPlayerDamageMultiplier(mobLevel)
                * (mobBaseHpMultiplier + 0.01 * (mobLevel - 1))
            );
        }

        public static double CalculateMobDamage(int mobLevel, double multiplier = 1.0)
        {
            return multiplier
                * Math.Sqrt(CalculateAssumedPlayerDefenseMultiplier(mobLevel));
        }

        public static double CalculateMobAccuracy(int mobLevel)
        {
            // First implementation: Accuracy rating is identical to default damage
            return CalculateMobDamage(mobLevel, 1.0);
        }

        public static double CalculateReforgingSuccessRate(int abilityLevel, int currentRarity)
        {
            return abilityLevel / (abilityLevel + Math.Pow(currentRarity + 1, 2) * 10);
        }

        public static double CalculateReforgingDuration(Entity item, Entity? crafter)
        {
            var materialTier = item.GetComponent<ItemMaterialComponent>()?.Tier ?? 0;
            double baseDuration = Definitions.Stats.ReforgingBaseDuration 
                + Definitions.Stats.ReforgingDurationPerMaterialTier * materialTier;
            double speed = crafter?.ApplyAllApplicableModifiers(1.0, 
                new string[] { Definitions.Tags.CraftingSpeed }, 
                crafter.GetTags()) ?? 1.0;
            
            // CornerCut: Minimum speed of 1% to prevent eternal crafts, realistically will never be below 1.0
            return Math.Ceiling(baseDuration / Math.Max(speed, 0.01));
        }

        public static int CalculateCraftingCost(Entity item, Entity? crafter)
        {
            int baseCost = item.GetComponent<ItemReforgeableComponent>()?.Cost ?? 100;
            double cost = crafter?.ApplyAllApplicableModifiers(baseCost, 
                new string[] { Definitions.Tags.CraftingCost },
                crafter.GetTags()) ?? baseCost;
            return (int)Math.Ceiling(Math.Max(cost, 1.0));
        }
    }
}
