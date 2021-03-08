using System;
using Minefield;

namespace Minesolver
{
    internal class Coord
    {
        public Coord[] Adjacent { get; set; } = Array.Empty<Coord>();
        public int Value { get => value; set => SetValue(value); }

        public ComboStats Stats { get; }
        public int Row { get; }
        public int Col { get; }
        public int NumOpen { get; private set; }
        public int NumFlags { get; private set; }

        public int NumAdj => Adjacent.Length;
        public int NumHidden => NumAdj - NumOpen - NumFlags;

        private int value = Field.Hidden;
      
        public Coord(int row, int col)
        {
            Row = row;
            Col = col;
            Stats = new ComboStats();
        }

        public bool IsValid()
        {
            if (Value < 0) return true; 

            if (Value < NumFlags) return false;

            if (NumHidden < Value - NumFlags) return false;

            return true;
        }

        private void SetValue(int newVal)
        {
            if (newVal == value) return;

            switch (value)
            {
                case Field.Hidden:
                    
                    if(newVal == Field.Mine) ApplyAdj(C => C.NumFlags++);
                    
                    if (newVal >= 0) ApplyAdj(C => C.NumOpen++);

                    break;

                case Field.Mine:

                    if (newVal == Field.Hidden) ApplyAdj(C => C.NumFlags--);

                    if (newVal >= 0) ApplyAdj(C => { C.NumFlags--; C.NumOpen++; });

                    break;

                default:

                    if (newVal == Field.Hidden) ApplyAdj(C => C.NumOpen--);

                    if (newVal == Field.Mine) ApplyAdj(C => { C.NumOpen--; C.NumFlags++; });

                    break;
            }

            value = newVal;
        }

        private void ApplyAdj(Action<Coord> action)
        {
            for (int i = 0; i < NumAdj; i++)
            {
                action(Adjacent[i]);
            }
        }
    }
}
