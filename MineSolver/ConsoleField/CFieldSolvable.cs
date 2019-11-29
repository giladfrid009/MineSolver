using Minesolver.Solvers;
using System;

namespace Minesolver.ConsoleField
{
    internal class CFieldSolvable : CField
    {
        protected readonly SolverAdvanced solver;

        public CFieldSolvable(int width, int height) : base(width, height)
        {
            solver = new SolverAdvanced(this);
        }

        public override void Generate(double minePrecent, int xOrigin, int yOrigin, int originSize = 3, int? seed = null)
        {
            int nHidden;

            do
            {
                nHidden = 0;

                base.Generate(minePrecent, xOrigin, yOrigin, originSize, seed);

                solver.Solve();

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (fieldUnsolved[x, y] == Hidden)
                        {
                            nHidden++;
                        }
                    }
                }
            }
            while (nHidden > Width * Height * 0.2);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (fieldUnsolved[x, y] == Hidden)
                    {
                        fieldSolved[x, y] = 0;
                    }
                }
            }

            GenerateVals();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    fieldUnsolved[x, y] = Hidden;
                }
            }

            Reveal(xOrigin, yOrigin);
        }

        public int[,] GetSolvedState()
        {
            int[,] clone = new int[Width, Height];

            Array.Copy(fieldSolved, 0, clone, 0, Width * Height);

            return clone;
        }
    }
}
