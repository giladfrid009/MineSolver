﻿using System;
using System.Diagnostics;
using Minesolver.Solvers;

namespace Minesolver.ConsoleField.Benchmark
{
    public static class Benchmarks
    {
        private static void CheckCompatibility<TFieldData, TCoordData>(SolverBase<TFieldData, TCoordData> solver)
            where TCoordData : CoordData, new()
            where TFieldData : FieldData<TCoordData>, new()
        {
            if (solver.Field is CField == false)
            {
                throw new Exception("Field class isn't compatible with this function");
            }
        }

        public static TimeSpan MeasureTime<TFieldData, TCoordData>(SolverBase<TFieldData, TCoordData> solver, int iterations, int? seed = null)
            where TCoordData : CoordData, new()
            where TFieldData : FieldData<TCoordData>, new()
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

        public static int CountUnsolved<TFieldData, TCoordData>(SolverBase<TFieldData, TCoordData> solver, int iterations, int? seed = null, Action? onIter = null)
            where TCoordData : CoordData, new()
            where TFieldData : FieldData<TCoordData>, new()
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
                        if (field[x, y] == FieldBase.Hidden)
                        {
                            totalHidden++;
                        }
                    }
                }

                onIter?.Invoke();
            }

            return totalHidden;
        }
    }
}