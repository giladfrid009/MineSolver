using System;
using System.Collections.Generic;

namespace Minesolver.Solvers.Advanced
{
    public class Combo
    {
        public int Length { get; }
        public int NumMines { get; }

        private readonly bool[] vals;

        public Combo(bool[] vals)
        {
            Length = vals.Length;

            this.vals = new bool[Length];

            for (int i = 0; i < Length; i++)
            {
                this.vals[i] = vals[i];

                if (this.vals[i])
                {
                    NumMines++;
                }
            }
        }

        public bool this[int index] => vals[index];

        public void Apply<TCoord>(Field<TCoord> field, List<(int X, int Y)> coords) where TCoord : CoordAdvanced
        {
            if (coords.Count != Length)
            {
                throw new ArgumentOutOfRangeException(nameof(coords));
            }

            for (int i = 0; i < Length; i++)
            {
                (int x, int y) = coords[i];

                if (vals[i])
                {
                    field[x, y].ForceFlag = true;
                }
                else
                {
                    field[x, y].ForceReveal = true;
                }
            }
        }

        public void Remove<TCoord>(Field<TCoord> field, List<(int X, int Y)> coords) where TCoord : CoordAdvanced
        {
            if (coords.Count != Length)
            {
                throw new ArgumentOutOfRangeException(nameof(coords));
            }

            for (int i = 0; i < Length; i++)
            {
                (int x, int y) = coords[i];

                field[x, y].ForceFlag = false;
                field[x, y].ForceReveal = false;
            }
        }
    }
}
