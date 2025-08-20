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
                new(Definitions.Titles.Swift, 1, false),
                new(Definitions.Titles.Prudent, 1, false),
                new(Definitions.Titles.Patient, 1, false),
                new(Definitions.Titles.Conqueror, 1, true),
                new(Definitions.Titles.ConquerorHC, 2, true),
            ];

        public static Title? GetTitle(string id)
        {
            return _Titles.FirstOrDefault(t => t.Id == id);
        } 
    }
}
