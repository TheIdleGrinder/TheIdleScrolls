using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.CharacterPaths.Paths
{
    public static class RangerPath
    {
        const string PathId = "ranger";
        public static CharacterPath Path { get; } = new CharacterPath(PathId, Properties.Skills.PathRanger_Name, Properties.Skills.PathRanger_Description);

        static RangerPath()
        {

        }
    }
}
