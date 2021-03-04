using System.Collections.Generic;
using Minefield;

namespace Minesolver
{
    public class Solver
    {
        public Field field;

        public Solver(Field field)
        {
            this.field = field;
        }

        public void Solve()
        {
            FieldData fieldData = new FieldData(field);

            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    SolveFrom(fieldData[row, col]);
                }
            }
        }

        private void SolveFrom(Coord oCoord)
        {
            HashSet<Coord> oldAffected = new();
            HashSet<Coord> newAffected = new();

            oldAffected.Add(oCoord);

            while (true)
            {
                foreach (Coord aCoord in oldAffected)
                {
                    if (aCoord.Value == Field.Hidden || aCoord.Value == Field.Mine || aCoord.NumHidden == 0)
                    {
                        continue;
                    }

                    if (aCoord.NumFlagged == aCoord.Value)
                    {
                        foreach (Coord nCoord in aCoord.Adjacent)
                        {
                            if (nCoord.Value != Field.Hidden) continue;

                            field.Reveal(nCoord.Row, nCoord.Col);

                            newAffected.UnionWith(nCoord.Adjacent);
                        }
                    }
                    else if (aCoord.NumHidden == aCoord.Value - aCoord.NumFlagged)
                    {
                        foreach (Coord nCoord in aCoord.Adjacent)
                        {
                            if (nCoord.Value != Field.Hidden) continue;

                            field.Flag(nCoord.Row, nCoord.Col);

                            newAffected.UnionWith(nCoord.Adjacent);
                        }
                    }
                }

                if (newAffected.Count == 0) return;

                oldAffected = newAffected;

                newAffected.Clear();
            }
        }
    }
}
