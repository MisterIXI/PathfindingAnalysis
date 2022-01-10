using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    class Coord
    {
        public int X;
        public int Y;

        public Coord(int newX, int newY)
        {
            X = newX;
            Y = newY;
        }

        public Coord(Coord toCopy)
        {
            X = toCopy.X;
            Y = toCopy.Y;
        }

        public void Move(int moveX, int moveY)
        {
            X += moveX;
            Y += moveY;
        }

        public bool Equals(Coord other)
        {
            if (other.X == X && other.Y == Y)
                return true;
            return false;
        }
    }
}
