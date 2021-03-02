using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesolver.Solvers
{
    public abstract class BaseSolver<TFieldData, TCoordData>
        where TCoordData : Coord
        where TFieldData : Field<TCoordData>
    {
        public BaseField Field { get; }
        public bool HasLost { get; protected set; } = false;

        protected readonly TFieldData fieldData;
        protected readonly MoveLog log;
        protected readonly int width;
        protected readonly int height;

        public BaseSolver(BaseField field)
        {
            fieldData = (TFieldData)Activator.CreateInstance(typeof(TFieldData), field)!;

            Field = field;
            log = new MoveLog();
            width = field.Width;
            height = field.Height;

            field.OnLoss += (x, y) => HasLost = true;
        }

        public abstract MoveLog Solve();

        protected List<(int X, int Y)> GetHidden(int x, int y)
        {
            return fieldData[x, y].Neighbors.Where(coord => fieldData.IsRevealed(coord.X, coord.Y) == false).ToList();
        }

        protected List<(int X, int Y)> GetValues(int x, int y)
        {
            return fieldData[x, y].Neighbors.Where(coord => fieldData.IsValue(coord.X, coord.Y)).ToList();
        }

        protected List<(int X, int Y)> GetUnsolved(int x, int y)
        {
            return GetValues(x, y).Where(coord => fieldData.IsSolved(coord.X, coord.Y) == false).ToList();
        }

        protected virtual void Reset()
        {
            log.Moves.Clear();

            HasLost = false;
        }
    }
}
