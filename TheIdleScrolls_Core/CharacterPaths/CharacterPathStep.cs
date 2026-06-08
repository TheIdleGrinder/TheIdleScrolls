using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;

namespace TheIdleScrolls_Core.CharacterPaths
{
    public class CharacterPathStep(string id,
                                   string name,
                                   string description)
    {
        public string Id { get { return id; } }
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public HashSet<string> StepTags { get; init; } = [];
        public string? PathId { get; set; }
        public int StepNumber { get; init; } = 0;
        public string? PrerequisiteId { get; init; }
        public IAchievementReward Reward { get; init; }

        public bool CanBeTaken(List<CharacterPathStep> previousSteps)
        {
            return !previousSteps.Any(s => s.Id == Id)
                && (PrerequisiteId is null || previousSteps.Any(s => s.Id == PrerequisiteId))
                && StepsTakenOnPath(PathId, previousSteps) >= StepNumber;
        }

        public static int StepsTakenOnPath(string? pathId, List<CharacterPathStep> previousSteps)
        {
            if (pathId is null)
                return 0;
            return previousSteps.Count(s => s.PathId == pathId);
        }

        public static bool StepIsTaken(CharacterPathStep step, List<CharacterPathStep> steps)
        {
            return steps.Contains(step);
        }

        public static Func<List<CharacterPathStep>, bool> MakeStandardRequirements(string prereqId, int minStepCount)
        {
            return (previousSteps) =>
                previousSteps.Count >= minStepCount
                && (prereqId.Length == 0 || previousSteps.Any(s => s.Id == prereqId));
        }
    }
}
