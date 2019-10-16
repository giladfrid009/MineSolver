using Minesolver.Solvers;

namespace Minesolver
{
    class Program
    {
        static void Main()
        {
            MineField field1 = new MineField(250, 65, 1);
            SolverBasic solverSimple1 = new SolverBasic(field1);
            SolverAdvanced solverAdvanced1 = new SolverAdvanced(field1, solverSimple1);
         
            OnlineGraphics.Subscibe(field1);

            solverAdvanced1.Solve();
           
        }

    }
}
