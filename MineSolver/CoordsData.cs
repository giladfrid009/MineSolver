using System.Collections.Generic;
using System.Linq;
using Minefield;

namespace Minesolver
{
    internal class CoordsData
    {
        public int Width { get; }
        public int Height { get; }

        public Coord this[int row, int col] => Data[row,col];

        private readonly Coord[,] Data;

        public CoordsData(Field field)
        {
            Width = field.Width;
            Height = field.Height;

            Data = new Coord[field.Height, field.Width];

            field.OnMove += SetVal;
            field.OnReset += GenData;

            GenData(field);
        }

        private void SetVal(Field sender, MoveArgs e)
        {
            Data[e.Row, e.Col].Value = e.Value;
        }

        private void GenData(Field sender)
        {
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    Data[row, col] = new Coord(row, col);
                }
            }

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    Data[row, col].Adjacent = GetAdj(row, col).ToArray();
                }
            }

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    Data[row, col].Value = sender[row, col];
                }
            }
        }

        IEnumerable<Coord> GetAdj(int row, int col)
        {
            for (int nRow = row - 1; nRow <= row + 1; nRow++)
            {
                if (nRow < 0 || nRow > Height - 1) continue;

                for (int nCol = col - 1; nCol <= col + 1; nCol++)
                {
                    if (nCol < 0 || nCol > Width - 1) continue;

                    if (nRow == row && nCol == col) continue;

                    yield return Data[nRow, nCol];
                }
            }
        }
    }
}
