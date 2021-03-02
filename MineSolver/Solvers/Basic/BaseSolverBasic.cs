using System;
using System.Collections.Generic;
using System.Linq;

namespace Minesolver.Solvers.Basic
{
    public abstract class BaseSolverBasic : BaseSolver<Field<Coord>, Coord>
    {
        public BaseSolverBasic(BaseField field) : base(field)
        {

        }

        protected HashSet<(int X, int Y)> RevealHidden(int x, int y)
        {
            HashSet<(int, int)> affected = new HashSet<(int, int)>();

            List<(int X, int Y)> hidden = GetHidden(x, y);

            foreach ((int x2, int y2) in hidden)
            {
                Field.Reveal(x2, y2);
                log.Add(x2, y2, Move.Reveal);
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
                log.Add(x2, y2, Move.Flag);
            }

            foreach ((int x2, int y2) in hidden)
            {
                affected.UnionWith(GetUnsolved(x2, y2));
            }

            return affected;
        }

        private List<(int X, int Y)> GetOpenAreaBounds(int x, int y)
        {
            HashSet<(int X, int Y)> area = new HashSet<(int X, int Y)>();

            if (Field[x, y] != 0)
            {
                throw new Exception();
            }

            GetOpenedAreaRecursive(x, y, area);

            return area.Where(coord => Field[coord] != 0 && fieldData.IsSolved(coord.X, coord.Y) == false).ToList();
        }

        private void GetOpenedAreaRecursive(int x, int y, HashSet<(int, int)> area)
        {
            if (area.Contains((x, y)))
            {
                return;
            }

            area.Add((x, y));

            if (Field[x, y] == 0)
            {
                foreach ((int x2, int y2) in fieldData[x, y].Neighbors)
                {
                    GetOpenedAreaRecursive(x2, y2, area);
                }
            }
        }
    }
}
