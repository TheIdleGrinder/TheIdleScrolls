using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Modifiers
{
    public enum ModifierType
    {
        AddBase,
        Increase,
        More,
        AddFlat
    }

    public class Modifier
    {
        public string Id { get; set; } = "";

        public ModifierType Type { get; set; }

        public double Value { get; set; }

        public (HashSet<string> All, HashSet<string> Any) RequiredTags { get; set; }

        public Modifier() { }

        public Modifier(string id, ModifierType type, double value, HashSet<string> tagsAll, HashSet<string> tagsAny)
        {
            Id = id;
            Type = type;
            Value = value;
            RequiredTags = (tagsAll, tagsAny);
        }

        public bool IsApplicable(IEnumerable<string> tags)
        {
            return (RequiredTags.All.All(t => tags.Contains(t)))
                && (RequiredTags.Any.Count == 0 || RequiredTags.Any.Any(t => tags.Contains(t)));
        }
    }

    public static class Modifiers
    {
        public static double ApplyAllApplicable(this IEnumerable<Modifier> modifiers, double baseValue, IEnumerable<string> tags)
        {
            var applicable = modifiers.Where(m => m.IsApplicable(tags));
            double result = applicable.Aggregate(baseValue, (total, mod) => total + (mod.Type == ModifierType.AddBase ? mod.Value : 0.0));
            result *= applicable.Aggregate(1.0, (total, mod) => total + (mod.Type == ModifierType.Increase ? mod.Value : 0.0));
            result = applicable.Aggregate(result, (total, mod) => total * (1.0 + (mod.Type == ModifierType.More ? mod.Value : 0.0)));
            result = applicable.Aggregate(result, (total, mod) => total + (mod.Type == ModifierType.AddFlat ? mod.Value : 0.0));
            return result;
        }
    }
}
