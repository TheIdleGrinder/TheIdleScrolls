using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Modifiers;

namespace TheIdleScrolls_Core.Achievements
{
    public record PerkReward(Perk Perk) : IAchievementReward
    {
        public string Description => $"'{Perk.Name}' Perk";

        public bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback)
        {
            var perkComp = entity.GetComponent<PerksComponent>();
            if (perkComp == null)
                return false;
            if (!perkComp.HasPerk(Perk.Id))
            {
                perkComp.AddPerk(Perk);
            }
            // No need to send message, PerkSystem will update the perk and send one
            return true;
        }
    }
}
