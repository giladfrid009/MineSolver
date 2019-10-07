using System.Linq;
using System.Collections.Generic;

namespace MineSolver
{
    public abstract class SolverBase
    {
        protected readonly MineFieldBase field;
        protected bool[,] solveStatus;
        protected readonly int width;
        protected readonly int height;

        public SolverBase(MineFieldBase field)
        {
            this.field = field;
            width = field.Width;
            height = field.Height;
            solveStatus = new bool[field.Width, field.Height];
        }

        public abstract SolveLog Solve();

        protected (int NumHidden, int NumFlags) GetCoordInfo(int x, int y)
        {
            int nHidden = 0;
            int nFlags = 0;

            foreach ((int xN, int yN) in field.GetNeighbors(x, y))
            {
                if (field[xN, yN] == MineFieldBase.HiddenVal)
                {
                    nHidden++;
                }
                else if (field[xN, yN] == MineFieldBase.BombVal)
                {
                    nFlags++;
                }
            }

            return (nHidden, nFlags);
        }

        protected List<(int X, int Y)> GetUnsolved(int x, int y)
        {
            return field.GetRevealed(x, y).Where(coord => solveStatus[coord.X, coord.Y] == false).ToList();
        }        
    }
}
