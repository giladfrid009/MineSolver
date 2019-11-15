using System.Collections.Generic;
using Minesolver.Solvers.Basic;

namespace Minesolver.Solvers
{
    public class SolverBasicRecursive : SolverBasicBase
    {
        public SolverBasicRecursive(FieldBase field) : base(field)
        {

        }

        public override SolveLog Solve()
        {
            Reset();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SolveCoord(x, y);
                }
            }

            return log.Clone();
        }

        private void SolveCoord(int x, int y)
        {
            if (fieldData[x, y].IsSolved || (fieldData[x, y].IsValue == false))
            {
                return;
            }

            List<(int, int)> affected = new List<(int, int)>();

            int nHidden = fieldData[x, y].NumHidden;
            int nMines = fieldData[x, y].NumMines;

            if (Field[x, y] == nMines)
            {
                affected.AddRange(RevealHidden(x, y));
            }
            else if (nHidden == Field[x, y] - nMines)
            {
                affected.AddRange(FlagHidden(x, y));
            }

            foreach ((int x2, int y2) in affected)
            {
                SolveCoord(x2, y2);
            }
        }
    }
}
