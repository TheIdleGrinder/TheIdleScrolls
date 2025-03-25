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
                .OrderByDescending(t => TitleList.GetTitle(t)?.Priority)
                .FirstOrDefault(t => TitleList.GetTitle(t)?.IsPrefix == true) ?? "";
        }

        public string GetSuffixTitle()
        {
            return Titles
                .OrderByDescending(t => TitleList.GetTitle(t)?.Priority)
                .FirstOrDefault(t => TitleList.GetTitle(t)?.IsSuffix == true) ?? "";
        }
    }
}
