using System;
using System.Collections.Generic;

namespace Minesolver
{
    public class MineField : MineFieldBase
    {
        private readonly Random rnd = new Random();

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

        public override int Reveal(int x, int y)
        {
            if (fieldUnsolved[x, y] != Hidden)
                return fieldUnsolved[x, y];

            var coordVal = fieldSolved[x, y];

            if (coordVal == Hidden)
            {
                throw new Exception("Field not properly initialized.");
            }
            else if (coordVal == Mine)
            {
                // todo: remove eventially
                throw new Exception("You lost.");
            }

            fieldUnsolved[x, y] = coordVal;

            if (coordVal == 0)
            {
                foreach (var (x2, y2) in GetNeighbors(x, y))
                {
                    Reveal(x2, y2);
                }
            }

            RaiseOnReveal(x, y, fieldUnsolved[x, y]);

            return fieldUnsolved[x, y];
        }

        public override void Flag(int x, int y)
        {
            fieldUnsolved[x, y] = Mine;

            RaiseOnFlag(x, y);
        }

        public override void Unflag(int x, int y)
        {
            fieldUnsolved[x, y] = Hidden;

            RaiseOnUnflag(x, y);
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
            List<(int, int)> freeCoords = new List<(int, int)>(Width * Height);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (fieldSolved[x, y] == Hidden)
                        freeCoords.Add((x, y));
                }
            }

            for (int i = 0; i < nMines; i++)
            {
                if (freeCoords.Count == 0)
                    return;

                int index = rnd.Next(0, freeCoords.Count);

                var (x, y) = freeCoords[index];

                fieldSolved[x, y] = Mine;

                freeCoords.RemoveAt(index);
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

        public override MineFieldBase Clone()
        {
            MineField copy = new MineField(Width, Height);

            Array.Copy(fieldSolved, 0, copy.fieldSolved, 0, fieldSolved.LongLength);
            Array.Copy(fieldUnsolved, 0, copy.fieldUnsolved, 0, fieldUnsolved.LongLength);

            return copy;
        }
    }
}
