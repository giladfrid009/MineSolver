using System;
using System.Collections.Generic;
using System.Linq;

namespace MineSolver.Solvers.Complex
{
    public class Combo
    {
        public int Length { get; }
        public int NumFlags { get; }

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
                    NumFlags++;
                }
            }
        }

        public bool this[int index]
        {
            get => comboVals[index];
        }

        public static bool?[] FindCommon(List<Combo> combos)
        {
            var firstCombo = combos[0];
            var length = firstCombo.Length;

            bool?[] common = new bool?[length];

            for (int i = 0; i < length; i++)
            {
                common[i] = firstCombo[i];

                foreach(var combo in combos)
                {
                    if(combo[i] != common[i])
                    {
                        common[i] = null;
                        break;
                    }
                }
            }

            return common;
        }        

        public void Apply(MineFieldBase field ,List<(int X, int Y)> coords)
        {
            if (coords.Count != Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < Length; i++)
            {
                if (comboVals[i])
                {
                    field.Flag(coords[i].X, coords[i].Y);
                }
            }
        }

        public void Remove(MineFieldBase field, List<(int X, int Y)> coords)
        {
            if (coords.Count != Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < Length; i++)
            {
                if(comboVals[i])
                {
                    field.Unflag(coords[i].X, coords[i].Y);
                }
            }
        }
    }
}
