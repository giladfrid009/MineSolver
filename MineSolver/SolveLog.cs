using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MineSolver
{
    public enum Move { Flag, Reveal }

    public class SolveLog
    {       
        public ReadOnlyCollection<((int X, int Y) Coord, Move Move)> MoveLog { get => moveLog.AsReadOnly(); }

        private readonly List<((int X, int Y) Coord, Move Move)> moveLog;

        public SolveLog()
        {
            moveLog = new List<((int, int), Move)>();
        }

        public void AddMove(int x, int y, Move move)
        {
            moveLog.Add(((x, y), move));
        }

        public void Combine(SolveLog other)
        {
            moveLog.AddRange(other.moveLog);
        }

        public void Print()
        {
            Console.WriteLine(ToString());
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder(6 * moveLog.Count);

            for (int i = 0; i < moveLog.Count; i++)
            {
                str.Append(string.Format("{0}: {1} \n", i, moveLog[i]));
            }

            return str.ToString();
        }
    }
}
