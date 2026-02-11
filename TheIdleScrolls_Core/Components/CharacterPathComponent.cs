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
        public List<(string StepId, string PathId)> _TakenSteps = [];

        public int StepPoints { get; set; } = 0;
        public int StepsTaken => _TakenSteps.Count;
        public int RemainingStepPoints => StepPoints - StepsTaken;


        public List<CharacterPathStep> StepsTakenOnPath(string pathId)
        {
            return [.. _TakenSteps
                .Where(s => s.PathId == pathId)
                .Select(s => GetStep(s.PathId, s.StepId))
                .Where(s => s != null)
            ];
        }

        public CharacterPathStep? GetStep(string pathId, string stepId)
        {
            CharacterPath? path = CharacterPaths.FirstOrDefault(p => p.Id == pathId);
            if (path == null)
                return null;
            return path.GetStep(stepId);
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
            if (step == null || !step.CanBeTaken(StepsTakenOnPath(pathId)))
                return false;
            _TakenSteps.Add((step.Id, pathId));
            return true;
        }

        public List<CharacterPathStep> AvailableSteps(string pathId)
        {
            List<CharacterPathStep> result = [];
            var paths = CharacterPaths.Where(p => pathId.Length == 0 || p.Id == pathId);
            foreach (var path in paths)
            {
                result.AddRange(path.Steps.Where(s => s.CanBeTaken(StepsTakenOnPath(pathId))));
            }
            return result;
        }
    }
}
