using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Resources;

namespace TheIdleScrolls_Core.Components
{
    public class TitleBearerComponent : IComponent
    {
        public HashSet<string> Titles { get; set; } = [];

        public void AddTitle(string title)
        {
            Titles.Add(title);
        }

        public string GetPrefixTitle()
        {
            return Titles
                .Select(TitleList.GetTitle)
                .Where(t => t != null)
                .OrderByDescending(t => t!.Priority)
                .FirstOrDefault(t => t!.IsPrefix == true)
                ?.Text
                ?? "";
        }

        public string GetSuffixTitle()
        {
            return Titles
                .Select(TitleList.GetTitle)
                .Where(t => t != null)
                .OrderByDescending(t => t!.Priority)
                .FirstOrDefault(t => t!.IsSuffix == true) 
                ?.Text
                ?? "";
        }
    }

    public record TitleEarnedMessage(Entity Character, string Title) : IMessage
    {
        string IMessage.BuildMessage() => $"{Character.GetName()} earned the title of '{Title}'";
        IMessage.PriorityLevel IMessage.GetPriority() => IMessage.PriorityLevel.VeryHigh;
    }
}
