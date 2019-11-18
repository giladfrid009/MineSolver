using Minesolver.Solvers.Advanced;
using System;
using System.Collections.Generic;

namespace Minesolver.Solvers
{
    internal class SolverAdvancedGuesser : SolverAdvancedBase
    {
        private readonly SolverAdvanced solverAdvanced;
        public double testVal = 0;

        public SolverAdvancedGuesser(FieldBase field) : base(field)
        {
            solverAdvanced = new SolverAdvanced(field);
        }

        public override SolveLog Solve()
        {
            Reset();

            while (fieldData.IsFieldSolved == false)
            {
                FieldState oldState = new FieldState(Field);

                log.Combine(solverAdvanced.Solve());

                ResetFieldChanges(oldState);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (fieldData[x, y].TryAdvanced == false)
                        {
                            continue;
                        }

                        CalcTotals(x, y);

                        fieldData[x, y].TryAdvanced = false;
                    }
                }

                (int xMin, int yMin) = FindMinimum();

                if (xMin == -1)
                {
                    break;
                }

                Field.Reveal(xMin, yMin);
                log.AddMove(xMin, yMin, Move.Reveal);

                if (HasLost)
                {
                    break;
                }

                ResetCoordData(xMin, yMin, new HashSet<(int, int)>());
            }

            return log.Clone();
        }

        private bool CalcTotals(int x, int y)
        {
            if (fieldData.IsSolved(x, y) || (fieldData[x, y].IsValue == false))
            {
                return false;
            }

            List<(int X, int Y)> hidden = GetNotForced(GetHidden(x, y));

            if (hidden.Count == 0)
            {
                return false;
            }

            int nFlagsMissing = Field[x, y] - fieldData.NumMines(x, y);

            if (hidden.Count < nFlagsMissing)
            {
                return false;
            }

            Combo[] combos = comboLibrary[hidden.Count, nFlagsMissing];

            HashSet<(int X, int Y)> affected = new HashSet<(int X, int Y)>();

            foreach ((int x2, int y2) in hidden)
            {
                affected.UnionWith(GetValues(x2, y2));
            }

            bool result = false;

            foreach (Combo combo in combos)
            {
                combo.Apply(fieldData, hidden);

                if (IsComboValid(affected))
                {
                    result = true;

                    foreach ((int X, int Y) coord in hidden)
                    {
                        fieldData[coord].TotalCombos++;

                        if (fieldData[coord].IsFlagged)
                        {
                            fieldData[coord].TotalFlagged++;
                        }
                    }
                }

                combo.Remove(fieldData, hidden);
            }

            return result;
        }

        protected override bool IsCoordValid(int x, int y)
        {
            int nMines = fieldData.NumMines(x, y);

            if (nMines > Field[x, y])
            {
                return false;
            }

            if (nMines < Field[x, y] && CalcTotals(x, y) == false)
            {
                return false;
            }

            if (nMines == Field[x, y])
            {
                CalcTotals(x, y);
            }

            return true;
        }

        private (int X, int Y) FindMinimum()
        {
            double minPrecent = 1.0;

            (int X, int Y) minCoord = (-1, -1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (fieldData[x, y].IsRevealed || fieldData[x, y].TotalCombos == 0)
                    {
                        continue;
                    }

                    double precent = (double)fieldData[x, y].TotalFlagged / fieldData[x, y].TotalCombos;

                    if (precent == 0)
                    {
                        throw new Exception();
                    }

                    if (precent < minPrecent)
                    {
                        minPrecent = precent;
                        minCoord = (x, y);
                    }
                }
            }

            return minCoord;
        }
    }
}
