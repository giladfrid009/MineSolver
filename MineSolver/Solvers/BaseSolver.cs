using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesolver.Solvers
{
    public abstract class BaseSolver<TField, TCoord>
        where TCoord : Coord
        where TField : Field<TCoord>
    {
        public BaseField Field { get; }
        public bool HasLost { get; protected set; } = false;

        protected readonly TField field;

        protected readonly MoveLog log;
        
        protected int width => field.Width;
        protected int height => field.Height;

        public BaseSolver(BaseField field)
        {
            this.field = (TField)Activator.CreateInstance(typeof(TField), field)!;

            Field = field;
            log = new MoveLog();

            field.OnLoss += (x, y) => HasLost = true;
        }

        public abstract MoveLog Solve();

        protected List<(int X, int Y)> GetHidden(int x, int y)
        {
            return field[x, y].Neighbors.Where(coord => field.IsRevealed(coord.X, coord.Y) == false).ToList();
        }

        protected List<(int X, int Y)> GetValues(int x, int y)
        {
            return field[x, y].Neighbors.Where(coord => field.IsValue(coord.X, coord.Y)).ToList();
        }

        protected List<(int X, int Y)> GetUnsolved(int x, int y)
        {
            return GetValues(x, y).Where(coord => field.IsSolved(coord.X, coord.Y) == false).ToList();
        }

        protected virtual void Reset()
        {
            log.Moves.Clear();

            HasLost = false;
        }
    }
}
