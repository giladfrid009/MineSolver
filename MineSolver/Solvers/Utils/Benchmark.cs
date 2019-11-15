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

        private static void PrintLog(int iteration, int seed)
        {
            Console.WriteLine(string.Format("Iter: {0} | Seed: {1}", iteration, seed));
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
                field.Generate(0.2, xMid, yMid, 3, rnd.Next());

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
                field.Generate(0.2, xMid, yMid, 3, rnd.Next());

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

                // todo: remove later
                Console.Write('.');
                if (i == 10)
                    OnlineGraphics.Subscibe(field);
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
                int localSeed = rnd.Next();

                PrintLog(i, localSeed);

                field.Generate(0.2, xMid, yMid, 3, localSeed);

                solver.Solve();

                yield return solver;
            }
        }
    }
}
