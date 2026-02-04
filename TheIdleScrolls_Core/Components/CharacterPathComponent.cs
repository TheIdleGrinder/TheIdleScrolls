using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.CharacterPaths;

namespace TheIdleScrolls_Core.Components
{
    public class CharacterPathComponent : IComponent
    {
        public List<CharacterPath> CharacterPaths { get; } = [];
        Dictionary<string, List<CharacterPathStep>> _TakenSteps = [];

        public List<CharacterPathStep> StepsTakenOnPath(string pathId)
        {
            return _TakenSteps.TryGetValue(pathId, out List<CharacterPathStep>? steps) ? steps! : [];
        }

        public void AddPath(CharacterPath path)
        {
            if (CharacterPaths.Contains(path)) 
                return;
            CharacterPaths.Add(path);
        }

        public bool TakeStep(string pathId, string id)
        {
            CharacterPath? path = CharacterPaths.FirstOrDefault(p => p.Id == pathId);
            if (path == null)
                return false;

            CharacterPathStep? step = path.Steps.FirstOrDefault(s => s.Id == id);
            if (step == null || !step.CanBeTaken(_TakenSteps[pathId]))
                return false;
            _TakenSteps[pathId].Add(step);
            return true;
        }

        public List<CharacterPathStep> AvailableSteps(string pathId)
        {
            List<CharacterPathStep> result = [];
            var paths = CharacterPaths.Where(p => pathId.Length == 0 || p.Id == pathId);
            foreach (var path in paths)
            {
                result.AddRange(path.Steps.Where(s => s.CanBeTaken(_TakenSteps[path.Id])));
            }
            return result;
        }
    }
}
