using System;
using System.Linq;
using System.Collections.Generic;

namespace MineSolver
{
    public class CoordInfo
    {
        public IMIneField Field { get; set; }

        public bool IsSolved { get => IsSolvedFunc(); }
        public bool IsRevealed { get => Value != Field.ValHidden ? true : false; }
        public bool IsMine { get => Value == Field.ValMine ? true : false; }
        public bool IsValue { get => IsRevealed && !IsMine; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get => Field[X, Y]; }
        public int NumMines { get => NumFlagsFunc(); }
        public int NumHidden { get => NumHiddenFunc(); }
        public List<(int X, int Y)> NeighborCoords { get => GetNeighbors(); }
        public List<(int X, int Y)> HiddenCoords { get => GetHidden(); }
        public List<(int X, int Y)> ValueCoords { get => GetValues(); }
        public List<(int X, int Y)> MineCoords { get => GetMines(); }

        private bool isSolvedCache;
        private List<(int, int)> neighborsCache;

        private bool IsSolvedFunc()
        {
            if (isSolvedCache == true)
                return true;

            foreach (var coord in NeighborCoords)
            {
                if (Field[coord] == Field.ValHidden)
                    return false;
            }

            isSolvedCache = true;

            return true;
        }

        private int NumFlagsFunc()
        {
            int nFlags = 0;

            foreach (var coord in NeighborCoords)
            {
                if (Field[coord] == Field.ValMine)
                    nFlags++;          
            }

            return nFlags;
        }

        private int NumHiddenFunc()
        {
            int nHidden = 0;

            foreach (var coord in NeighborCoords)
            {
                if (Field[coord] == Field.ValHidden)
                    nHidden++;
            }

            return nHidden;
        }

        private List<(int, int)> GetNeighbors()
        {
            if (neighborsCache != null)
                return neighborsCache;

            neighborsCache = new List<(int, int)>(3);

            int xLow = Math.Max(0, X - 1);
            int xHigh = Math.Min(X + 1, Field.Width - 1);
            int yLow = Math.Max(0, Y - 1);
            int yHigh = Math.Min(Y + 1, Field.Height - 1);

            for (int xNeighbor = xLow; xNeighbor <= xHigh; xNeighbor++)
            {
                for (int yNeighbor = yLow; yNeighbor <= yHigh; yNeighbor++)
                {
                    if (xNeighbor == X && yNeighbor == Y)
                        continue;

                    neighborsCache.Add((xNeighbor, yNeighbor));
                }
            }

            return neighborsCache;
        }

        private List<(int X, int Y)> GetHidden()
        {
            return NeighborCoords.Where(coord => Field[coord] == Field.ValHidden).ToList();
        }

        private List<(int X, int Y)> GetValues()
        {
            return NeighborCoords.Where(coord => (Field[coord] != Field.ValHidden) && (Field[coord] != Field.ValMine)).ToList();
        }

        private List<(int X, int Y)> GetMines()
        {
            return NeighborCoords.Where(coord => Field[coord] == Field.ValMine).ToList();
        }
    }
}
