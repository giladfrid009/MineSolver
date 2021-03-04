using Minefield;
using System;

namespace Minesolver
{
    internal class Program
    {
        private static void Main()
        {
            Field field = new Field(10, 10);

            ConsGraphics G = new ConsGraphics();
            
            G.Subscribe(field);

            field.Generate(0.15, 5, 5, 1, 0);

            Solver S = new Solver(field);

            S.Solve();

            Console.ReadKey();

            S.Solve();

            Console.ReadKey();
        }
    }
}
