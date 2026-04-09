using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.GameWorld;

namespace TheIdleScrolls_Core.Modifiers
{
    public interface IPerkCondition
    {
        string Description => GetDescription();

        bool IsSatisfied(Entity entity);

        string GetDescription();
    }
}
