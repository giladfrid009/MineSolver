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

            double testValTotal = 0;

            int iterations = 1000;

            // todo: precentage isn't right
            // maybe also cuz of tryAdvnced = false

            // todo: fix this bug.
            //Benchmarks.CountUnsolved(solverGuesser, 1, 157);

            int totalUnsolved = 0;

            for (int i = 0; i < iterations; i++)
            {
                Console.WriteLine(i);
                totalUnsolved += Benchmarks.CountUnsolved(solverAdvanced, 1, i);
                Console.WriteLine(totalUnsolved);
                //testValTotal += solverGuesser.testVal;
            }

            Console.WriteLine(testValTotal / iterations);

            Console.ReadKey();
        }

    }
}
