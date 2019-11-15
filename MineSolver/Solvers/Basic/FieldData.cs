﻿namespace Minesolver.Solvers.Basic
{
    public class FieldData<TCoordInfo> where TCoordInfo : CoordData, new()
    {
        public int Width => field.Width;
        public int Height => field.Height;
        public bool IsSolved => IsSolvedFunc();

        private readonly MineFieldBase field;

        private readonly TCoordInfo[,] coordsData;

        public FieldData(MineFieldBase field)
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

        private bool IsSolvedFunc()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (coordsData[x, y].IsSolved == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}