using Minesolver.Solvers;
using Minesolver.Solvers.Utils;
using System;

namespace Minesolver
{
    internal class Program
    {
        private static void Main()
        {
            MineField field = new MineField(205, 65);

            SolverAdvancedGuesser solverGuesser = new SolverAdvancedGuesser(field);

            OnlineGraphics.Subscibe(field);

            Console.WriteLine(Benchmarks.CountUnsolved(solverGuesser, 1, 1));

            Console.ReadKey();
        }

    }
}
