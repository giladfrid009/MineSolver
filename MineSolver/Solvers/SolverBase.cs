using System;
using System.Linq;
using System.Collections.Generic;
using MineSolver.Solvers.Utils;

namespace MineSolver.Solvers
{
    public abstract class SolverBase<TCoordInfo> where TCoordInfo : CoordData, new()
    {
        protected readonly FieldData<TCoordInfo> fieldData;
        protected readonly MineFieldBase field;
        protected readonly int width;
        protected readonly int height;

        public SolverBase(MineFieldBase field)
        {
            fieldData = new FieldData<TCoordInfo>(field);
            this.field = field;
            width = field.Width;
            height = field.Height;
        }

        public abstract SolveLog Solve();


        protected List<(int X, int Y)> GetHidden(int x, int y)
        {
            return fieldData[x, y].Neighbors.Where(coord => fieldData[coord].IsRevealed == false).ToList();
        }

        protected List<(int X, int Y)> GetValues(int x, int y)
        {
            return fieldData[x, y].Neighbors.Where(coord => fieldData[coord].IsValue).ToList();
        }

        protected List<(int X, int Y)> GetUnsolved(int x, int y)
        {
            return GetValues(x, y).Where(coord => fieldData[coord].IsSolved == false).ToList();
        }

        protected List<(int X, int Y)> GetAreaBounds(int x, int y)
        {           
            var coords = new HashSet<(int X, int Y)>();

            if (field[x, y] != 0)
                throw new Exception();

            GetOpenedAreaRecursive(x, y, coords);

            return coords.Where(coord => field[coord.X, coord.Y] != 0).ToList();
        }

        private void GetOpenedAreaRecursive(int x, int y, HashSet<(int, int)> coords)
        {
            if (coords.Contains((x, y)))
                return;

            coords.Add((x, y));

            if (field[x, y] == 0)
            {
                foreach(var (x2, y2) in fieldData[x,y].Neighbors)
                {
                    GetOpenedAreaRecursive(x2, y2, coords);
                }
            }
        }
    }
}
