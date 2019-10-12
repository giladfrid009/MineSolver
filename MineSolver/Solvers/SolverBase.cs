using System.Linq;
using System.Collections.Generic;

namespace MineSolver
{
    public abstract class SolverBase<TCoordInfo> where TCoordInfo : CoordInfo, new()
    {
        protected readonly FieldInfo<TCoordInfo> fieldInfo;
        protected readonly IMIneField field;
        protected readonly int width;
        protected readonly int height;

        public SolverBase(IMIneField field)
        {
            this.field = field;
            fieldInfo = new FieldInfo<TCoordInfo>(field);
            width = field.Width;
            height = field.Height;
        }

        public abstract SolveLog Solve();

        protected List<(int X, int Y)> GetUnsolved(int x, int y)
        {
            return fieldInfo[x, y].ValueCoords.Where(coord => fieldInfo[coord].IsSolved == false).ToList();
        }        
    }
}
