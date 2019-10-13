using System;
using MineSolver.Solvers;

namespace MineSolver
{
    class Program
    {
        static void Main()
        {
            MineField field = new MineField(250, 65, 6);

            field.Generate(0.23, field.Width / 2, field.Height / 2, 2);
            field.Reveal(field.Width / 2, field.Height / 2);

            field.EnablePrintOnline();

            SolverComplex solverComplex = new SolverComplex(field);
            solverComplex.Solve();

            //SolverSimple solverSimple = new SolverSimple(field);

            //solverRecursive.Solve(); 

            //Console.ReadKey();

            //solverRecursive.Solve();

            //solverSimple.Solve();

            //Console.ReadKey();

            //Console.WriteLine(TestSpeed(field, solverSimple, 100));

            Console.ReadKey();

            // TODO: SOLVES MORE ON THE SECOND TIME. FIGURE OUT WHAT'S GOING ON!!!

            while (true)
            {
                SolverComplex solverComplex2 = new SolverComplex(field);
                solverComplex2.Solve();
            }
            //field.Print();

            Console.ReadKey();
        }

        static TimeSpan TestSpeed<TCoordInfo>(MineField field, SolverBase<TCoordInfo> solver, int iterations) where TCoordInfo : CoordInfo, new()
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
