using System.Collections.Generic;
using System.Linq;
using Minesolver.Solvers.Basic;

namespace Minesolver.Solvers
{
    public abstract class SolverBase<TCoordData> where TCoordData : CoordData, new()
    {
        public FieldBase Field { get; }
        public bool HasLost { get; protected set; } = false;

        protected readonly FieldData<TCoordData> fieldData;
        protected readonly SolveLog log;
        protected readonly int width;
        protected readonly int height;

        public SolverBase(FieldBase field)
        {
            fieldData = new FieldData<TCoordData>(field);
            Field = field;
            log = new SolveLog();
            width = field.Width;
            height = field.Height;

            field.OnLoss += (x, y) => HasLost = true;
        }

        public abstract SolveLog Solve();

        protected List<(int X, int Y)> GetHidden(int x, int y)
        {
            return fieldData[x, y].Neighbors.Where(coord => fieldData[coord].IsRevealed == false).ToList();
        }

        protected List<(int X, int Y)> GetValues(int x, int y)
        {
            return fieldData[x, y].Neighbors.Where(coord => fieldData[coord].IsValue).ToList();
        }

        protected List<(int X, int Y)> GetUnsolved(int x, int y)
        {
            return GetValues(x, y).Where(coord => fieldData[coord].IsSolved == false).ToList();
        }

        protected abstract void Reset();
    }
}
