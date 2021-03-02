using System;
using System.Collections.Generic;
using System.Text;

namespace Minesolver.Solvers
{
    public enum Move { Flag, Reveal }

    public class MoveLog : IClonable<MoveLog>
    {
        public List<((int X, int Y) Coord, Move Move)> Moves { get; set; }

        public MoveLog()
        {
            Moves = new List<((int, int), Move)>();
        }

        public void Add(int x, int y, Move move)
        {
            Moves.Add(((x, y), move));
        }

        public void Combine(MoveLog other)
        {
            Moves.AddRange(other.Moves);
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder(6 * Moves.Count);

            for (int i = 0; i < Moves.Count; i++)
            {
                str.Append(string.Format("{0}: {1} \n", i, Moves[i]));
            }

            return str.ToString();
        }

        public MoveLog Clone()
        {
            MoveLog clone = new MoveLog();
            clone.Combine(this);

            return clone;
        }
    }
}
