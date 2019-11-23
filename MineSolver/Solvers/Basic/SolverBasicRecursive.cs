using Minesolver.Solvers.Basic;
using System.Collections.Generic;

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

                    if (HasLost)
                    {
                        return log.Clone();
                    }
                }
            }

            return log.Clone();
        }

        private void SolveCoord(int x, int y)
        {
            if (fieldData.IsSolved(x, y) || (fieldData.IsValue(x, y) == false) || HasLost)
            {
                return;
            }

            List<(int, int)> affected = new List<(int, int)>();

            int nHidden = fieldData.NumHidden(x, y);
            int nMines = fieldData.NumMines(x, y);

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
