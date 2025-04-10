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
    using CandidateComparator = Func<Entity, List<Entity>, List<RelativeValue>>;

    public enum RelativeQuality
    {
        Better,
        Equal,
        Worse
    }

    public enum RelativeValue
    {
        Higher,
        Equal,
        Lower
    }

    public static class Extensions
    {
        public static RelativeQuality ToRelativeQuality(this RelativeValue value, bool higherIsBetter = true)
        {
            if (value == RelativeValue.Equal)
            {
                return RelativeQuality.Equal;
            }
            else
            {
                return (higherIsBetter && value == RelativeValue.Higher) || (!higherIsBetter && value == RelativeValue.Lower) 
                        ? RelativeQuality.Better 
                        : RelativeQuality.Worse;
            }
        }
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
        public static CandidateComparator CompareCooldown    => (a, b) => Compare(a, b, GetCooldown);
        public static CandidateComparator CompareDps         => (a, b) => Compare(a, b, GetDps);
        public static CandidateComparator CompareArmor       => (a, b) => Compare(a, b, GetArmor);
        public static CandidateComparator CompareEvasion     => (a, b) => Compare(a, b, GetEvasion);
        public static CandidateComparator CompareEncumbrance => (a, b) => Compare(a, b, GetEncumbrance);

        private static RelativeValue Compare(Entity firstItem, Entity secondItem, ValueExtractor valueExtractor)
        {
            return CompareValues(valueExtractor(firstItem), valueExtractor(secondItem));
        }

        private static List<RelativeValue> Compare(Entity item, List<Entity> candidates, 
                                                            ValueExtractor valueExtractor)
        {
            return candidates.Select(candidate => Compare(item, candidate, valueExtractor)).ToList();
        }

        private static RelativeValue CompareValues(double first, double second)
        {
            if (first == second)
            {
                return RelativeValue.Equal;
            }
            return first > second ? RelativeValue.Higher : RelativeValue.Lower;
        }
    }
}
