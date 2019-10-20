using System.Collections.Generic;
using Minesolver.Solvers.Basic;

namespace Minesolver.Solvers
{
    public class SolverBasic : SolverBase<CoordData>
    {
        public SolverBasic(MineFieldBase field) : base(field)
        {

        }

        public override SolveLog Solve()
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
            if (fieldData[x, y].IsSolved || (fieldData[x, y].IsValue == false) || HasLost)
            {
                return new HashSet<(int, int)>();
            }

            int nHidden = fieldData[x, y].NumHidden;
            int nMines = fieldData[x, y].NumMines;

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