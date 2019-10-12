namespace MineSolver.Solvers
{
    public class SolverSimpleRecursive : SolverBase<CoordInfo>
    {
        public SolverSimpleRecursive(IMIneField field) : base(field)
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

        private void SolveLogic(int x, int y, SolveLog log)
        {
            if (fieldInfo[x, y].IsSolved || fieldInfo[x, y].IsValue == false)
            {
                return;
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
                    SolveLogic(x2, y2, log);

                    foreach (var (x3, y3) in GetUnsolved(x2, y2))
                    {
                        SolveLogic(x3, y3, log);
                    }
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
                    foreach ((int x3, int y3) in GetUnsolved(x2, y2))
                    {
                        SolveLogic(x3, y3, log);
                    }
                }
            }
        }
    }
}
