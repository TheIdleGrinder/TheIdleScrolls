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
            public const int    BasePlayerHitPoints = 50;
            public const double PlayerHitPointPerLevel = 10;

            public const double BattleBaseDistance = 5.0;
            public const double BaseMovementSpeed = 3.0;

            public const double AttackBonusPerLevel = 0.02;
            public const double HitPointsBonusPerLevel = 0.02;
            public const double TimeShieldBonusPerLevel = 0.02;
            public const double AttackDamagePerAbilityLevel = 0.02;
            public const double AttackSpeedPerAbilityLevel = 0.005;
            public const double DualWieldAttackSpeedMulti = 0.1;
            public const double DefensePerAbilityLevel = 0.02;
            public const double MaxAttacksPerSecond = 10.0;

            public const double ArmorSlowdownPerPoint = 0.01;
            public const double EvasionBonusPerPoint = 0.01;
            public const double MaxEvasionChargeDuration = 5.0;
            public const double MaxEvasionEffectDuration = 1.0;
            public const double MaxResistanceFromArmor = 0.9;
            public const double MaxResistanceFromEvasion = 0.9;
            public const double MaxResistances = 80.0;
            
            public const double BaseBlockMitigation = 0.3;
            public const double BaseBlockCooldown = 2.0;

            public const double DamagePerMomentum = 0.02;

            public const double ItemBaseValue = 5.0;
            public const double ItemValueQualityMultiplier = 1.25;

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
            public const double BasicDamageIncrease         = 0.12;
            public const double BasicAttackSpeedIncrease    = 0.05;
            public const double BasicDefenseIncrease        = 0.08;
            public const double BasicTimeIncrease           = 0.12;
            public const double BigPerkFactor               = 1.5;
            public const double MasterPerkMultiplier        = 0.1;
            public const double SavantXpMultiplier          = 0.3;
            public const double TradeoffPerkBonus           = 1.05;    
            public const double TradeoffPerkMalus           = 0.97;

            public const double PruningBaseEffect           = 0.1;
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
            public const int LevelVoidMax       = 125;
            public const int LevelEndgame       = 150;
            public const int LevelUberEndgame   = 200;
        }

        public static class Tags
        {
            // Scaling targets
            public const string HitPoints = "HitPoints";
            public const string Speed = "Speed";
            public const string ChargeSpeed = "ChargeSpeed";
            public const string Damage = "Damage";
            public const string DamageOverTime = "DoT";
            public const string Healing = "Healing";
            public const string AttackSpeed = "AttackSpeed";
            public const string Defense = "Defense";
            public const string ArmorRating = "ArmorRating";
            public const string EvasionRating = "EvasionRating";
            public const string DamageTakenMultiplier = "DamageTakenMultiplier";
            public const string DamageTaken = "DamageTaken";
            public const string TimeLoss = "TimeLoss";
            public const string LifeRegeneration = "LifeRegeneration";
            public const string DamageReduction = "DamageReduction";
            public const string DefensiveLayers = "DefensiveLayers";
            public const string TimeShield = "TimeShield";
            public const string MovementSpeed = "MovementSpeed";
            public const string Range = "Range";
            public const string CooldownRecovery = "CooldownRecovery";
            public const string ActivationChance = "ActivationChance"; 
            
            public const string BlockChance = "BlockChance";
            public const string BlockRecovery = "BlockRecovery";
            public const string BlockMitigation = "BlockMitigation";

            public const string MomentumLimit = "MomentumLimit";
            public const string MomentumGain = "MomentumGain";
            public const string MomentumOnAttackHit = "MomentumOnAttackHit";
            public const string MomentumOnEvade = "MomentumOnEvade";
            public const string MomentumOnBlock = "MomentumOnBlock";

            public const string CharacterXpGain = "CharacterXpGain";
            public const string AbilityXpGain = "AbilityXpGain";

            public const string CraftingStrength = "CraftingStrength";
            public const string CraftingSlots = "CraftingSlot";
            public const string ActiveCrafts = "ActiveCraftingSlot";
            public const string CraftingSpeed = "CraftingSpeed";
            public const string CraftingCostEfficiency = "CraftingCostEfficiency";

            // Skill Types
            public const string AlchemySkill = "Skill_Alchemy";
            public const string AttackSkill = "Skill_Attack";
            public const string BuffSkill = "Skill_Buff";
            public const string DamageSkill = "Skill_Damage";
            public const string DabuffSkill = "Skill_Debuff";
            public const string DoTSkill = "Skill_DoT";
            public const string DurationSkill = "Skill_Duration";
            public const string InsightSkill = "Skill_Insight";
            public const string SpellSkill = "Skill_Spell";
            public const string TrickSkill = "Skill_Trick";

            // Skill Attributes
            public const string Duration = "Duration";
            public const string Unblockable = "Unblockable";

            // Situational modifiers
            public const string Local = "Local";
            public const string Global = "Global";

            public const string Attack = "Attack";
            public const string Spell = "Spell";
            public const string Insight = "Insight";
            public const string Trick = "Trick";

            public const string Buff = "Buff";
            public const string Debuff = "Debuff";

            public const string Hit = "Hit";
            public const string Projectile = "Projectile";

            public const string Resistance = "Resistance";
            public const string Evasion = "Evasion";

            public const string Elemental = "Elemental";
            public const string Status = "Status";
            public const string Stun = "Stun";
            public const string Slow = "Slow";
            public const string Intimidate = "Intimidate";
            public const string Prune = "Prune";

            public const string QualityPrefix = "+";
            public const string HandSuffix = "H";
            public const string OneHandedWeapon = $"1{HandSuffix}";
            public const string TwoHandedWeapon = $"2{HandSuffix}";
            public const string Weapon = "Weapon";
            public const string Armor = "Armor";
            public const string Shield = "Shield";
            public const string MainHand = "MainHand";
            public const string OffHand = "OffHand";
            
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
            public const string VsLowLife = "VsLowLife";
            public const string Evading = "Evading";
        }

        public static class DamageTypeTags
        {
            public const string Physical = "Physical";
            public const string Fire = "Fire";
            public const string Poison = "Poison";

            public static List<string> Types => [Physical, Fire, Poison];
        }

        public static class DropRestrictions
        {
            public const string MasterKey   = "MasterKey"; // Used when generating the full list of items to override restrictions

            public const string MaterialT4  = "MaterialT4";
            public const string Bow         = "Bow";
            public const string ClothItems  = "ClothItems";
        }

        public enum DamageType
        {
            Physical,
            Fire,
            Poison
        }

        public static class DamageTypeMethods
        {
            public static string ToTag(this DamageType type)
            {
                return type switch
                {
                    DamageType.Physical => DamageTypeTags.Physical,
                    DamageType.Fire => DamageTypeTags.Fire,
                    DamageType.Poison => DamageTypeTags.Poison,
                    _ => DamageTypeTags.Physical,
                };
            }

            public static HashSet<string> GetMatchingTags(this DamageType type)
            {
                HashSet<string> tags = [Tags.Damage, type.ToTag()];
                if (type == DamageType.Fire)
                {
                    tags.Add(Tags.Elemental);
                }
                // Hit vs. DoT
                tags.Add(type switch
                {
                    DamageType.Physical => Tags.Hit,
                    _ => Tags.DamageOverTime
                });
                return tags;
            }
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

        public static double CalculateAssumedPlayerHitPoints(int level)
        {
            return (Stats.BasePlayerHitPoints + Stats.PlayerHitPointPerLevel * (level - 1))
                * (1.0 + level * Stats.HitPointsBonusPerLevel); // Rough estimate for bonuses from perks and equipment
        }

        public static double CalculateAssumedPlayerDefenseMultiplier(int level)
        {
            var maxGearLevel = Stats.ScalingSwitchLevel;
            return (1.0 + CalculateAbilityDefenseBonus(level))                      // Ability defense bonus
				* Math.Pow(MaterialBonusPerLevel, Math.Min(level, maxGearLevel))    // Material scaling (4 tiers)
				* (1.0 + (0.2 / maxGearLevel * Math.Min(maxGearLevel, level)))      // Smooth transition to highest tier of armor
				* QualityBonusAtLevel(level)
                // Account for growth of player health pool
                * (1.0 + (level - 1) * (1.0 * Stats.PlayerHitPointPerLevel / Stats.BasePlayerHitPoints))
                //* (1.0 + (level - 1) * Stats.HitPointsBonusPerLevel) // Rough estimate for bonuses from perks and equipment
                ;
        }

        public static double CalculateDefenseRating(double armor, double evasion, int level)
        {
            double damage = 1.0; // use default damage for calculation
            double accuracy = CalculateMobAccuracy(level);
            double multiplier = CalculateArmorBonusMultiplier(armor, level, damage) * CalculateEvasionBonusMultiplier(evasion, accuracy);
            return 1.0 - (1.0 / multiplier);
        }

        public static double CalculateEvasionBonusMultiplier(double evasion, double enemyAccuracy)
        {
            double evasionChance = Math.Min(evasion / (evasion + enemyAccuracy), Stats.MaxResistanceFromEvasion);
            return 1.0 / (1.0 - evasionChance);
        }

        public static double CalculateArmorBonusMultiplier(double armor, int enemyLevel, double incomingDamage = 1.0)
        {
            if (incomingDamage == 0.0)
                incomingDamage = 1.0;
            double damage = CalculateMobDamage(enemyLevel, incomingDamage);
            if (armor == 0.0)
                return 1.0;
            double multiplier = Math.Max(damage / (damage + armor), 1.0 - Stats.MaxResistanceFromArmor);
            return 1.0 / multiplier;
        }

        public static double CalculateEncumbranceSlowdown(double encumbrance)
        {
            return 1.0 + Math.Max(encumbrance, 0.0) / 100.0;
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

        public static double CalculateMobDamage(int mobLevel, double multiplier = 1.0)
        {
            const double mobBaseDamage = Stats.BasePlayerHitPoints / 10.0;
            double hpMulti = CalculateAssumedPlayerHitPoints(mobLevel) / Stats.BasePlayerHitPoints;
            double assumedMitigationMulti = 1.0 + 0.02 * Math.Max(mobLevel - 6, 0);

            return mobBaseDamage * multiplier * hpMulti * assumedMitigationMulti;
        }

        public static double CalculateMobArmorPierce(int mobLevel, double multiplier = 1.0)
        {
            return multiplier
                * Math.Sqrt(CalculateMobDamage(mobLevel));
        }

        public static double CalculateMobAccuracy(int mobLevel)
        {
            // First implementation: Accuracy rating is identical to default damage value
            return CalculateMobDamage(mobLevel);
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
            double baseDuration = Stats.CraftingBaseDuration;
            if (item.GetBlueprint()?.GetMaterial()?.Id != MaterialId.Simple)
            {
                double matMulti = Math.Pow(item.GetBlueprint()!.GetMaterial().PowerMultiplier, 0.2);
                double tierMulti = Math.Pow(item.GetBlueprint()!.GetDropLevel() / 10, 0.2);
                double typeMulti = item.IsWeapon() ? Stats.CraftingCostWeaponMultiplier : 1.0;
                baseDuration *= matMulti * tierMulti * typeMulti;
            }
            double speed = crafter?.ApplyAllApplicableModifiers(1.0, 
                [Tags.CraftingSpeed], 
                crafter.GetTags()) ?? 1.0;
            
            // CornerCut: Minimum speed of 1% to prevent eternal crafts, realistically will never be below 1.0
            return Math.Ceiling(baseDuration / Math.Max(speed, 0.01));
        }

        public static int CalculateCraftingCost(Entity item, Entity? crafter)
        {
            int baseCost = item.GetComponent<ItemRefinableComponent>()?.Cost ?? 100;
            double efficiency = crafter?.ApplyAllApplicableModifiers(1.0, 
                [Tags.CraftingCostEfficiency],
                crafter.GetTags()) ?? 1.0;
            return (int)Math.Ceiling(Math.Max(baseCost / ((efficiency == 0) ? 0.1 : efficiency), 1.0));
        }

        public static double ApplyDamageRounding(double damage)
        {
            return Math.Round(damage);
        }

        public static double ApplyDefenseRounding(double defense)
        {
            return Math.Round(defense);
        }
    }
}
