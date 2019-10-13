using System.Linq;
using System.Collections.Generic;

namespace MineSolver.Solvers.Utils
{
    public class CoordInfo
    {
        public MineFieldBase Field { get; set; }

        public bool IsRevealed { get => Value != MineFieldBase.Hidden ? true : false; }
        public bool IsMine { get => Value == MineFieldBase.Mine ? true : false; }
        public bool IsValue { get => IsRevealed && !IsMine; }
        public bool IsSolved { get => IsSolvedFunc(); }
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get => Field[X, Y]; }
        public int NumMines { get => NumMinesFunc(); }
        public int NumHidden { get => NumHiddenFunc(); }

        private bool isSolvedCache;
        private List<(int, int)> neighborsCache;
  
        public List<(int, int)> GetNeighbors()
        {
            if (neighborsCache != null)
                return neighborsCache;

            return neighborsCache = Field.GetNeighbors(X, Y);
        }

        public List<(int X, int Y)> GetHidden()
        {
            return GetNeighbors().Where(coord => Field[coord] == MineFieldBase.Hidden).ToList();
        }

        public List<(int X, int Y)> GetMines()
        {
            return GetNeighbors().Where(coord => Field[coord] == MineFieldBase.Mine).ToList();
        }

        public List<(int X, int Y)> GetValues()
        {
            return GetNeighbors().Where(coord => (Field[coord] != MineFieldBase.Hidden) && (Field[coord] != MineFieldBase.Mine)).ToList();
        }

        private bool IsSolvedFunc()
        {
            if (isSolvedCache == true)
                return true;

            foreach (var coord in GetNeighbors())
            {
                if (Field[coord] == MineFieldBase.Hidden)
                    return false;
            }

            isSolvedCache = true;

            return true;
        }

        private int NumMinesFunc()
        {
            int nMines = 0;

            foreach (var coord in GetNeighbors())
            {
                if (Field[coord] == MineFieldBase.Mine)
                    nMines++;
            }

            return nMines;
        }

        private int NumHiddenFunc()
        {
            int nHidden = 0;

            foreach (var coord in GetNeighbors())
            {
                if (Field[coord] == MineFieldBase.Hidden)
                    nHidden++;
            }

            return nHidden;
        }
    }
}
