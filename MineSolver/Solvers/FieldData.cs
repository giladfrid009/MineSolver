using System;

namespace Minesolver.Solvers
{
    public class FieldData<TCoordData> where TCoordData : CoordData
    {
        public int Width => Field.Width;
        public int Height => Field.Height;

        protected readonly FieldBase Field;

        protected readonly TCoordData[,] CoordsData;

        public FieldData(FieldBase field)
        {
            Field = field;
            CoordsData = new TCoordData[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    CoordsData[x, y] = (TCoordData)Activator.CreateInstance(typeof(TCoordData), x, y, field)!;
                }
            }
        }

        public TCoordData this[int x, int y] => CoordsData[x, y];

        public TCoordData this[(int x, int y) coord] => CoordsData[coord.x, coord.y];

        public bool IsFieldSolved()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (IsSolved(x, y) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsSolved(int x, int y)
        {
            return NumMines(x, y) == Field[x, y] && NumHidden(x, y) == 0;
        }

        public bool IsRevealed(int x, int y)
        {
            return Field[x, y] != FieldBase.Hidden ? true : false;
        }

        public bool IsFlagged(int x, int y)
        {
            return Field[x, y] == FieldBase.Mine ? true : false;
        }

        public bool IsValue(int x, int y)
        {
            return IsRevealed(x, y) && !IsFlagged(x, y);
        }

        public virtual int NumMines(int x, int y)
        {
            int num = 0;

            foreach ((int x2, int y2) in CoordsData[x, y].Neighbors)
            {
                if (IsFlagged(x2, y2))
                {
                    num++;
                }
            }

            return num;
        }

        public virtual int NumHidden(int x, int y)
        {
            int num = 0;

            foreach ((int x2, int y2) in CoordsData[x, y].Neighbors)
            {
                if (IsRevealed(x2, y2) == false)
                {
                    num++;
                }
            }

            return num;
        }
    }
}
