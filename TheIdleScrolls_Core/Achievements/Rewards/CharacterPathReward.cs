using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.CharacterPaths;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Achievements.Rewards
{
    public class CharacterPathReward(CharacterPath Path) : IAchievementReward
    {
        public string Description => $"{Path.Name}";

        public bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback)
        {
            var pathComponent = entity.GetComponent<CharacterPathComponent>();
            if (pathComponent is null)
                return false;
            
            // Only send message if this path was unlocked for the first time (not when loading char)
            if (!pathComponent.KnownPaths.Contains(Path.Id))
            {
                postMessageCallback(new TextMessage($"Unlocked '{Path.Name}'", IMessage.PriorityLevel.High));
            }
            pathComponent.AddPath(Path);

            foreach (var step in Path.Steps)
            {
                if (pathComponent.TakenSteps.Any(s => s.PathId == Path.Id && s.StepId == step.Id))
                {
                    // Don't send message for previously taken steps
                    step.Reward.GiveReward(entity, world, (message) => { });
                }
            }
            
            return true;
        }
    }
}
