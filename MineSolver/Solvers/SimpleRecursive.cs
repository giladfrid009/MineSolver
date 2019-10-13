using System.Collections.Generic;
using MineSolver.Solvers.Utils;

namespace MineSolver.Solvers
{
    public class SimpleRecursive : SolverBase<CoordInfo>
    {
        public SimpleRecursive(MineFieldBase field) : base(field)
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
            if (fieldInfo[x, y].IsSolved || (fieldInfo[x, y].IsValue == false))
            {
                return;
            }

            var affected = new HashSet<(int, int)>();

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
                        GetOpenedAreaRecursive(x2, y2, affected);
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

            foreach(var (x2, y2) in affected)
            {
                SolveCoord(x2, y2, log);
            }
        }
    }
}
