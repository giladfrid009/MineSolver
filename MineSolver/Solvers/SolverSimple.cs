using System.Collections.Generic;

namespace MineSolver.Solvers
{
    public class SolverSimple : SolverBase<CoordInfo>
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
                    SolveLogic(x, y, log);
                }
            }

            return log;
        }

        protected void SolveLogic(int x, int y, SolveLog log)
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
                SolveLogic(x2, y2, log);
            }
        }
    }
}
