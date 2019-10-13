using System;
using System.Collections.Generic;

namespace MineSolver
{
    public abstract class MineFieldBase 
    {      
        public int Width { get; }
        public int Height { get; }

        public const int Mine = -1;
        public const int Hidden = -2;

        protected MineFieldBase (int width, int height)
        {
            Width = width;
            Height = height;
        }

        public abstract int this[int x, int y] { get; }

        public int this[(int x, int y) coord] { get => this[coord.x, coord.y]; }

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

        public abstract MineFieldBase Copy();
    }
}
