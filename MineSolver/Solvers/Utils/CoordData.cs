using System.Collections.Generic;

namespace Minesolver.Solvers.Utils
{
    public class CoordData
    {
        public MineFieldBase Field { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool IsInitialized { get; private set; } = false;
        public bool IsRevealed { get => Value != MineFieldBase.Hidden ? true : false; }
        public bool IsMine { get => Value == MineFieldBase.Mine ? true : false; }
        public bool IsValue { get => IsRevealed && !IsMine; }
        public bool IsSolved { get => GetIsSolved(); set => isSolvedCache = value; }
        public int Value { get => Field[X, Y]; }
        public int NumMines { get => GetNumMines(); }
        public int NumHidden { get => GetNumHidden(); }
        public List<(int X, int Y)> Neighbors { get; private set; }

        private bool isSolvedCache;

        public virtual void Initialize(int x, int y, MineFieldBase field)
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
                return true;

            foreach (var coord in Neighbors)
            {
                if (Field[coord] == MineFieldBase.Hidden)
                    return false;
            }

            isSolvedCache = true;

            return true;
        }

        private int GetNumMines()
        {
            int nMines = 0;

            foreach (var coord in Neighbors)
            {
                if (Field[coord] == MineFieldBase.Mine)
                    nMines++;
            }

            return nMines;
        }

        private int GetNumHidden()
        {
            int nHidden = 0;

            foreach (var coord in Neighbors)
            {
                if (Field[coord] == MineFieldBase.Hidden)
                    nHidden++;
            }

            return nHidden;
        }
    }
}
