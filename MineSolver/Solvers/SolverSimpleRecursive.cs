namespace MineSolver.Solvers
{
    public class SolverSimpleRecursive : SolverBase
    {
        public SolverSimpleRecursive(MineFieldBase field) : base(field)
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
                    SolveLogic(x2, y2, log);

                    foreach ((int x3, int y3) in GetUnsolved(x2, y2))
                    {
                        SolveLogic(x3, y3, log);
                    }
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
                    foreach ((int x3, int y3) in GetUnsolved(x2, y2))
                    {
                        SolveLogic(x3, y3, log);
                    }
                }
            }
        }
    }
}
