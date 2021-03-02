using System;

namespace Minesolver.Solvers
{
    public class Field<TCoordData> where TCoordData : Coord
    {
        public int Width => field.Width;
        public int Height => field.Height;

        protected readonly BaseField field;

        protected readonly TCoordData[,] coordsData;

        public Field(BaseField field)
        {
            this.field = field;
            coordsData = new TCoordData[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    coordsData[x, y] = (TCoordData)Activator.CreateInstance(typeof(TCoordData), x, y, field)!;
                }
            }
        }

        public TCoordData this[int x, int y] => coordsData[x, y];

        public TCoordData this[(int x, int y) coord] => coordsData[coord.x, coord.y];

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
            return NumMines(x, y) == field[x, y] && NumHidden(x, y) == 0;
        }

        public bool IsRevealed(int x, int y)
        {
            return field[x, y] != BaseField.Hidden ? true : false;
        }

        public bool IsFlagged(int x, int y)
        {
            return field[x, y] == BaseField.Mine ? true : false;
        }

        public bool IsValue(int x, int y)
        {
            return IsRevealed(x, y) && !IsFlagged(x, y);
        }

        public virtual int NumMines(int x, int y)
        {
            int num = 0;

            foreach ((int x2, int y2) in coordsData[x, y].Neighbors)
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

            foreach ((int x2, int y2) in coordsData[x, y].Neighbors)
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
