using System.Collections.Generic;

namespace MineSolver.Solvers
{
    public class SolverSimple : SolverBase<CoordInfo>
    {
        public SolverSimple(IMIneField field) : base(field)
        {

        }

        public override SolveLog Solve()
        {
            SolveLog log = new SolveLog();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    HashSet<(int X, int Y)> pending = new HashSet<(int X, int Y)> { (x, y) };

                    while (pending.Count > 0)
                    {
                        HashSet<(int, int)> pendingNew = new HashSet<(int, int)>();

                        foreach ((int x2, int y2) in pending)
                        {
                            pendingNew.UnionWith(SolveLogic(x2, y2, log));
                        }

                        pending = pendingNew;
                    }
                }
            }

            return log;
        }

        protected HashSet<(int X, int Y)> SolveLogic(int x, int y, SolveLog log)
        {
            HashSet<(int, int)> affected = new HashSet<(int, int)>();

            if (fieldInfo[x, y].IsSolved || fieldInfo[x, y].IsValue == false)
            {
                return affected;
            }            

            int nHidden = fieldInfo[x, y].NumHidden;
            int nFlags = fieldInfo[x, y].NumMines;

            if (field[x, y] == nFlags)
            {
                foreach (var (x2, y2) in fieldInfo[x, y].HiddenCoords)
                {
                    field.Reveal(x2, y2);
                    log.AddMove(x2, y2, Move.Reveal);
                }

                foreach (var (x2, y2) in GetUnsolved(x, y))
                {
                    affected.Add((x2, y2));
                    affected.UnionWith(GetUnsolved(x2, y2));
                }
            }
            else if (nHidden == field[x, y] - nFlags)
            {
                var hidden = fieldInfo[x, y].HiddenCoords;

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
