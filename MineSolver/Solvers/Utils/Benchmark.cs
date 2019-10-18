using System;
using System.Diagnostics;

namespace Minesolver.Solvers.Utils
{
    public static class Benchmarks
    {
        public static TimeSpan MeasureTime<TCoordData>(SolverBase<TCoordData> solver, int iterations, int seed) where TCoordData : CoordData, new()
        {
            Random rnd = new Random(seed);
            Stopwatch stopwatch = new Stopwatch();
            MineField field = (MineField)solver.Field;

            for (int i = 0; i < iterations; i++)
            {
                field.Generate(0.2, field.Width / 2, field.Height / 2, 3, rnd.Next(0, int.MaxValue));
                field.Reveal(field.Width / 2, field.Height / 2);

                stopwatch.Start();
                solver.Solve();
                stopwatch.Stop();
            }

            return stopwatch.Elapsed;
        }

        public static int CountUnsolved<TCoordData>(SolverBase<TCoordData> solver, int iterations, int seed)
            where TCoordData : CoordData, new()
        {
            Random rnd = new Random(seed);
            MineField field = (MineField)solver.Field;

            int width = field.Width;
            int height = field.Height;

            int totalHidden = 0;

            for (int i = 0; i < iterations; i++)
            {
                field.Generate(0.2, width / 2, height / 2, 3, rnd.Next(0, int.MaxValue));
                field.Reveal(width / 2, height / 2);

                solver.Solve();

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (field[x, y] == MineFieldBase.Hidden)
                        {
                            totalHidden++;
                        }
                    }
                }
            }

            return totalHidden;
        }
    }
}
