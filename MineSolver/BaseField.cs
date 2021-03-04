using System;
using System.Collections.Generic;
using System.Text;

namespace Minesolver
{
    public abstract class BaseField
    {
        public event CoordFunc? OnLoss;
        public event CoordFunc? OnFlag;
        public event CoordFunc? OnUnflag;
        public event CoordValueFunc? OnReveal;

        public delegate void CoordFunc(int x, int y);
        public delegate void CoordValueFunc(int x, int y, int val);

        public int Width { get; }
        public int Height { get; }

        public const int Mine = -1;
        public const int Hidden = -2;

        protected BaseField(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public abstract int this[int x, int y] { get; }

        public int this[(int x, int y) coord] => this[coord.x, coord.y];

        public abstract void Flag(int x, int y);

        public abstract void Unflag(int x, int y);

        public abstract int Reveal(int x, int y);

        protected void RaiseOnFlag(int x, int y) => OnFlag?.Invoke(x, y);

        protected void RaiseOnUnflag(int x, int y) => OnUnflag?.Invoke(x, y);

        protected void RaiseOnReveal(int x, int y, int val) => OnReveal?.Invoke(x, y, val);

        protected void RaiseOnLose(int x, int y) => OnLoss?.Invoke(x, y);

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
                    {
                        continue;
                    }

                    neighbors.Add((xNeighbor, yNeighbor));
                }
            }

            return neighbors;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder(Width * Height + 2 * Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int val = this[x, y];

                    if (val == Hidden)
                    {
                        str.Append(' ');
                    }
                    else if (val == Mine)
                    {
                        str.Append('@');
                    }
                    else
                    {
                        str.Append(val);
                    }
                }

                str.Append('\n');
            }

            return str.ToString();
        }
    }
}
