using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core.Items
{
    using ValueExtractor = Func<Entity, double>;
    using CandidateComparator = Func<Entity, Entity, ComparisonResult>;

    public record ComparisonResult(RelativeQuality Quality, double Difference);

    public enum RelativeQuality
    {
        Better,
        Equal,
        Worse
    }

    public static class ItemComparator
    {
        private static ValueExtractor GetDamage => item => item.GetComponent<WeaponComponent>()?.Damage ?? 0.0;
        private static ValueExtractor GetCooldown => item => item.GetComponent<WeaponComponent>()?.Cooldown ?? 0.0;
        private static ValueExtractor GetDps => item => (item.GetComponent<WeaponComponent>()?.Damage ?? 0.0) /
                                                        (item.GetComponent<WeaponComponent>()?.Cooldown ?? 1.0);
        private static ValueExtractor GetArmor => item => item.GetComponent<ArmorComponent>()?.Armor ?? 0.0;
        private static ValueExtractor GetEvasion => item => item.GetComponent<ArmorComponent>()?.Evasion ?? 0.0;
        private static ValueExtractor GetEncumbrance => item => item.GetComponent<EquippableComponent>()?.Encumbrance ?? 0.0;

        public static CandidateComparator CompareDamage      => (a, b) => Compare(a, b, GetDamage);
        public static CandidateComparator CompareCooldown    => (a, b) => Compare(a, b, GetCooldown, false);
        public static CandidateComparator CompareDps         => (a, b) => Compare(a, b, GetDps);
        public static CandidateComparator CompareArmor       => (a, b) => Compare(a, b, GetArmor);
        public static CandidateComparator CompareEvasion     => (a, b) => Compare(a, b, GetEvasion);
        public static CandidateComparator CompareEncumbrance => (a, b) => Compare(a, b, GetEncumbrance, false);

        private static ComparisonResult Compare(Entity firstItem, Entity secondItem, ValueExtractor valueExtractor, bool higherIsBetter = true)
        {
            return CompareValues(valueExtractor(firstItem), valueExtractor(secondItem), higherIsBetter);
        }

        private static ComparisonResult CompareValues(double first, double second, bool higherIsBetter)
        {
            if (first == second)
            {
                return new(RelativeQuality.Equal, 0.0);
            }
            bool firstIsBetter = (higherIsBetter && first > second) || (!higherIsBetter && first < second);
            return new(firstIsBetter ? RelativeQuality.Better : RelativeQuality.Worse, first - second);
        }
    }
}
