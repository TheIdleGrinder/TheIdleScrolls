using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
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
            step.PathId = Id;
            _Steps[step.Id] = step;
            return true;
        }

        public CharacterPathStep? GetStep(string id)
        {
            if (_Steps.TryGetValue(id, out var step))
                return step;
            return null;
        }

        public List<List<CharacterPathStep?>> BuildTopology(string initialStepId)
        {
            List<List<CharacterPathStep?>> topology = [];

            bool AlreadyMapped(string stepId)
            {
                return topology.Any(level => level.Any(step => step?.Id == stepId));
            }

            List<CharacterPathStep?>? ListWithStep(string stepId)
            {
                foreach (var row in topology)
                {
                    if (row.Any(step => step?.Id == stepId))
                        return row;
                }
                return null;
            }

            void IntegrateStep(CharacterPathStep step)
            {
                if (step.PrerequisiteId == null || step.PrerequisiteId == initialStepId)
                {
                    List<CharacterPathStep?> newRow = [];
                    if (step.StepNumber > 1)
                        newRow = [.. Enumerable.Repeat<CharacterPathStep?>(null, step.StepNumber - 1)];
                    newRow.Add(step);
                    topology.Add(newRow);
                    return;
                }
                var prereqRow = ListWithStep(step.PrerequisiteId);
                if (prereqRow == null)
                {
                    var prereqStep = GetStep(step.PrerequisiteId);
                    if (prereqStep == null)
                        return; // Prereq is missing, so this step can't be integrated
                    IntegrateStep(GetStep(step.PrerequisiteId)!);
                    prereqRow = ListWithStep(step.PrerequisiteId)!;
                }
                while (prereqRow.Count < step.StepNumber - 1)
                {
                    prereqRow.Add(null);
                }
                if (prereqRow.Count >= step.StepNumber)
                {
                    if (prereqRow[step.StepNumber - 1] is not null)
                    {
                        int index = topology.IndexOf(prereqRow);
                        List<CharacterPathStep?> newRow = [];
                        if (step.StepNumber > 1)
                            newRow = [.. Enumerable.Repeat<CharacterPathStep?>(null, step.StepNumber - 1)];
                        newRow.Add(step);
                        topology.Insert(index + 1, newRow);
                    }
                    prereqRow[step.StepNumber - 1] = step;
                }
                prereqRow.Add(step);
            }

            foreach (var step in Steps)
            {
                if (AlreadyMapped(step.Id))
                    continue;
                // Assume that no two steps in a row have the same tier
                // Also, no step has a prerequisite that has a higher tier than itself
                IntegrateStep(step);
            }

            return topology;
        }
    }
}
