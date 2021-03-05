using System.Collections.Generic;
using Minefield;

namespace Minesolver
{
    public class Solver
    {
        private readonly Field field;
        private readonly FieldData fieldData;

        public Solver(Field field)
        {
            this.field = field;
            fieldData = new FieldData(field);
        }

        public void Solve()
        {
            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    SolveFrom(fieldData[row, col]);
                }
            }
        }

        private void SolveFrom(Coord origin)
        {
            if (origin.Value < 1) return;

            HashSet<Coord> oldAffected = new();
            HashSet<Coord> newAffected = new();

            field.OnMove += OnMove;

            oldAffected.Add(origin);

            while (true)
            {
                foreach (Coord C in oldAffected)
                {
                    oldAffected.Remove(C);

                    if (C.Value < 1) continue;

                    if (C.NumFlagged == C.Value)
                    {
                        foreach (Coord nCoord in C.Adjacent)
                        {
                            if (nCoord.Value != Field.Hidden) continue;

                            field.Reveal(nCoord.Row, nCoord.Col);
                        }
                    }
                    else if (C.NumHidden == C.Value - C.NumFlagged)
                    {
                        foreach (Coord nCoord in C.Adjacent)
                        {
                            if (nCoord.Value != Field.Hidden) continue;

                            field.Flag(nCoord.Row, nCoord.Col);
                        }
                    }
                }

                if (newAffected.Count == 0)
                {
                    field.OnMove -= OnMove;
                    return;
                }

                Swap(ref oldAffected, ref newAffected);                       
            }

            void OnMove(Field sender, MoveArgs e)
            {
                Coord C = fieldData[e.Row, e.Col];

                if (e.Move == Move.Reveal)
                {
                    newAffected.Add(C);

                    newAffected.UnionWith(C.Adjacent);
                }

                if (e.Move == Move.Flag)
                {
                    newAffected.UnionWith(C.Adjacent);
                }
            }
        }

        private void Swap<T>(ref T first, ref T second)
        {
            T tmp = first;

            first = second;

            second = tmp;
        }
    }
}
