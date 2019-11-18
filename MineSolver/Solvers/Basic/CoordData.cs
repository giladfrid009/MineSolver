using System.Collections.Generic;

namespace Minesolver.Solvers.Basic
{
    public class CoordData
    {
        private FieldBase field;
        private int x;
        private int y;

        public bool IsInitialized { get; private set; } = false;
        public virtual bool IsRevealed => field[x, y] != FieldBase.Hidden ? true : false;
        public virtual bool IsFlagged => field[x, y] == FieldBase.Mine ? true : false;
        public virtual bool IsValue => IsRevealed && !IsFlagged;
        public List<(int X, int Y)> Neighbors { get; private set; }

        public virtual void Initialize(int x, int y, FieldBase field)
        {
            this.field = field;
            this.x = x;
            this.y = y;
            Neighbors = field.GetNeighbors(x, y);

            IsInitialized = true;
        }
    }
}
