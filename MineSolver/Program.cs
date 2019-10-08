using System;
using MineSolver.Solvers;

namespace MineSolver
{
    class Program
    {
        static void Main()
        {
            MineField field = new MineField(360, 90);

            field.Generate(0.2, field.Width / 2, field.Height / 2);
            field.Reveal(field.Width / 2, field.Height / 2);

            field.PrintOnlineEnable();

            SolverComplex solverComplex = new SolverComplex(field);
            solverComplex.Solve();

            //field.Print();

            Console.ReadKey();
        }

        static TimeSpan TestSpeed(MineField field, SolverBase solver, int iterations)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

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
