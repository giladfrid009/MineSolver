using System;
using System.Diagnostics;
using MineSolver.Solvers.Utils;

namespace MineSolver.Benchmarks
{
    public static class Benchmark
    {
        public static TimeSpan TestSpeed<TCoordInfo>(MineField field, Solvers.SolverBase<TCoordInfo> solver, int iterations) where TCoordInfo : CoordInfo, new()
        {
           Stopwatch sw = new Stopwatch();

            for (int i = 0; i < iterations; i++)
            {
                field.Generate(0.2, 3, 3, 2);
                field.Reveal(3, 3);

                sw.Start();

                solver.Solve();

                sw.Stop();
            }

            return sw.Elapsed;
        }

    }
}
