using System;
using System.Text;

namespace MineSolver
{
    public class MineField : MineFieldBase
    {
        private static readonly Random rnd = new Random();

        public char BombChar { get; set; } = '@';
        private char HiddenChar { get; set; } = ' ';

        private bool printOnline;
        private readonly int[,] fieldSolved;
        private readonly int[,] fieldUnsolved;

        public MineField(int Width, int Height) : base(Width, Height)
        {
            fieldSolved = new int[Width, Height];
            fieldUnsolved = new int[Width, Height];

            PrintOnlineDisable();
        }

        protected override int GetCoordVal(int x, int y)
        {
            return fieldUnsolved[x, y];
        }

        public override bool Reveal(int x, int y)
        {
            if (GetCoordVal(x, y) >= 0)
                return true;

            var coordVal = fieldSolved[x, y];

            if (coordVal == HiddenVal)
            {
                throw new Exception("Field not properly initialized.");
            }
            else if (coordVal == BombVal)
            {
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
            fieldUnsolved[x, y] = BombVal;

            if(printOnline)
            {
                PrintCoord(x, y);
            }
        }

        public override void Unflag(int x, int y)
        {
            fieldUnsolved[x, y] = HiddenVal;

            if (printOnline)
            {
                PrintCoord(x, y);
            }
        }

        public void Reset()
        {
            PrintOnlineDisable();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    fieldSolved[x, y] = HiddenVal;
                    fieldUnsolved[x, y] = HiddenVal;
                }
            }
        }

        public bool IsSolved()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (GetCoordVal(x, y) != fieldSolved[x, y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }   

        public void Generate(double bombPrecent, int xOrigin, int yOrigin, int originSize = 1)
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

            foreach (var (xN, yN) in GetNeighbors(xOrigin, yOrigin))
            {
                fieldSolved[xN, yN] = 0;

                GenerateOrigin(xN, yN, size - 1);
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
                while (fieldSolved[xB, yB] != HiddenVal);

                fieldSolved[xB, yB] = BombVal;
            }
        }

        private void GenerateVals()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (fieldSolved[x, y] == BombVal)
                        continue;

                    fieldSolved[x, y] = 0;

                    foreach (var (x2, y2) in GetNeighbors(x, y))
                    {
                        if (fieldSolved[x2, y2] == BombVal)
                        {
                            fieldSolved[x, y]++;
                        }
                    }
                }
            }
        }

        public void Print()
        {
            Console.WindowWidth = Console.LargestWindowWidth;
            Console.WindowHeight = Console.LargestWindowHeight;

            StringBuilder str = new StringBuilder(Width * Height + 2 * Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var coordVal = GetCoordVal(x, y);

                    if (coordVal == HiddenVal)
                    {
                        str.Append(HiddenChar);
                    }
                    else if (coordVal == BombVal)
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

            var coordVal = GetCoordVal(x, y);

            if (coordVal == HiddenVal)
            {
                Console.Write(HiddenChar);
            }
            else if (coordVal == BombVal)
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

        public override MineFieldBase Copy()
        {
            MineField newField = new MineField(Width, Height);
            Array.Copy(fieldSolved, 0, newField.fieldSolved, 0, Width * Height);
            Array.Copy(fieldUnsolved, 0, newField.fieldUnsolved, 0, Width * Height);

            return newField;
        }
    }
}
