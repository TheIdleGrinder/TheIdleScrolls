using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.CharacterPaths
{
    public class CharacterPath(string id, string name, string description)
    {
        public string Id { get { return id; } }
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public List<CharacterPathStep> Steps { get { return [.. _Steps.Values]; } }

        Dictionary<string, CharacterPathStep> _Steps { get; } = [];

        public bool AddStep(CharacterPathStep step)
        {
            if (_Steps.ContainsKey(step.Id))
                return false;
            step.Path = this;
            _Steps[step.Id] = step;
            return true;
        }

        public CharacterPathStep? GetStep(string id)
        {
            if (_Steps.TryGetValue(id, out var step))
                return step;
            return null;
        }
    }
}
