using System;
using MineSolver.Benchmarks;

namespace MineSolver
{
    class Program
    {
        static void Main()
        {
            MineField field = new MineField(250, 65, 4);

            //OnlineGraphics.Subscibe(field);

            field.Generate(0.23, field.Width / 2, field.Height / 2, 3);
            field.Reveal(field.Width / 2, field.Height / 2);

            Solvers.Simple solverSimple = new Solvers.Simple(field);
            Solvers.Complex solverComplex = new Solvers.Complex(field, solverSimple);

            Solvers.SimpleRecursive solverSimpleRecursive = new Solvers.SimpleRecursive(field);
            Solvers.Complex solverComplex2 = new Solvers.Complex(field, solverSimpleRecursive);

            Console.WriteLine(Benchmark.TestSpeed(field, solverSimple, 1000));
            Console.WriteLine(Benchmark.TestSpeed(field, solverComplex, 1000));
            Console.WriteLine(Benchmark.TestSpeed(field, solverSimpleRecursive, 1000));
            Console.WriteLine(Benchmark.TestSpeed(field, solverComplex2, 1000));

            Console.ReadKey();
        }

    }
}
