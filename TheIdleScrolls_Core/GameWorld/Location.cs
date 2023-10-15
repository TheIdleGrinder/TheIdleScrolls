using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    public record Location(int X = 0, int Y = 0)
    {
        public override string ToString()
        {
            return $"{X}/{Y}";
        }

        public static Location FromString(string serialized)
        {
            var parts = serialized.Split('/');
            if (parts.Length != 2)
                throw new ArgumentException($"Not a valid Location string: '{serialized}'");
            return new(Int32.Parse(parts[0]), Int32.Parse(parts[1]));
        }
    }
}
