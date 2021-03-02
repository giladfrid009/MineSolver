using Minesolver.Solvers;
using System;
using System.Diagnostics;

namespace Minesolver.ConsoleField.Benchmark
{
    public static class Benchmarks
    {
        private static void CheckCompatibility<TFieldData, TCoordData>(BaseSolver<TFieldData, TCoordData> solver)
            where TCoordData : Coord
            where TFieldData : Field<TCoordData>
        {
            if (solver.Field is CField == false)
            {
                throw new Exception("Field class isn't compatible with this function");
            }
        }

        public static TimeSpan MeasureTime<TFieldData, TCoordData>(BaseSolver<TFieldData, TCoordData> solver, int iterations, int? seed = null)
            where TCoordData : Coord
            where TFieldData : Field<TCoordData>
        {
            CheckCompatibility(solver);

            Random rnd = seed != null ? new Random(seed.Value) : new Random();
            Stopwatch stopwatch = new Stopwatch();
            CField field = (CField)solver.Field;

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

        public static int CountUnsolved<TFieldData, TCoordData>(BaseSolver<TFieldData, TCoordData> solver, int iterations, int? seed = null)
            where TCoordData : Coord
            where TFieldData : Field<TCoordData>
        {
            CheckCompatibility(solver);

            Random rnd = seed != null ? new Random(seed.Value) : new Random();
            CField field = (CField)solver.Field;

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
                        if (field[x, y] == BaseField.Hidden)
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
