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
        public HashSet<string> RequiredLocalTags { get; set; } = new();
        public HashSet<string> RequiredGlobalTags { get; set; } = new();

        public Modifier() { }

        public Modifier(string id, ModifierType type, double value, HashSet<string> localTags, HashSet<string> globalTags)
        {
            Id = id;
            Type = type;
            Value = value;
            RequiredLocalTags = localTags;
            RequiredGlobalTags = globalTags;
        }

        public bool IsApplicable(IEnumerable<string> localTags, IEnumerable<string>? globalTags)
        {
            return RequiredLocalTags.All(t => localTags.Contains(t)) 
                    && (globalTags == null || RequiredGlobalTags.All(t => globalTags.Contains(t)));
        }
    }

    public static class Modifiers
    {
        public static double ApplyAllApplicable(this IEnumerable<Modifier> modifiers, double baseValue, 
            IEnumerable<string> localTags, 
            IEnumerable<string>? globalTags)
        {
            double increase = 1.0;
            double multi = 1.0;
            double flat = 0.0;
            foreach (var mod in modifiers)
            {
                if (mod.IsApplicable(localTags, globalTags))
                {
                    switch (mod.Type)
                    {
                        case ModifierType.AddBase:
                            baseValue += mod.Value;
                            break;
                        case ModifierType.Increase:
                            increase += mod.Value;
                            break;
                        case ModifierType.More:
                            multi *= 1.0 + mod.Value;
                            break;
                        case ModifierType.AddFlat:
                            flat += mod.Value;
                            break;
                    }
                }
            }
            return baseValue * increase * multi + flat;
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
                Definitions.Tags.CraftingSlots,
                Definitions.Tags.ActiveCrafts,
                Definitions.Tags.CraftingSpeed,
                Definitions.Tags.CraftingCost
            };
            targetTags = targetTags.Where(t => modifier.RequiredLocalTags.Contains(t)).ToList();
            List<string> whileTags = new()
            {
                Definitions.Tags.Unarmed,
                Definitions.Tags.Unarmored,
                Definitions.Tags.DualWield,
            };
            whileTags = whileTags.Where(t => modifier.RequiredLocalTags.Contains(t)).ToList();
            List<string> withTags = modifier.RequiredLocalTags.Except(targetTags).Except(whileTags).ToList();

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
