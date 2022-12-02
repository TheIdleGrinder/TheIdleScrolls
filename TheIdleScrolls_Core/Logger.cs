using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;

namespace TheIdleScrolls_Core
{
    public static class Logger
    {
/*        public static string GetEntityName(Entity entity)
        {
            return entity.GetComponent<NameComponent>()?.Name ?? $"#{entity.Id}";
        }*/

        public static void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
