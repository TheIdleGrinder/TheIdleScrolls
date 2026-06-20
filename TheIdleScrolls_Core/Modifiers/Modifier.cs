using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;

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
        
        // Used only for display string. Makes AddBase and AddFlat show as percentages
        public bool AlwaysPercentage { get; set; } = false;
        public double? CoverValue { get; set; } = null;
        public string? CoverText { get; set; } = null;

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
            var allTags = modifier.RequiredLocalTags.Union(modifier.RequiredGlobalTags);

            List<string> withTags = [Tags.FirstStrike, .. Abilities.All];
            withTags.RemoveAll(t => !allTags.Contains(t));

            List<string> whileTags =
            [
                Tags.DualWield,
                Tags.Evading,                
                Tags.Shielded,
                Tags.SingleHanded,
                Tags.TwoHanded,
                Tags.Unarmed,
                Tags.Unarmored
            ];
            whileTags = whileTags.Where(t => allTags.Contains(t)).ToList();

            List<string> localGlobal = allTags.Where(t => t == Tags.Local || t == Tags.Global).ToList();
            List<string> damageTypes = allTags.Where(DamageTypeTags.Types.Contains).ToList();

            List<string> targetTags = allTags.Except(withTags).Except(whileTags).Except(localGlobal).Except(damageTypes).ToList();

            double absValue = Math.Abs(modifier.CoverValue ?? modifier.Value);
            string valueString = (modifier.Type, modifier.Value >= 0, modifier.AlwaysPercentage) switch
            {
                (ModifierType.AddBase, true, false) => $"+{absValue:0.##}",
                (ModifierType.AddBase, false, false) => $"-{absValue:0.##}",
                (ModifierType.AddBase, true, true) => $"+{absValue:0.##%}",
                (ModifierType.AddBase, false, true) => $"-{absValue:0.##%}",
                (ModifierType.Increase, true, _) => $"{absValue:0.##%} increased",
                (ModifierType.Increase, false, _) => $"{absValue:0.##%} reduced",
                (ModifierType.More, true, _) => $"{absValue:0.##%} more",
                (ModifierType.More, false, _) => $"{absValue:0.##%} less",
                (ModifierType.AddFlat, _, true) => $"{absValue:0.##%} additional",
                (ModifierType.AddFlat, _, false) => $"{absValue:0.##} additional",
                _ => "??"
            };
            
            string idString = showId ? $"[{modifier.Id}] " : "";

            if (modifier.CoverText != null)
            {
                valueString = modifier.CoverText.Replace("{0}", valueString);
                return valueString;
            }

            string target = String.Join(", ", targetTags.Select(s => s.Localize()));
            if (target == String.Empty)
                target = "???";
            string localGlobalString = String.Join(", ", localGlobal.Select(s => s.Localize()));
            if (localGlobalString.Length > 0)
                localGlobalString += " ";

            string damageTypeString = String.Join(", ", damageTypes.Select(s => s.Localize()));
            if (damageTypeString.Length > 0)
                damageTypeString += " ";

            string whileString = String.Join(" and ", whileTags.Select(s => s.Localize()));
            string withString = String.Join(", ", withTags
                .Select(s => s.Localize() + (Abilities.Weapons.Contains(s) ? " weapons" : "")));

            return $"{idString}{valueString} {localGlobalString}{damageTypeString}{target}" +
                $"{((withString.Length > 0) ? " with " : "")}{withString}" +
                $"{((whileString.Length > 0) ? " while " : "")}{whileString}";
        }
    }
}
