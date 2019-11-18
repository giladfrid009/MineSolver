namespace Minesolver.Solvers.Basic
{
    public class FieldData<TCoordInfo> where TCoordInfo : CoordData, new()
    {
        public int Width => field.Width;
        public int Height => field.Height;
        public bool IsFieldSolved => IsFieldSolvedFunc();

        private readonly FieldBase field;

        private readonly TCoordInfo[,] coordsData;

        public FieldData(FieldBase field)
        {
            this.field = field;
            coordsData = new TCoordInfo[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    coordsData[x, y] = new TCoordInfo();
                    coordsData[x, y].Initialize(x, y, field);
                }
            }
        }

        public TCoordInfo this[int x, int y] => coordsData[x, y];

        public TCoordInfo this[(int x, int y) coord] => coordsData[coord.x, coord.y];

        private bool IsFieldSolvedFunc()
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

        public int NumMines(int x, int y)
        {
            int num = 0;

            foreach ((int x2, int y2) in coordsData[x, y].Neighbors)
            {
                if (this[x2, y2].IsFlagged)
                {                    
                    num++;
                }                
            }

            return num;
        }

        public int NumHidden(int x, int y)
        {
            int num = 0;

            foreach ((int x2, int y2) in coordsData[x, y].Neighbors)
            {
                if (coordsData[x2, y2].IsRevealed == false)
                {
                    num++;
                }
            }

            return num;
        }

        public bool IsSolved(int x, int y)
        {
            return NumMines(x, y) == field[x, y] && NumHidden(x, y) == 0;
        }
    }
}
