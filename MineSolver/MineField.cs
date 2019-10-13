using System;
using System.Text;

namespace MineSolver
{
    public class MineField : MineFieldBase
    {
        private readonly Random rnd = new Random();

        public char MineChar { get; set; } = '@';
        private char HiddenChar { get; set; } = ' ';

        private bool printOnline = false;

        private readonly int[,] fieldSolved;
        private readonly int[,] fieldUnsolved;

        public MineField(int width, int height, int? seed = null) : base(width, height)
        {
            fieldSolved = new int[Width, Height];
            fieldUnsolved = new int[Width, Height];

            rnd = seed == null ? new Random() : new Random(seed.Value);
        }

        public override int this[int x, int y]
        {
            get => fieldUnsolved[x, y];
        }

        public override bool Reveal(int x, int y)
        {
            if (fieldUnsolved[x, y] >= 0)
                return true;

            var coordVal = fieldSolved[x, y];

            if (coordVal == Hidden)
            {
                throw new Exception("Field not properly initialized.");
            }
            else if (coordVal == Mine)
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

        public override void Flag(int x, int y)
        {
            fieldUnsolved[x, y] = Mine;

            if(printOnline)
            {
                PrintCoord(x, y);
            }
        }

        public override void Unflag(int x, int y)
        {
            fieldUnsolved[x, y] = Hidden;

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
                    fieldSolved[x, y] = Hidden;
                    fieldUnsolved[x, y] = Hidden;
                }
            }
        }

        public void Generate(double minePrecent, int xOrigin, int yOrigin, int originSize = 2)
        {
            Reset();

            GenerateOrigin(xOrigin, yOrigin, originSize);

            GenerateMines((int)(minePrecent * Width * Height));

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

        private void GenerateMines(int nMines)
        {
            for (int i = 0; i < nMines; i++)
            {
                int xB;
                int yB;

                do
                {
                    xB = rnd.Next(0, Width);
                    yB = rnd.Next(0, Height);
                }
                while (fieldSolved[xB, yB] != Hidden);

                fieldSolved[xB, yB] = Mine;
            }
        }

        private void GenerateVals()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (fieldSolved[x, y] == Mine)
                        continue;

                    fieldSolved[x, y] = 0;

                    foreach (var (x2, y2) in GetNeighbors(x, y))
                    {
                        if (fieldSolved[x2, y2] == Mine)
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

                    if (coordVal == Hidden)
                    {
                        str.Append(HiddenChar);
                    }
                    else if (coordVal == Mine)
                    {
                        str.Append(MineChar);
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

        public void EnablePrintOnline()
        {
            Console.Clear();
            Print();
            printOnline = true;
        }

        public void DisablePrintOnline()
        {
            printOnline = false;
        }

        private void PrintCoord(int x, int y)
        {
            Console.SetCursorPosition(x, y);

            var coordVal = fieldUnsolved[x, y];

            if (coordVal == Hidden)
            {
                Console.Write(HiddenChar);
            }
            else if (coordVal == Mine)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(MineChar);
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

        public override MineFieldBase Copy()
        {
            MineField copy = new MineField(Width, Height);

            Array.Copy(fieldSolved, 0, copy.fieldSolved, 0, fieldSolved.LongLength);
            Array.Copy(fieldUnsolved, 0, copy.fieldUnsolved, 0, fieldUnsolved.LongLength);

            return copy;
        }
    }
}
