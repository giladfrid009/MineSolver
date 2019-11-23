using Minesolver.ConsoleField;
using Minesolver.ConsoleField.Benchmark;
using Minesolver.Solvers;
using System;

namespace Minesolver
{
    internal class Program
    {
        private static void Main()
        {
            CField field = new CField(50, 50);

            SolverGuesser solverGuesser = new SolverGuesser(field);

            //OnlineGraphics.Subscibe(field);

            Console.WriteLine(Benchmarks.CountUnsolved(solverGuesser, 100, 0)); // 2070

            //Console.WriteLine(Benchmarks.MeasureTime(solverGuesser, 100, 0));

            Console.ReadKey();
        }

    }
}
