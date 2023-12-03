using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.GameWorld
{
    internal class SpiralMapPath : IMapProgressPath
    {
        private readonly int MinLevel = 1;

        public SpiralMapPath(int minLevel = 1)
        {
            MinLevel = minLevel;
        }

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
            int r = CalcRingIndex(location);

            if (x > Math.Abs(y)) // Right side of the square, but not at the top yet
                return new(x, y + 1); // move up
            else if (y == r && x > -r) // top side of the square, but not at the left end yet
                return new(x - 1, y); // move left
            else if (x == -r && y > -r) // left side of the square, but not at the bottom yet
                return new(x, y - 1); // move down
            else
                return new(x + 1, y); // move right
        }

        public Location? PreviousLocation(Location location)
        {
            int x = location.X;
            int y = location.Y;

            if (x == 0 && y == 0)
                return null;

            Location left = new(x - 1, y);
            if (NextLocation(left) == location)
                return left;
            Location up = new(x, y + 1);
            if (NextLocation(up) == location)
                return up;
            Location right = new(x + 1, y);
            if (NextLocation(right) == location)
                return right;
            Location down = new(x, y - 1);
            if (NextLocation(down) == location)
                return down;
            return null;
        }

        private static int CalcRingIndex(Location location)
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
