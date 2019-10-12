using System;
using System.Collections.Generic;

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
    }
}
