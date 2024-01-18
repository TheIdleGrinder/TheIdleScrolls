using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            public const double CraftingAbilityBonusPerLevel = 0.02;

            public const int MobBaseHp = 20;
            public const double EarlyHpScaling = 1.056;
            public const double LaterHpScaling = 1.035;
            public const int ScalingSwitchLevel = 70;
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
        }

        public static class Tags
        {
            public const string RarityPrefix = "+";
            public const string HandSuffix = "H";
            public const string Shield = "Shield";
            public const string Unarmed = "Unarmed";
            public const string Unarmored = "Unarmored";
            public const string DualWield = "DualWield";
            public const string MixedWeapons = "MixedWeapons";
            public const string MixedArmor = "MixedArmor";
        }
    }

    public static class Functions
    {
        public static double CalculateAbilityAttackSpeedBonus(int abilityLevel)
        {
            return Math.Pow(1.0 + Definitions.Stats.AttackSpeedPerAbilityLevel, abilityLevel) - 1.0;
        }

        public static double CalculateAbilityAttackDamageBonus(int abilityLevel)
        {
            return Math.Pow(1.0 + Definitions.Stats.AttackDamagePerAbilityLevel, abilityLevel) - 1.0;
        }

        public static double CalculateAbilityDefenseBonus(int abilityLevel)
        {
            return Math.Pow(1.0 + Definitions.Stats.DefensePerAbilityLevel, abilityLevel) - 1.0;
        }

        public static int CalculateMobHp(int mobLevel, double multiplier = 1.0)
        {
            return (int) Math.Min(1_000_000_000, 
                Definitions.Stats.MobBaseHp * multiplier
                * Math.Pow(Definitions.Stats.EarlyHpScaling, Math.Min(mobLevel, Definitions.Stats.ScalingSwitchLevel))
                * Math.Pow(Definitions.Stats.LaterHpScaling, Math.Max(mobLevel - Definitions.Stats.ScalingSwitchLevel, 0))
                * (1.0 + Definitions.Stats.AttackBonusPerLevel * (mobLevel - 1))
            );
        }

        public static double CalculateMobDamage(int mobLevel, double multiplier = 1.0)
        {
            return multiplier
                * (1.0 + CalculateAbilityDefenseBonus(mobLevel)) // Scale parallel-ish to players armor ability
                * Math.Pow(Math.Pow(1.45, 1.0 / 20), Math.Min(mobLevel, Definitions.Stats.ScalingSwitchLevel) );
        }
    }
}
