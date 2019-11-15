using System;
using System.Collections.Generic;
using Minesolver.Solvers.Basic;

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

        public void Apply<TCoordData>(MineFieldBase field, FieldData<TCoordData> fieldData, List<(int X, int Y)> coords) where TCoordData : CoordDataAdvanced, new()
        {
            if (coords.Count != Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < Length; i++)
            {
                (int x, int y) = coords[i];

                fieldData[x, y].UsedInCombo = true;

                if (vals[i])
                {
                    field.Flag(x, y);
                }
            }
        }

        public void Remove<TCoordData>(MineFieldBase field, FieldData<TCoordData> fieldData, List<(int X, int Y)> coords) where TCoordData : CoordDataAdvanced, new()
        {
            if (coords.Count != Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < Length; i++)
            {
                (int x, int y) = coords[i];

                fieldData[x, y].UsedInCombo = false;

                if (vals[i])
                {
                    field.Unflag(x, y);
                }
            }
        }
    }
}
