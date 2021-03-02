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

        public void Apply<TCoordData>(Field<TCoordData> fieldData, List<(int X, int Y)> coords) where TCoordData : CoordAdvanced
        {
            if (coords.Count != Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < Length; i++)
            {
                (int x, int y) = coords[i];

                if (vals[i])
                {
                    fieldData[x, y].ForceFlag = true;
                }
                else
                {
                    fieldData[x, y].ForceReveal = true;
                }
            }
        }

        public void Remove<TCoordData>(Field<TCoordData> fieldData, List<(int X, int Y)> coords) where TCoordData : CoordAdvanced
        {
            if (coords.Count != Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < Length; i++)
            {
                (int x, int y) = coords[i];

                fieldData[x, y].ForceFlag = false;
                fieldData[x, y].ForceReveal = false;
            }
        }
    }
}
