using System.Collections.Generic;
using MineSolver.Solvers.Utils;

namespace MineSolver.Solvers
{
    public class Simple : SolverBase<CoordInfo>
    {
        public Simple(MineFieldBase field) : base(field)
        {

        }

        public override SolveLog Solve()
        {
            SolveLog log = new SolveLog();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pending = new HashSet<(int, int)> { (x, y) };

                    while (pending.Count > 0)
                    {
                        var pendingNew = new HashSet<(int, int)>();

                        foreach (var (x2, y2) in pending)
                        {
                            pendingNew.UnionWith(SolveCoord(x2, y2, log));
                        }

                        pending = pendingNew;
                    }
                }
            }

            return log;
        }

        private HashSet<(int X, int Y)> SolveCoord(int x, int y, SolveLog log)
        {
            var affected = new HashSet<(int, int)>();

            if (fieldInfo[x, y].IsSolved || (fieldInfo[x, y].IsValue == false))
            {
                return affected;
            }

            int nHidden = fieldInfo[x, y].NumHidden;
            int nMines = fieldInfo[x, y].NumMines;

            if (field[x, y] == nMines)
            {
                var hidden = fieldInfo[x, y].GetHidden();

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

                        affected.UnionWith(opened);

                        foreach(var (x3, y3) in opened)
                        {
                            affected.UnionWith(GetUnsolved(x3, y3));
                        }
                    }
                    else
                    {
                        affected.Add((x2, y2));
                        affected.UnionWith(GetUnsolved(x2, y2));
                    }
                }
            }
            else if (nHidden == field[x, y] - nMines)
            {
                var hidden = fieldInfo[x, y].GetHidden();

                foreach (var (x2, y2) in hidden)
                {
                    field.Flag(x2, y2);
                    log.AddMove(x2, y2, Move.Flag);
                }

                foreach (var (x2, y2) in hidden)
                {
                    affected.UnionWith(GetUnsolved(x2, y2));
                }
            }

            return affected;
        }
    }
}