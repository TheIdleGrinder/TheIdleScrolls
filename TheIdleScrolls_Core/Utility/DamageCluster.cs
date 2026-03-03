using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;

namespace TheIdleScrolls_Core.Utility
{
    public class DamageCluster
    {
        readonly Dictionary<DamageType, double> DamageComponents = [];

        public DamageCluster() { }

        public DamageCluster(Dictionary<DamageType, double> components)
        {
            DamageComponents = components;
        }

        public DamageCluster(DamageType type, double amount)
        {
            DamageComponents[type] = amount;
        }

        public List<DamageType> Types => [.. DamageComponents.Keys];

        public double TotalDamage => DamageComponents.Values.Sum();

        public double DamageOfType(DamageType type)
        {
            return DamageComponents.GetValueOrDefault(type, 0.0);
        }

        public void AddDamage(DamageType type, double amount)
        {
            if (DamageComponents.ContainsKey(type))
            {
                DamageComponents[type] += amount;
            }
            else
            {
                DamageComponents[type] = amount;
            }
        }

        public void SetDamage(DamageType type, double amount)
        {
            DamageComponents[type] = amount;
        }

        public void Add(DamageCluster other)
        {
            foreach (var type in other.Types)
            {
                AddDamage(type, other.DamageOfType(type));
            }
        }

        public static DamageCluster operator +(DamageCluster a, DamageCluster b)
        {
            DamageCluster result = new(a.DamageComponents);
            result.Add(b);
            return result;
        }

        public static DamageCluster operator *(DamageCluster cluster, double factor)
        {
            return cluster.Multiply(factor);
        }

        public static DamageCluster operator *(double factor, DamageCluster cluster)
        {
            return cluster.Multiply(factor);
        }

        public DamageCluster Multiply(double factor)
        {
            DamageCluster result = new();
            foreach (var type in Types)
            {
                result.AddDamage(type, DamageOfType(type) * factor);
            }
            return result;
        }

        public DamageCluster ScaleWithModifiers(IEnumerable<Modifier> modifiers, IEnumerable<string> localTags, IEnumerable<string> globalTags)
        {
            DamageCluster result = new();
            foreach (var type in Enum.GetValues<DamageType>().Cast<DamageType>())
            {
                double value = modifiers.ApplyAllApplicable(DamageOfType(type),
                    localTags.Concat(type.GetMatchingTags()),
                    globalTags);
                if (value > 0.0)
                {
                    result.AddDamage(type, value);
                }
            }
            return result;

        }
    }
}
