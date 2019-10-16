using System;
using System.Diagnostics;

namespace Minesolver.Solvers.Utils
{
    public static class Benchmarks
    {
        public static TimeSpan TestSpeed<TCoordData>(SolverBase<TCoordData> solver, int iterations) where TCoordData : CoordData, new()
        {
            Stopwatch stopwatch = new Stopwatch();

            var field = (MineField)solver.Field;

            for (int i = 0; i < iterations; i++)
            {
                field.Generate(0.2, field.Width / 2, field.Height / 2, 3);
                field.Reveal(field.Width / 2, field.Height / 2);

                stopwatch.Start();
                solver.Solve();
                stopwatch.Stop();
            }

            return stopwatch.Elapsed;
        }

        public static int MeasureEffectiveness<TCoordData>(SolverBase<TCoordData> solver, int iterations)
            where TCoordData : CoordData, new()
        {
            var field = (MineField)solver.Field;

            int width = field.Width;
            int height = field.Height;

            int totalHidden = 0;

            for (int i = 0; i < iterations; i++)
            {
                field.Generate(0.2, width / 2, height / 2, 3);

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
