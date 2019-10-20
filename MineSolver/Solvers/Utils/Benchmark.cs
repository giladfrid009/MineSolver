using System;
using System.Diagnostics;
using System.Collections.Generic;
using Minesolver.Solvers.Basic;

namespace Minesolver.Solvers.Utils
{
    public static class Benchmarks
    {

        private static void CheckCompatibility<TCoordData>(SolverBase<TCoordData> solver) where TCoordData : CoordData, new()
        {
            if (solver.Field is MineField == false)
            {
                throw new Exception("Field class isn't compatible with this function");
            }
        }

        public static TimeSpan MeasureTime<TCoordData>(SolverBase<TCoordData> solver, int iterations, int? seed = null) where TCoordData : CoordData, new()
        {
            CheckCompatibility(solver);

            Random rnd = seed != null ? new Random(seed.Value) : new Random();
            Stopwatch stopwatch = new Stopwatch();
            MineField field = (MineField)solver.Field;

            int xMid = field.Width / 2;
            int yMid = field.Height / 2;

            for (int i = 0; i < iterations; i++)
            {
                field.Generate(0.2, xMid, yMid, 3, rnd.Next(int.MinValue, int.MaxValue));
                field.Reveal(xMid, yMid);

                stopwatch.Start();
                solver.Solve();
                stopwatch.Stop();
            }

            return stopwatch.Elapsed;
        }

        public static int CountUnsolved<TCoordData>(SolverBase<TCoordData> solver, int iterations, int? seed = null) where TCoordData : CoordData, new()
        {
            CheckCompatibility(solver);

            Random rnd = seed != null ? new Random(seed.Value) : new Random();
            MineField field = (MineField)solver.Field;

            int width = field.Width;
            int height = field.Height;

            int xMid = field.Width / 2;
            int yMid = field.Height / 2;

            int totalHidden = 0;

            for (int i = 0; i < iterations; i++)
            {
                field.Generate(0.2, xMid, yMid, 3, rnd.Next(0, int.MaxValue));
                field.Reveal(xMid, yMid);

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

        public static IEnumerable<SolverBase<TCoordData>> RunSolver<TCoordData>(SolverBase<TCoordData> solver, int iterations, int? seed = null) where TCoordData : CoordData, new()
        {
            CheckCompatibility(solver);

            Random rnd = seed != null ? new Random(seed.Value) : new Random();
            MineField field = (MineField)solver.Field;

            int xMid = field.Width / 2;
            int yMid = field.Height / 2;

            for (int i = 0; i < iterations; i++)
            {
                field.Generate(0.2, xMid, yMid, 3, rnd.Next(0, int.MaxValue));
                field.Reveal(xMid, yMid);

                solver.Solve();

                yield return solver;
            }
        }
    }
}
