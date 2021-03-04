using System;
using Minefield;

namespace Minesolver
{
    public class Coord
    {
        public Coord[] Adjacent { get; set; } = Array.Empty<Coord>();
        public int Row { get; }
        public int Col { get; }
        public int Value { get => value; set => SetValue(value); }
        public int NumRevealed { get; private set; }
        public int NumFlagged { get; private set; }

        public int NumAdj => Adjacent.Length;
        public int NumHidden => NumAdj - NumRevealed - NumFlagged;


        private int value = Field.Hidden;
      
        public Coord(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public bool IsValid()
        {
            if (Value >= 0 && NumFlagged > Value) return false;

            return true;
        }

        private void SetValue(int newVal)
        {
            if(newVal == Field.Hidden && value == Field.Mine)
            {
                foreach (var c in Adjacent)
                {
                    c.NumFlagged--;
                }
            }
            else if (newVal == Field.Mine && value == Field.Hidden)
            {
                foreach (var c in Adjacent)
                {
                    c.NumFlagged++;
                }
            }
            else if (newVal >= 0 && value == Field.Hidden)
            {
                foreach (var c in Adjacent)
                {
                    c.NumRevealed++;
                }
            }
            else
            {
                return;
            }

            value = newVal;
        }
    }
}
