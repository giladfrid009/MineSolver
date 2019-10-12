using System;
using System.Text;
using System.Collections.Generic;

namespace MineSolver
{
    public class MineField : IMIneField
    {
        private readonly Random rnd = new Random();

        public int Width { get; }
        public int Height { get; }
        public int ValMine { get; } = -1;
        public int ValHidden { get; } = -2;
        public char BombChar { get; set; } = '@';
        private char HiddenChar { get; set; } = ' ';

        private bool printOnline;

        private readonly int[,] fieldSolved;
        private readonly int[,] fieldUnsolved;

        public MineField(int width, int height, int? seed = null)
        {
            Width = width;
            Height = height;

            fieldSolved = new int[width, height];
            fieldUnsolved = new int[width, height];

            rnd = seed == null ? new Random() : new Random(seed.Value);

            PrintOnlineDisable();
        }

        public int this[int x, int y]
        {
            get => fieldUnsolved[x, y];
        }

        public int this[(int x, int y) coord]
        {
            get => fieldUnsolved[coord.x, coord.y];
        }

        public bool Reveal(int x, int y)
        {
            if (fieldUnsolved[x, y] >= 0)
                return true;

            var coordVal = fieldSolved[x, y];

            if (coordVal == ValHidden)
            {
                throw new Exception("Field not properly initialized.");
            }
            else if (coordVal == ValMine)
            {
                throw new Exception("You lost.");
                return false;
            }

            fieldUnsolved[x, y] = coordVal;

            if (coordVal == 0)
            {
                foreach (var (x2, y2) in GetNeighbors(x, y))
                {
                    Reveal(x2, y2);
                }
            }

            if (printOnline)
            {
                PrintCoord(x, y);
            }

            return true;
        }

        public void Flag(int x, int y)
        {
            fieldUnsolved[x, y] = ValMine;

            if(printOnline)
            {
                PrintCoord(x, y);
            }
        }

        public void Unflag(int x, int y)
        {
            fieldUnsolved[x, y] = ValHidden;

            if (printOnline)
            {
                PrintCoord(x, y);
            }
        }

        public void Reset()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    fieldSolved[x, y] = ValHidden;
                    fieldUnsolved[x, y] = ValHidden;
                }
            }
        }

        public bool IsSolved()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (fieldUnsolved[x, y] != fieldSolved[x, y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected List<(int, int)> GetNeighbors(int x, int y)
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

        public void Generate(double bombPrecent, int xOrigin, int yOrigin, int originSize = 2)
        {
            Reset();

            GenerateOrigin(xOrigin, yOrigin, originSize);

            GenerateBombs((int)(bombPrecent * Width * Height));

            GenerateVals();
        }

        private void GenerateOrigin(int xOrigin, int yOrigin, int size)
        {
            if (size <= 0 || fieldSolved[xOrigin, yOrigin] == 0)
                return;

            fieldSolved[xOrigin, yOrigin] = 0;

            foreach (var (x2, y2) in GetNeighbors(xOrigin, yOrigin))
            {
                GenerateOrigin(x2, y2, size - 1);
            }
        }

        private void GenerateBombs(int nBombs)
        {
            for (int i = 0; i < nBombs; i++)
            {
                int xB;
                int yB;

                do
                {
                    xB = rnd.Next(0, Width);
                    yB = rnd.Next(0, Height);
                }
                while (fieldSolved[xB, yB] != ValHidden);

                fieldSolved[xB, yB] = ValMine;
            }
        }

        private void GenerateVals()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (fieldSolved[x, y] == ValMine)
                        continue;

                    fieldSolved[x, y] = 0;

                    foreach (var (x2, y2) in GetNeighbors(x, y))
                    {
                        if (fieldSolved[x2, y2] == ValMine)
                        {
                            fieldSolved[x, y]++;
                        }
                    }
                }
            }
        }

        public void Print()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.CursorVisible = false;
            Console.WindowWidth = Console.LargestWindowWidth;
            Console.WindowHeight = Console.LargestWindowHeight;

            StringBuilder str = new StringBuilder(Width * Height + 2 * Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var coordVal = fieldUnsolved[x, y];

                    if (coordVal == ValHidden)
                    {
                        str.Append(HiddenChar);
                    }
                    else if (coordVal == ValMine)
                    {
                        str.Append(BombChar);
                    }
                    else if (coordVal >= 0)
                    {
                        str.Append(coordVal);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Unsupported value.");
                    }
                }
                str.Append("\n");
            }

            Console.Write(str);
        }

        public void PrintOnlineEnable()
        {
            Console.Clear();
            Print();
            printOnline = true;
        }

        public void PrintOnlineDisable()
        {
            printOnline = false;
        }

        private void PrintCoord(int x, int y)
        {
            Console.SetCursorPosition(x, y);

            var coordVal = fieldUnsolved[x, y];

            if (coordVal == ValHidden)
            {
                Console.Write(HiddenChar);
            }
            else if (coordVal == ValMine)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(BombChar);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else if (coordVal >= 0)
            {
                Console.Write(coordVal);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Unsupported value.");
            }
        }

        public IMIneField Copy()
        {
            MineField copy = new MineField(Width, Height);

            Array.Copy(fieldSolved, 0, copy.fieldSolved, 0, fieldSolved.LongLength);
            Array.Copy(fieldUnsolved, 0, copy.fieldUnsolved, 0, fieldUnsolved.LongLength);

            return copy;
        }
    }
}
