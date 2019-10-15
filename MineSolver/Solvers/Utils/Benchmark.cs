using System;
using System.Diagnostics;

namespace Minesolver.Solvers.Utils
{
    public static class Benchmarks
    {
        public static TimeSpan TestSpeed<TCoordInfo>(MineField field, SolverBase<TCoordInfo> solver, int iterations) where TCoordInfo : CoordData, new()
        {
           Stopwatch stopwatch = new Stopwatch();

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
    }
}
