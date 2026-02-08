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
            pathComponent.AddPath(Path);
            return true;
        }
    }
}
