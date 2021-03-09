using System;
using Minefield;

namespace Minesolver
{
    internal class Program
    {
        private static void Main()
        {
            Field field = new Field(30, 120);

            ConsGraphics G = new ConsGraphics();

            G.Subscribe(field);

            Solver S = new Solver(field)
            {
                MaxDepth = 10
            };

            for (int i = 0; i < 100; i++)
            {
                Console.Title = i.ToString();

                field.Generate(0.2, 5, 5, 1, i);

                S.Solve();

                Console.ReadKey();
            }

            Console.ReadKey();
        }
    }
}
