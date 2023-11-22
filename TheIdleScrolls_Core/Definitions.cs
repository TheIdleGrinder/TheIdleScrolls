using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core
{
    public static class Definitions
    {
        public const double AttackBonusPerLevel = 0.1;
        public const double AttackDamagePerAbilityLevel = 0.02;
        public const double AttackSpeedPerAbilityLevel = 0.01;
        public const double DualWieldAttackSpeedMulti = 0.2;        

        public const double ArmorSlowdownPerPoint = 0.01;
        public const double EvasionBonusPerPoint = 0.01;

        public const double CraftingAbilityBonusPerLevel = 0.02;
    }

    public static class Functions
    {
        public static double CalculateAbilityAttackSpeedBonus(int abilityLevel)
        {
            return Math.Pow(1.0 + Definitions.AttackSpeedPerAbilityLevel, abilityLevel) - 1.0;
        }

        public static double CalculateAbilityAttackDamageBonus(int abilityLevel)
        {
            return Math.Pow(1.0 + Definitions.AttackDamagePerAbilityLevel, abilityLevel) - 1.0;
        }
    }
}
