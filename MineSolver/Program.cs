using System;
using Minesolver.Solvers;
using Minesolver.Solvers.Utils;

namespace Minesolver
{
    class Program
    {
        static void Main()
        {
            // TODO: MAKE SOLVERS STATIC!!!

            MineField field = new MineField(205, 65);

            SolverAdvanced solver1 = new SolverAdvanced(field);

            OnlineGraphics.Subscibe(field);

            //field2.Generate(0.2, 5, 5);
            //field2.Reveal(5, 5);

            //solver2.Solve();

            //Console.WriteLine(Benchmarks.CountUnsolved(solver1, 10));
            //Console.WriteLine(Benchmarks.CountUnsolved(solver2, 10));
            Console.WriteLine(Benchmarks.MeasureTime(solver1, 20, 5));


            //OnlineGraphics.Subscibe(field1);

            //field1.Generate(0.2, 5, 5);
            //field1.Reveal(5, 5);



            //solverPrecent.Solve();

            Console.ReadKey();          
        }

    }
}
