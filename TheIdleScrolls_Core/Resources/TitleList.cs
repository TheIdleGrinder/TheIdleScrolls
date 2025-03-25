using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Resources
{
    public record Title(string Id, int Priority, bool IsPrefix)
    {
        public string Text => Id.Localize();
        public bool IsSuffix => !IsPrefix;
    };

    public static class TitleList
    {
        private static List<Title> _Titles =
            [
                new("Title_Swift", 1, false),
                new("Title_Prudent", 1, false),
                new("Title_Patient", 1, false),
                new("Title_Conqueror", 1, true),
                new("Title_ConquerorHC", 2, true),
            ];

        public static Title? GetTitle(string id)
        {
            return _Titles.FirstOrDefault(t => t.Id == id);
        } 
    }
}
