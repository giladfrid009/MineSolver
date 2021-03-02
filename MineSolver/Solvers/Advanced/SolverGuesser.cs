using Minesolver.Solvers.Advanced;
using System;
using System.Collections.Generic;

namespace Minesolver.Solvers
{
    internal class SolverGuesser : BaseSolverAdvanced
    {
        private readonly SolverAdvanced solverAdvanced;

        public uint MaxDepthSecondary
        {
            get => solverAdvanced.MaxDepth;
            set => solverAdvanced.MaxDepth = value;
        }

        public SolverGuesser(BaseField field) : base(field)
        {
            solverAdvanced = new SolverAdvanced(field);
        }

        public override MoveLog Solve()
        {
            Reset();

            while (fieldData.IsFieldSolved() == false)
            {
                FieldState oldState = new FieldState(Field);

                log.Combine(solverAdvanced.Solve());

                if (HasLost)
                {
                    return log.Clone();
                }

                ResetFieldChanges(oldState);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (fieldData[x, y].TryAdvanced == false)
                        {
                            continue;
                        }

                        CalcTotals(x, y, 1);

                        fieldData[x, y].TryAdvanced = false;
                    }
                }

                (int xMin, int yMin) = FindMinimum();
                
                if (xMin == -1)
                {
                    break;
                }

                Field.Reveal(xMin, yMin);
                log.Add(xMin, yMin, Move.Reveal);

                if (HasLost)
                {
                    break;
                }

                ResetCoordData(xMin, yMin, new HashSet<(int, int)>());
            }

            return log.Clone();
        }

        private bool CalcTotals(int x, int y, uint depth)
        {
            if (fieldData.IsSolved(x, y) || (fieldData.IsValue(x, y) == false))
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

            if (depth > MaxDepth)
            {
                return true;
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

                if (IsComboValid(affected, depth + 1))
                {
                    result = true;

                    foreach ((int X, int Y) coord in hidden)
                    {
                        fieldData[coord].TotalCombos++;

                        if (fieldData[coord].ForceFlag)
                        {
                            fieldData[coord].TotalFlagged++;
                        }
                    }
                }

                combo.Remove(fieldData, hidden);
            }

            return result;
        }

        protected override bool IsCoordValid(int x, int y, uint depth)
        {
            int nMines = fieldData.NumMines(x, y);

            if (nMines > Field[x, y])
            {
                return false;
            }

            if (nMines < Field[x, y] && CalcTotals(x, y, depth) == false)
            {
                return false;
            }

            if (nMines == Field[x, y])
            {
                CalcTotals(x, y, depth);
            }

            return true;
        }

        // todo: we should find biggest peak, not min.
        private (int X, int Y) FindMinimum()
        {
            double minPrecent = 1.0;

            (int X, int Y) minCoord = (-1, -1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (fieldData.IsRevealed(x, y) || fieldData[x, y].TotalCombos == 0)
                    {
                        continue;
                    }

                    double precent = (double)fieldData[x, y].TotalFlagged / fieldData[x, y].TotalCombos;

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
