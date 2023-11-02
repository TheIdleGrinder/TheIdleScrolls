using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    internal class SpiralMapPath : IMapProgressPath
    {
        const int MinLevel = 1;

        public int? LocationLevel(Location location)
        {
            int ring = CalcRingIndex(location);
            if (location.Y == -ring || location.X == -ring) // bottom or left side of the square
            {
                return MaxRingLevel(ring) - 2 * ring + location.X - location.Y;
            }
            else // right or top side
            {
                return MinRingLevel(ring) + 2 * ring - 1 - location.X + location.Y;
            }
        }

        public Location? NextLocation(Location location)
        {
            int x = location.X;
            int y = location.Y;

            if (x > Math.Abs(y)) // Right side of the square, but not at the top yet
                return new(x, y + 1); // move up
            else if (y > Math.Abs(x)) // top side of the square, but not at the left end yet
                return new(x - 1, y); // move left
            else if (-x > Math.Abs(y)) // left side of the square, but not at the bottom yet
                return new(x, y - 1); // move down
            else
                return new(x + 1, y); // move right
        }

        public Location? PreviousLocation(Location location)
        {
            int x = location.X;
            int y = location.Y;

            throw new NotImplementedException();
        }

        private int CalcRingIndex(Location location)
        {
            return Math.Max(Math.Abs(location.X), Math.Abs(location.Y));
        }

        private int MinRingLevel(int ringIndex)
        {
            return (ringIndex == 0) ? MinLevel : (int)(Math.Pow(2 * ringIndex - 1, 2) + MinLevel);
        }

        private int MaxRingLevel(int ringIndex)
        {
            return (int)(Math.Pow(2 * ringIndex + 1, 2) - 1 + MinLevel);
        }
    }
}
