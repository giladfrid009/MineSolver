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

            SolverBasic solverBase = new SolverBasic(field);

            SolverAdvanced solverAdvanced = new SolverAdvanced(field);

            SolverAdvancedGuesser solverGuesser = new SolverAdvancedGuesser(field);

            //OnlineGraphics.Subscibe(field);
            Console.WriteLine(Benchmarks.CountUnsolved(solverBase, 5, 1));
            Console.WriteLine(Benchmarks.CountUnsolved(solverAdvanced, 5, 1));
            Console.WriteLine(Benchmarks.CountUnsolved(solverGuesser, 5, 1));

            Console.ReadKey();
        }

    }
}
