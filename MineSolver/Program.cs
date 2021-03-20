using System;
using Minefield;

namespace Minesolver
{
    internal class Program
    {
        private static void Main()
        {
            Field field = new Field(10, 10);

            new ConsGraphics(field);

            Solver S = new Solver(field);

            int nGames = 10000000;
            int nWins = 0;

            for (int i = 0; i < nGames; i++)
            {
                new ConsGraphics(field);

                field.Generate(0.15625, field.Height / 2, field.Width / 2, 2, i);

                S.Solve();

                //Console.ReadKey();
            }

            Console.WriteLine((double)nWins / nGames * 100);

            Console.WriteLine("Done");
        }
    }
}
