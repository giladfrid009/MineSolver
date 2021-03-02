using Minesolver.Solvers.Basic;
using System.Collections.Generic;

namespace Minesolver.Solvers
{
    public class SolverBasic : BaseSolverBasic
    {
        public SolverBasic(BaseField field) : base(field)
        {

        }

        public override MoveLog Solve()
        {
            Reset();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    HashSet<(int, int)> pending = new HashSet<(int, int)> { (x, y) };

                    while (pending.Count > 0)
                    {
                        HashSet<(int, int)> pendingNew = new HashSet<(int, int)>();

                        foreach ((int x2, int y2) in pending)
                        {
                            pendingNew.UnionWith(SolveCoord(x2, y2));                      
                        }

                        pending = pendingNew;
                    }
                }
            }

            return log.Clone();
        }

        private HashSet<(int X, int Y)> SolveCoord(int x, int y)
        {
            if (fieldData.IsSolved(x, y) || (fieldData.IsValue(x, y) == false) || HasLost)
            {
                return new HashSet<(int, int)>();
            }

            int nHidden = fieldData.NumHidden(x, y);
            int nMines = fieldData.NumMines(x, y);

            if (Field[x, y] == nMines)
            {
                return RevealHidden(x, y);
            }
            else if (nHidden == Field[x, y] - nMines)
            {
                return FlagHidden(x, y);
            }

            return new HashSet<(int, int)>();
        }
    }
}