using System;
using System.Text;
using System.Collections.Generic;

namespace MineSolver
{
    public abstract class MineFieldBase 
    {
        public event PrintMineFunc OnFlag;
        public event PrintMineFunc OnUnflag;
        public event PrintValFunc OnReveal;

        public delegate void PrintMineFunc(int x, int y);
        public delegate void PrintValFunc(int x, int y, int val);

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

        public abstract int Reveal(int x, int y);

        protected void RaiseOnFlag(int x, int y)
        {
            OnFlag?.Invoke(x, y);
        }

        protected void RaiseOnUnflag(int x, int y)
        {
            OnUnflag?.Invoke(x, y);
        }

        protected void RaiseOnReveal(int x, int y, int val)
        {
            OnReveal?.Invoke(x, y, val);
        }

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

        public void Print(char mineChar = '@')
        {
            StringBuilder str = new StringBuilder(Width * Height + 2 * Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var val = this[x, y];

                    if (val == Hidden)
                    {
                        str.Append(' ');
                    }
                    else if (val == Mine)
                    {
                        str.Append(mineChar);
                    }
                    else
                    {
                        str.Append(val);
                    }                   
                }
                str.Append("\n");
            }

            Console.Write(str);
        }

        public abstract MineFieldBase Copy();
    }
}
