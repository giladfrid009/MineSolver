using System;
using Minefield;

namespace Minesolver
{
    internal class Program
    {
        private static void Main()
        {
            Field field = new Field(16, 16);

            ConsGraphics G = new ConsGraphics();

            G.Subscribe(field);

            Solver S = new Solver(field);

            int nGames = 100000;
            int nWins = 0;

            for (int i = 56280; i < nGames; i++)
            {
                field.Generate(0.15625, 5, 5, 2, i);

                S.Solve();

                Console.ReadKey();
            }

            Console.WriteLine((double)nWins / nGames * 100);

            Console.WriteLine("Done");
        }
    }
}
