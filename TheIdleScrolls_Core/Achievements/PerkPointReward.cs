using MiniECS;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrolls_Core.Achievements
{
    internal class PerkPointReward(HashSet<string> PointIds) : IAchievementReward
    {
        public PerkPointReward(string id) : this([id]) { }

        public string Description => $"+{PointIds.Count} perk point{(PointIds.Count != 1 ? "s" : "")}";

        public bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback)
        {
            var perksComp = entity.GetComponent<PerksComponent>();
            if (perksComp == null)
                return false;
            foreach (var pointId in PointIds)
            {
                perksComp.AddBonusPerkPoint(pointId);
            }
            return true;
        }
    }
}
