using System.Collections.Generic;

namespace MineSolver.Solvers
{
    public class SolverSimple : SolverBase
    {
        public SolverSimple(MineFieldBase field) : base(field)
        {

        }

        public override SolveLog Solve()
        {
            SolveLog log = new SolveLog();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    HashSet<(int X, int Y)> pendingSolve = new HashSet<(int X, int Y)> { (x, y) };

                    while (pendingSolve.Count > 0)
                    {
                        HashSet<(int, int)> pendingSolveNew = new HashSet<(int, int)>();

                        foreach (var coord in pendingSolve)
                        {
                            pendingSolveNew.UnionWith(SolveLogic(coord.X, coord.Y, log));
                        }

                        pendingSolve = pendingSolveNew;
                    }
                }
            }

            return log;
        }

        private HashSet<(int X, int Y)> SolveLogic(int x, int y, SolveLog log)
        {
            HashSet<(int, int)> affectedCoords = new HashSet<(int, int)>();

            if (solveStatus[x, y] || field[x, y] < 0)
            {
                return affectedCoords;
            }

            (int nHidden, int nFlags) = GetCoordInfo(x, y);

            if (field[x, y] == nFlags)
            {
                solveStatus[x, y] = true;

                foreach ((int x2, int y2) in field.GetHidden(x, y))
                {
                    field.Reveal(x2, y2);
                    log.AddMove(x2, y2, Move.Reveal);
                }

                foreach ((int x2, int y2) in GetUnsolved(x, y))
                {
                    affectedCoords.Add((x2, y2));
                    affectedCoords.UnionWith(GetUnsolved(x2, y2));
                }
            }
            else if (nHidden == field[x, y] - nFlags)
            {
                solveStatus[x, y] = true;

                var hidden = field.GetHidden(x, y);

                foreach ((int x2, int y2) in hidden)
                {
                    field.Flag(x2, y2);
                    log.AddMove(x2, y2, Move.Flag);
                }

                foreach ((int x2, int y2) in hidden)
                {
                    affectedCoords.UnionWith(GetUnsolved(x2, y2));
                }
            }

            return affectedCoords;
        }
    }
}
