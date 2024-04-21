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

        public HashSet<string> RequiredTags { get; set; } = new();

        public Modifier() { }

        public Modifier(string id, ModifierType type, double value, HashSet<string> tags)
        {
            Id = id;
            Type = type;
            Value = value;
            RequiredTags = tags;
        }

        public bool IsApplicable(IEnumerable<string> tags)
        {
            return RequiredTags.All(t => tags.Contains(t));
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

        public static string ToPrettyString(this Modifier modifier, bool showId = false)
        {
            List<string> targetTags = new() {
                Definitions.Tags.Damage,
                Definitions.Tags.AttackSpeed,
                Definitions.Tags.Defense,
                Definitions.Tags.ArmorRating,
                Definitions.Tags.EvasionRating,
                Definitions.Tags.CharacterXpGain,
                Definitions.Tags.AbilityXpGain,
                Definitions.Tags.CraftingSlot
            };
            targetTags = targetTags.Where(t => modifier.RequiredTags.Contains(t)).ToList();
            List<string> whileTags = new()
            {
                Definitions.Tags.Unarmed,
                Definitions.Tags.Unarmored,
                Definitions.Tags.DualWield,
            };
            whileTags = whileTags.Where(t => modifier.RequiredTags.Contains(t)).ToList();
            List<string> withTags = modifier.RequiredTags.Except(targetTags).Except(whileTags).ToList();

            double absValue = Math.Abs(modifier.Value);
            string valueString = (modifier.Type, modifier.Value >= 0) switch
            {
                (ModifierType.AddBase, true) => $"+{absValue:0.##}",
                (ModifierType.AddBase, false) => $"-{absValue:0.##}",
                (ModifierType.Increase, true) => $"{absValue:0.##%} increased",
                (ModifierType.Increase, false) => $"{absValue:0.##%} reduced",
                (ModifierType.More, true) => $"{absValue:0.##%} more",
                (ModifierType.More, false) => $"{absValue:0.##%} less",
                (ModifierType.AddFlat, _) => $"{modifier.Value:0.##} additional",
                _ => "??"
            };
            
            string idString = showId ? $"[{modifier.Id}] " : "";
            string target = String.Join(", ", targetTags.Select(s => s.Localize()));
            if (target == String.Empty)
                target = "???";

            string whileString = String.Join(", ", whileTags.Select(s => s.Localize()));
            string withString = String.Join(", ", withTags
                .Select(s => s.Localize() + (Definitions.Abilities.Weapons.Contains(s) ? "s" : "")));

            return $"{idString}{valueString} {target}" +
                $"{((withString.Length > 0) ? " with " : "")}{withString}" +
                $"{((whileString.Length > 0) ? " while " : "")}{whileString}";
        }
    }
}
