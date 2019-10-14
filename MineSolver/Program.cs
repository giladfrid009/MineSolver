using System;
using MineSolver.Solvers;
using MineSolver.Solvers.Utils;

namespace MineSolver
{
    class Program
    {
        static void Main()
        {
            MineField field = new MineField(250, 65);

            OnlineGraphics.Subscibe(field);

            SolverBasic solverSimple = new SolverBasic(field);
            SolverAdvanced solverComplex1 = new SolverAdvanced(field, solverSimple);

            SolverBasicRecursive solverSimpleRecursive = new SolverBasicRecursive(field);
            SolverAdvanced solverComplex2 = new SolverAdvanced(field, solverSimpleRecursive);

            field.Generate(0.2, 3, 3, 2);
            field.Reveal(3, 3);

            solverComplex1.Solve();

            Console.WriteLine(Benchmarks.TestSpeed(field, solverSimple, 1000));
            Console.WriteLine(Benchmarks.TestSpeed(field, solverComplex1, 1000));
            Console.WriteLine(Benchmarks.TestSpeed(field, solverSimpleRecursive, 1000));
            Console.WriteLine(Benchmarks.TestSpeed(field, solverComplex2, 1000));

            Console.ReadKey();
        }

    }
}
