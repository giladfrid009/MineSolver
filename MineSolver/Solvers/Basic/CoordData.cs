using System.Collections.Generic;

namespace Minesolver.Solvers.Basic
{
    public class CoordData
    {
        public FieldBase Field { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool IsInitialized { get; private set; } = false;
        public bool IsRevealed => Value != FieldBase.Hidden ? true : false;
        public bool IsMine => Value == FieldBase.Mine ? true : false;
        public bool IsValue => IsRevealed && !IsMine;
        public bool IsSolved { get => GetIsSolved(); set => isSolvedCache = value; }
        public int Value => Field[X, Y];
        public int NumMines => GetNumMines();
        public int NumHidden => GetNumHidden();
        public List<(int X, int Y)> Neighbors { get; private set; }

        private bool isSolvedCache;

        public virtual void Initialize(int x, int y, FieldBase field)
        {
            Field = field;
            X = x;
            Y = y;
            Neighbors = field.GetNeighbors(x, y);

            IsInitialized = true;
        }

        private bool GetIsSolved()
        {
            if (isSolvedCache == true)
            {
                return true;
            }

            foreach ((int X, int Y) coord in Neighbors)
            {
                if (Field[coord] == FieldBase.Hidden)
                {
                    return false;
                }
            }

            isSolvedCache = true;

            return true;
        }

        private int GetNumMines()
        {
            int nMines = 0;

            foreach ((int X, int Y) coord in Neighbors)
            {
                if (Field[coord] == FieldBase.Mine)
                {
                    nMines++;
                }
            }

            return nMines;
        }

        private int GetNumHidden()
        {
            int nHidden = 0;

            foreach ((int X, int Y) coord in Neighbors)
            {
                if (Field[coord] == FieldBase.Hidden)
                {
                    nHidden++;
                }
            }

            return nHidden;
        }
    }
}
