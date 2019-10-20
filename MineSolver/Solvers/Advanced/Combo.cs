using System;
using System.Collections.Generic;
using Minesolver.Solvers.Basic;

namespace Minesolver.Solvers.Advanced
{
    public class Combo
    {
        public int Length { get; }
        public int NumMines { get; }

        private readonly bool[] comboVals;

        public Combo(bool[] comboVals)
        {
            Length = comboVals.Length;

            this.comboVals = new bool[Length];

            for (int i = 0; i < Length; i++)
            {
                this.comboVals[i] = comboVals[i];

                if (this.comboVals[i])
                {
                    NumMines++;
                }
            }
        }

        public bool this[int index] => comboVals[index];

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

                if (comboVals[i])
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

                if (comboVals[i])
                {
                    field.Unflag(x, y);
                }
            }
        }
    }
}
