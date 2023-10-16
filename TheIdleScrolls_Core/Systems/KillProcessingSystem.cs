using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Systems
{
    public class KillProcessingSystem : AbstractSystem
    {
        public override void Update(World world, Coordinator coordinator, double dt)
        {
            HashSet<uint> deaths = new();
            foreach (var victim in coordinator.GetEntities<KilledComponent>())
            {
                deaths.Add(victim.Id);
                var killerId = victim.GetComponent<KilledComponent>()?.Killer ?? 0;
                if (killerId != 0 && coordinator.HasEntity(killerId))
                {
                    var killer = coordinator.GetEntity(killerId) ?? throw new Exception("Killer not found");
                    if (victim.HasComponent<XpGiverComponent>() && killer.HasComponent<XpGainerComponent>())
                    {
                        int sourceLevel = victim.GetComponent<LevelComponent>()?.Level ?? 1;
                        int targetLevel = killer.GetComponent<LevelComponent>()?.Level ?? sourceLevel;
                        double overlevelMalus = Math.Pow(0.975, Math.Max(0, targetLevel - sourceLevel));

                        int xp = victim.GetComponent<XpGiverComponent>()?.Amount ?? 0;
                        xp = (int)Math.Round(xp * world.XpMultiplier * overlevelMalus);
                        killer.GetComponent<XpGainerComponent>()?.AddXp(xp);
                        coordinator.PostMessage(this, new XpGainMessage(killer, xp));
                    }
                }
            }

            foreach (uint victim in deaths)
            {
                coordinator.RemoveEntity(victim);
            }
        }
    }

    public class XpGainMessage : IMessage
    {
        public Entity Recipient { get; set; }
        public int XP { get; set; }

        public XpGainMessage(Entity recipient, int xp)
        {
            Recipient = recipient;
            XP = xp;
        }

        string IMessage.BuildMessage()
        {
            string message = $"{Recipient.GetName()} received {XP:#,0} XP";
            return message;
        }

        IMessage.PriorityLevel IMessage.GetPriority()
        {
            return IMessage.PriorityLevel.Medium;
        }
    }
}
