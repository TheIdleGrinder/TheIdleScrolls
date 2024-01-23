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

        public double ApplyApplicableModifiers(double baseValue, IEnumerable<string> tags)
        {
            return Modifiers.Values.ApplyAllApplicable(baseValue, tags);
        }
    }
}
