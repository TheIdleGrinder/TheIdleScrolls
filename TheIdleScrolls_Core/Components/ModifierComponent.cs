using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Modifiers;

namespace TheIdleScrolls_Core.Components
{
    public class ModifierComponent : IComponent
    {
        readonly Dictionary<string, Modifier> Modifiers = new();

        public void AddModifier(Modifier modifier)
        {
            Modifiers[modifier.Id] = modifier;
        }

        public bool RemoveModifier(string id)
        {
            return Modifiers.Remove(id);
        }

        public void Clear()
        {
            Modifiers.Clear();
        }

        public double ApplyApplicableModifiers(double baseValue, IEnumerable<string> localTags, IEnumerable<string>? globalTags)
        {
            return Modifiers.Values.ApplyAllApplicable(baseValue, localTags, globalTags);
        }

        public List<Modifier> GetModifiers()
        {
            return Modifiers.Values.ToList();
        }
    }
}
