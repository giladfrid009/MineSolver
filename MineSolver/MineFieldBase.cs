using System;
using System.Linq;
using System.Collections.Generic;

namespace MineSolver
{
    public abstract class MineFieldBase 
    {
        public int Width { get; }
        public int Height { get; }

        public const int BombVal = -1;
        public const int HiddenVal = -2;

        public MineFieldBase(int width, int height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentOutOfRangeException();           

            Width = width;
            Height = height;
        }

        public int this[int x, int y]
        {
            get => GetCoordVal(x, y);
        }

        protected abstract int GetCoordVal(int x, int y);

        public abstract void Flag(int x, int y);

        public abstract void Unflag(int x, int y);

        public abstract bool Reveal(int x, int y);

        public List<(int X, int Y)> GetNeighbors(int x, int y)
        {
            List<(int, int)> neighbors = new List<(int, int)>(3);

            int xLow = Math.Max(0, x - 1);
            int xHigh = Math.Min(x + 1, Width - 1);
            int yLow = Math.Max(0, y - 1);
            int yHigh = Math.Min(y + 1, Height - 1);

            for (int xNeighbor = xLow; xNeighbor <= xHigh; xNeighbor++)
            {
                for (int yNeighbor = yLow; yNeighbor <= yHigh; yNeighbor++)
                {
                    if (xNeighbor == x && yNeighbor == y)
                        continue;

                    neighbors.Add((xNeighbor, yNeighbor));
                }
            }

            return neighbors;
        }

        public List<(int X, int Y)> GetHidden(int x, int y)
        {
            return GetNeighbors(x, y).Where(coord => GetCoordVal(coord.X, coord.Y) == HiddenVal).ToList();
        }

        public List<(int X, int Y)> GetRevealed(int x, int y)
        {
            return GetNeighbors(x, y).Where(coord => GetCoordVal(coord.X, coord.Y) >= 0).ToList();
        }

        public List<(int X, int Y)> GetFlagged(int x, int y)
        {
            return GetNeighbors(x, y).Where(coord => GetCoordVal(coord.X, coord.Y) == BombVal).ToList();
        }

        public abstract MineFieldBase Copy();
    }
}
