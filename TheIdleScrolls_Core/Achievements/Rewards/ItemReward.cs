using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.GameWorld;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrolls_Core.Achievements.Rewards
{
    public class ItemReward(ItemBlueprint Blueprint) : IAchievementReward
    {
        public string Description => $"Item: {Blueprint.GetItemName()}";

        public bool GiveReward(Entity entity, World world, Action<IMessage> postMessageCallback)
        {
            // If this is executed before the world is active, i.e. when loading the character, the rewards has already been
            // given during a previous session.
            // CornerCut: This can become problematic as soon as item rewards are given for achievements instead of path steps
            if (!world.Active)
            {
                return true;
            }
            var itemComp = entity.GetComponent<InventoryComponent>();
            if (itemComp is null)
                return false;
            var item = ItemFactory.MakeItem(Blueprint);
            if (item is null)
                return false;
            itemComp.AddItem(item);
            postMessageCallback(new ItemReceivedMessage(entity, item));
            return true;
        }
    }
}
