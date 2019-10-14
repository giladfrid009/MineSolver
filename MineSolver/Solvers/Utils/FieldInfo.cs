using System.Collections.Generic;
using System.Linq;

namespace MineSolver.Solvers.Utils
{
    public class FieldInfo<TCoordInfo> where TCoordInfo : CoordInfo, new()
    {
        public int Width { get => field.Width; }
        public int Height { get => field.Height; }
        public bool IsSolved { get => IsSolvedFunc(); }

        private readonly MineFieldBase field;

        private readonly TCoordInfo[,] coordsInfo;

        public FieldInfo(MineFieldBase field)
        {
            this.field = field;
            coordsInfo = new TCoordInfo[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    coordsInfo[x, y] = new TCoordInfo()
                    {                        
                        Field = field,
                        X = x,
                        Y = y
                    };
                }
            }
        }        

        public TCoordInfo this[int x, int y]
        {
            get => coordsInfo[x, y];
        }

        public TCoordInfo this[(int x, int y) coord]
        {
            get => coordsInfo[coord.x, coord.y];
        }

        private bool IsSolvedFunc()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (coordsInfo[x, y].IsSolved == false)
                        return false;
                }
            }

            return true;
        }
    }
}
