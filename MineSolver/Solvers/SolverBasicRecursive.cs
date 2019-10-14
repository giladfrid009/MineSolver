using System.Collections.Generic;
using MineSolver.Solvers.Utils;

namespace MineSolver.Solvers
{
    public class SolverBasicRecursive : SolverBase<CoordData>
    {
        public SolverBasicRecursive(MineFieldBase field) : base(field)
        {

        }

        public override SolveLog Solve()
        {
            SolveLog log = new SolveLog();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SolveCoord(x, y, log);
                }
            }

            return log;
        }

        private void SolveCoord(int x, int y, SolveLog log)
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
                var hidden = GetHidden(x, y);

                foreach (var (x2, y2) in hidden)
                {
                    field.Reveal(x2, y2);
                    log.AddMove(x2, y2, Move.Reveal);
                }

                foreach (var (x2, y2) in hidden)
                {
                    if (field[x2, y2] == 0)
                    {
                        var opened = GetAreaBounds(x2, y2);

                        affected.AddRange(opened);

                        foreach (var (x3, y3) in opened)
                        {
                            affected.AddRange(GetUnsolved(x3, y3));
                        }
                    }
                    else
                    {
                        affected.Add((x2, y2));
                        affected.AddRange(GetUnsolved(x2, y2));
                    }
                }
            }
            else if (nHidden == field[x, y] - nMines)
            {
                var hidden = GetHidden(x, y);

                foreach (var (x2, y2) in hidden)
                {
                    field.Flag(x2, y2);
                    log.AddMove(x2, y2, Move.Flag);
                }

                foreach (var (x2, y2) in hidden)
                {
                    affected.AddRange(GetUnsolved(x2, y2));
                }
            }

            foreach (var (x2, y2) in affected)
            {
                SolveCoord(x2, y2, log);
            }
        }
    }
}
