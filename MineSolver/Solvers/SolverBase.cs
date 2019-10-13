using System.Linq;
using System.Collections.Generic;

namespace MineSolver.Solvers
{
    public abstract class SolverBase<TCoordInfo> where TCoordInfo : CoordInfo, new()
    {
        protected readonly FieldInfo<TCoordInfo> fieldInfo;
        protected readonly MineFieldBase field;
        protected readonly int width;
        protected readonly int height;

        public SolverBase(MineFieldBase field)
        {
            this.field = field;
            fieldInfo = new FieldInfo<TCoordInfo>(field);
            width = field.Width;
            height = field.Height;
        }

        public abstract SolveLog Solve();

        protected List<(int X, int Y)> GetUnsolved(int x, int y)
        {
            return fieldInfo[x, y].GetValues().Where(coord => fieldInfo[coord].IsSolved == false).ToList();
        }             

        protected void GetOpenedAreaRecursive(int x, int y, HashSet<(int, int)> coords)
        {
            if (coords.Contains((x, y)))
                return;

            coords.Add((x, y));

            if (field[x, y] == 0)
            {
                foreach(var (x2, y2) in fieldInfo[x,y].GetNeighbors())
                {
                    GetOpenedAreaRecursive(x2, y2, coords);
                }
            }
        }
    }
}
