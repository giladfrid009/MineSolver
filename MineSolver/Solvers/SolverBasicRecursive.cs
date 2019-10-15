using System.Collections.Generic;
using Minesolver.Solvers.Utils;

namespace Minesolver.Solvers
{
    public class SolverBasicRecursive : SolverBase<CoordData>
    {
        public SolverBasicRecursive(MineFieldBase field) : base(field)
        {

        }

        public override SolveLog Solve()
        {
            log.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SolveCoord(x, y);
                }
            }

            return log;
        }

        private void SolveCoord(int x, int y)
        {
            if (fieldData[x, y].IsSolved || (fieldData[x, y].IsValue == false))
            {
                return;
            }

            var affected = new List<(int, int)>();

            int nHidden = fieldData[x, y].NumHidden;
            int nMines = fieldData[x, y].NumMines;

            if (field[x, y] == nMines)
            {
                affected.AddRange(RevealHidden(x, y));
            }
            else if (nHidden == field[x, y] - nMines)
            {
                affected.AddRange(FlagHidden(x, y));
            }

            foreach (var (x2, y2) in affected)
            {
                SolveCoord(x2, y2);
            }
        }
    }
}
