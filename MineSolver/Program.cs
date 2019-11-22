using Minesolver.Solvers;
using Minesolver.Solvers.Utils;
using System;

namespace Minesolver
{
    internal class Program
    {
        private static void Main()
        {
            Field field = new Field(50, 50);

            SolverAdvancedGuesser solverGuesser = new SolverAdvancedGuesser(field);

            OnlineGraphics.Subscibe(field);

            Console.WriteLine(Benchmarks.CountUnsolved(solverGuesser, 100, 0, () => Console.Clear())); // 2070

            //Console.WriteLine(Benchmarks.MeasureTime(solverGuesser, 100, 0));

            Console.ReadKey();
        }

    }
}
