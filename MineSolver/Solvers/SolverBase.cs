using System;
using System.Collections.Generic;
using System.Linq;
using Minesolver.Solvers.Basic;

namespace Minesolver.Solvers
{
    public abstract class SolverBase<TCoordData> where TCoordData : CoordData, new()
    {
        public MineFieldBase Field { get; }
        public bool HasLost { get; protected set; } = false;

        protected readonly FieldData<TCoordData> fieldData;
        protected readonly SolveLog log;
        protected readonly int width;
        protected readonly int height;

        public SolverBase(MineFieldBase field)
        {
            fieldData = new FieldData<TCoordData>(field);
            Field = field;
            log = new SolveLog();
            width = field.Width;
            height = field.Height;

            field.OnLoss += (x, y) => HasLost = true;
        }

        public abstract SolveLog Solve();

        protected HashSet<(int X, int Y)> RevealHidden(int x, int y)
        {
            HashSet<(int, int)> affected = new HashSet<(int, int)>();

            List<(int X, int Y)> hidden = GetHidden(x, y);

            foreach ((int x2, int y2) in hidden)
            {
                Field.Reveal(x2, y2);
                log.AddMove(x2, y2, Move.Reveal);
            }

            foreach ((int x2, int y2) in hidden)
            {
                if (Field[x2, y2] == 0)
                {
                    List<(int X, int Y)> opened = GetOpenAreaBounds(x2, y2);

                    affected.UnionWith(opened);

                    foreach ((int x3, int y3) in opened)
                    {
                        affected.UnionWith(GetUnsolved(x3, y3));
                    }
                }
                else
                {
                    affected.Add((x2, y2));
                    affected.UnionWith(GetUnsolved(x2, y2));
                }
            }

            return affected;
        }

        protected HashSet<(int X, int Y)> FlagHidden(int x, int y)
        {
            HashSet<(int, int)> affected = new HashSet<(int, int)>();

            List<(int X, int Y)> hidden = GetHidden(x, y);

            foreach ((int x2, int y2) in hidden)
            {
                Field.Flag(x2, y2);
                log.AddMove(x2, y2, Move.Flag);
            }

            foreach ((int x2, int y2) in hidden)
            {
                affected.UnionWith(GetUnsolved(x2, y2));
            }

            return affected;
        }

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

        protected List<(int X, int Y)> GetOpenAreaBounds(int x, int y)
        {
            HashSet<(int X, int Y)> coords = new HashSet<(int X, int Y)>();

            if (Field[x, y] != 0)
            {
                throw new Exception();
            }

            GetOpenedAreaRecursive(x, y, coords);

            return coords.Where(coord => Field[coord] != 0).ToList();
        }

        private void GetOpenedAreaRecursive(int x, int y, HashSet<(int, int)> coords)
        {
            if (coords.Contains((x, y)))
            {
                return;
            }

            coords.Add((x, y));

            if (Field[x, y] == 0)
            {
                foreach ((int x2, int y2) in fieldData[x, y].Neighbors)
                {
                    GetOpenedAreaRecursive(x2, y2, coords);
                }
            }
        }

        protected virtual void Reset()
        {
            log.Clear();

            HasLost = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldData[x, y].IsSolved = false;
                }
            }
        }
    }
}
