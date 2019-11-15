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

            SolverBasic solverBase = new SolverBasic(field);

            SolverAdvanced solverAdvanced = new SolverAdvanced(field);

            SolverAdvancedGuesser solverGuesser = new SolverAdvancedGuesser(field);

            //OnlineGraphics.Subscibe(field);

            //field.Generate(0.2, field.Width / 2, field.Height / 2, 3, 1389454863);

            //solverAdvanced.Solve();

            //double testValTotal = 0;

            //int iterations = 1000;

            //// todo: precentage isn't right
            //// maybe also cuz of tryAdvnced = false

            //int totalUnsolved = 0;

            //Console.WriteLine(Benchmarks.CountUnsolved(solverAdvanced, 500, 0));

            Console.WriteLine(Benchmarks.CountUnsolved(solverGuesser, 100, 0));

            //for (int i = 0; i < iterations; i++)
            //{
            //    Console.WriteLine(i);
            //    totalUnsolved += Benchmarks.CountUnsolved(solverAdvanced, 1, i);
            //    Console.WriteLine(totalUnsolved);
            //    //testValTotal += solverGuesser.testVal;
            //}

            //Console.WriteLine(testValTotal / iterations);

            Console.ReadKey();
        }

    }
}
