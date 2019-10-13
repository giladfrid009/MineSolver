using System;
using MineSolver.Solvers;

namespace MineSolver
{
    class Program
    {
        static void Main()
        {
            MineField field = new MineField(250, 65, 1);

            OnlineGraphics.Subscibe(field);

            field.Generate(0.23, field.Width / 2, field.Height / 2, 2);
            field.Reveal(field.Width / 2, field.Height / 2);

            Simple solverSimple = new Simple(field);
            SimpleRecursive solverSimpleRecursive = new SimpleRecursive(field);
            Complex solverComplex = new Complex(field);

            solverSimple.Solve();

            Console.ReadKey();

            field.Print();

            Console.ReadKey();

            Console.WriteLine(Benchmark.TestSpeed(field, solverSimple, 10));
            Console.WriteLine(Benchmark.TestSpeed(field, solverSimpleRecursive, 10));
            Console.WriteLine(Benchmark.TestSpeed(field, solverComplex, 10));

            Console.ReadKey();
        }

    }
}
