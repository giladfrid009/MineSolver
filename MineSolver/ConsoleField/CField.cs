using System;
using System.Collections.Generic;

namespace Minesolver.ConsoleField
{
    public class CField : FieldBase
    {
        private static readonly Random staticRnd = new Random();

        protected readonly int[,] fieldSolved;
        protected readonly int[,] fieldUnsolved;

        public CField(int width, int height) : base(width, height)
        {
            fieldSolved = new int[Width, Height];
            fieldUnsolved = new int[Width, Height];
        }

        public void Set(int x, int y, int val)
        {
            fieldUnsolved[x, y] = val;
        }

        public override int this[int x, int y] => fieldUnsolved[x, y];

        public override int Reveal(int x, int y)
        {
            if (fieldUnsolved[x, y] != Hidden)
            {
                return fieldUnsolved[x, y];
            }

            int coordVal = fieldSolved[x, y];

            if (coordVal == Hidden)
            {
                throw new Exception("Field not properly initialized.");
            }
            else if (coordVal == Mine)
            {
                RaiseOnLose(x, y);
                return Mine;
            }

            fieldUnsolved[x, y] = coordVal;

            if (coordVal == 0)
            {
                foreach ((int x2, int y2) in GetNeighbors(x, y))
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

        public virtual void Generate(double minePrecent, int xOrigin, int yOrigin, int originSize = 3, int? seed = null)
        {
            Reset();

            GenerateOrigin(xOrigin, yOrigin, originSize);

            Random rnd = new Random(seed ?? staticRnd.Next());

            GenerateMines((int)(minePrecent * Width * Height), rnd);

            GenerateVals();

            Reveal(xOrigin, yOrigin);
        }

        private void GenerateOrigin(int xOrigin, int yOrigin, int size)
        {
            if (size <= 0 || fieldSolved[xOrigin, yOrigin] == 0)
            {
                return;
            }

            fieldSolved[xOrigin, yOrigin] = 0;

            foreach ((int x2, int y2) in GetNeighbors(xOrigin, yOrigin))
            {
                GenerateOrigin(x2, y2, size - 1);
            }
        }

        private void GenerateMines(int nMines, Random rnd)
        {
            List<(int, int)> freeCoords = new List<(int, int)>(Width * Height);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (fieldSolved[x, y] == Hidden)
                    {
                        freeCoords.Add((x, y));
                    }
                }
            }

            for (int i = 0; i < nMines; i++)
            {
                if (freeCoords.Count == 0)
                {
                    return;
                }

                int index = rnd.Next(0, freeCoords.Count);

                (int x, int y) = freeCoords[index];

                fieldSolved[x, y] = Mine;

                freeCoords.RemoveAt(index);
            }
        }

        protected void GenerateVals()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (fieldSolved[x, y] == Mine)
                    {
                        continue;
                    }

                    fieldSolved[x, y] = 0;

                    foreach ((int x2, int y2) in GetNeighbors(x, y))
                    {
                        if (fieldSolved[x2, y2] == Mine)
                        {
                            fieldSolved[x, y]++;
                        }
                    }
                }
            }
        }
    }
}
