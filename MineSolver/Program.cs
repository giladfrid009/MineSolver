using System;
using MineSolver.Solvers;

namespace MineSolver
{
    class Program
    {
        static void Main()
        {
            MineField field = new MineField(250, 65, 4);

            OnlineGraphics.Subscibe(field);

            field.Generate(0.23, field.Width / 2, field.Height / 2, 3);
            field.Reveal(field.Width / 2, field.Height / 2);

            Simple solverSimple = new Simple(field);
            Complex solverComplex = new Complex(field, solverSimple);

            SimpleRecursive solverSimpleRecursive = new SimpleRecursive(field);
            Complex solverComplex2 = new Complex(field, solverSimpleRecursive);

            solverSimpleRecursive.Solve();

            Console.ReadKey();

            solverSimple.Solve();

            Console.ReadKey();

            Console.WriteLine(Benchmark.TestSpeed(field, solverSimple, 1000));
            Console.WriteLine(Benchmark.TestSpeed(field, solverComplex, 1000));
            Console.WriteLine(Benchmark.TestSpeed(field, solverSimpleRecursive, 1000));
            Console.WriteLine(Benchmark.TestSpeed(field, solverComplex2, 1000));

            Console.ReadKey();
        }

    }
}
