using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.CharacterPaths;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using static TheIdleScrolls_Core.Systems.LevelUpSystem;

namespace TheIdleScrolls_Core.Systems
{
    public class CharacterPathSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            bool levelUpMessagePosted = coordinator.MessageTypeIsOnBoard<LevelUpMessage>();

            foreach (var entity in coordinator.GetEntities<CharacterPathComponent>())
            {
                var pathComp = entity.GetComponent<CharacterPathComponent>();
                if (pathComp is null)
                    continue;

                // Update available step points on level up or their count is zero
                // A character that has the component should always have at least one step point
                if (pathComp.StepPoints == 0 || levelUpMessagePosted)
                {
                    int previousPoints = pathComp.StepPoints;
                    int level = entity.GetComponent<LevelComponent>()?.Level ?? 0;
                    int basePoints = level / 10 + 1;
                    pathComp.StepPoints = basePoints;
                    if (previousPoints != 0 && pathComp.StepPoints != previousPoints)
                    {
                        coordinator.PostMessage(this, new CharacterPathPointsChanged(entity, previousPoints, pathComp.StepPoints));
                    }
                }
            }

            foreach (var message in coordinator.FetchMessagesByType<TakeCharacterPathStepRequest>())
            {
                var entity = coordinator.GetEntity(message.EntityId);
                var pathComp = entity?.GetComponent<CharacterPathComponent>();
                if (entity is null || pathComp is null)
                    continue;

                if (pathComp.RemainingStepPoints == 0)
                {
                    coordinator.PostMessage(this, new TextMessage($"{entity.GetName()} has no remaining path steps", IMessage.PriorityLevel.VeryHigh));
                    continue;
                }

                bool stepTaken = pathComp.TakeStep(message.PathId, message.StepId);
                if (stepTaken)
                {
                    CharacterPath? path = pathComp.CharacterPaths.FirstOrDefault(p => p.Id == message.PathId);
                    CharacterPathStep? step = path?.Steps.FirstOrDefault(s => s.Id == message.StepId);
                    if (path is not null && step is not null)
                    {
                        step.Reward.GiveReward(entity, world, (IMessage message) => coordinator.PostMessage(this, message));
                        coordinator.PostMessage(this, new CharacterPathStepTaken(entity, path, step));
                    }
                }
            }
        }
    }

    public record CharacterPathPointsChanged(Entity Entity, int PreviousPoints, int NewPoints) : IMessage
    {
        public string BuildMessage() => $"{Entity.GetName()} gained {NewPoints - PreviousPoints} character " +
            $"path point{(Math.Abs(PreviousPoints - NewPoints) > 1 ? "s" : "")}";
        public IMessage.PriorityLevel GetPriority() => IMessage.PriorityLevel.High;
    }

    public record TakeCharacterPathStepRequest(uint EntityId, string PathId, string StepId) : IMessage
    {
        public string BuildMessage() => $"Request for #{EntityId} to take step '{StepId}' on character path '{PathId}'";
        public IMessage.PriorityLevel GetPriority() => IMessage.PriorityLevel.Debug;
    }

    public record CharacterPathStepTaken(Entity Entity, CharacterPath Path, CharacterPathStep Step) : IMessage
    {
        public string BuildMessage() => $"{Entity.GetName()} took step '{Step.Name}' on character path '{Path.Name}'";
        public IMessage.PriorityLevel GetPriority() => IMessage.PriorityLevel.Medium;
    }
}
