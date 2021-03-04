using System;

namespace Minesolver.Solvers
{
    public class Field<TCoord> where TCoord : Coord
    {
        public int Width => field.Width;
        public int Height => field.Height;

        protected readonly BaseField field;

        protected readonly TCoord[,] coords;

        public Field(BaseField field)
        {
            this.field = field;
            coords = new TCoord[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    coords[x, y] = (TCoord)Activator.CreateInstance(typeof(TCoord), x, y, field)!;
                }
            }
        }

        public TCoord this[int x, int y] => coords[x, y];

        public TCoord this[(int x, int y) coord] => coords[coord.x, coord.y];

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
            return field[x, y] != BaseField.Hidden;
        }

        public bool IsFlagged(int x, int y)
        {
            return field[x, y] == BaseField.Mine;
        }

        public bool IsValue(int x, int y)
        {
            return IsRevealed(x, y) && !IsFlagged(x, y);
        }

        public virtual int NumMines(int x, int y)
        {
            int num = 0;

            foreach ((int x2, int y2) in coords[x, y].Neighbors)
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

            foreach ((int x2, int y2) in coords[x, y].Neighbors)
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
