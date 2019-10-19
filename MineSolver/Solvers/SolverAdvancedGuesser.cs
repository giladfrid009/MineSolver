using Minesolver.Solvers.Utils.Advanced;
using System.Collections.Generic;
using System.Linq;

namespace Minesolver.Solvers
{
    internal class SolverAdvancedGuesser : SolverBase<CoordDataAdvanced>
    {
        private readonly SolverAdvanced solverAdvanced;
        private readonly ComboLibrary comboLibrary;

        public SolverAdvancedGuesser(MineFieldBase field) : base(field)
        {
            solverAdvanced = new SolverAdvanced(field);
            comboLibrary = new ComboLibrary();
        }

        public override SolveLog Solve()
        {
            Reset();

            while (true)
            {
                MineFieldBase oldField = Field.Clone();

                log.Combine(solverAdvanced.Solve());

                if (HasLost)
                {
                    break;
                }

                if (fieldData.IsSolved)
                {
                    break;
                }

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {                        
                        CalcTotals(x, y);
                    }
                }

                PerformBestMove();

                if (HasLost)
                {
                    break;
                }

                UpdateFieldChanges(oldField);
            }

            return log.Clone();
        }

        // todo: verify that precentage are correct
        private bool CalcTotals(int x, int y)
        {
            bool result = false;

            if (fieldData[x, y].IsSolved || (fieldData[x, y].IsValue == false))
            {
                return result;
            }
            int nFlagsToComplete = Field[x, y] - fieldData[x, y].NumMines;

            List<(int X, int Y)> hidden = GetHiddenUnused(x, y);

            if (hidden.Count < nFlagsToComplete)
            {
                return result;
            }

            Combo[] combos = comboLibrary[hidden.Count, nFlagsToComplete];

            HashSet<(int X, int Y)> affected = new HashSet<(int X, int Y)>();

            foreach ((int x2, int y2) in hidden)
            {
                affected.UnionWith(GetValues(x2, y2));
            }

            foreach (Combo combo in combos)
            {
                combo.Apply(Field, fieldData, hidden);

                if (affected.All(coord => IsCoordValid(coord.X, coord.Y)))
                {
                    result = true;

                    foreach ((int X, int Y) coord in hidden)
                    {
                        fieldData[coord].TotalCombos++;

                        if (fieldData[coord].IsMine)
                        {
                            fieldData[coord].TotalFlagged++;
                        }
                    }
                }

                combo.Remove(Field, fieldData, hidden);
            }

            return result;
        }

        private bool IsCoordValid(int x, int y)
        {
            int nMines = fieldData[x, y].NumMines;

            if (nMines > Field[x, y])
            {
                return false;
            }

            if (nMines < Field[x, y] && CalcTotals(x, y) == false)
            {
                return false;
            }

            return true;
        }

        private void PerformBestMove()
        {
            double minPrecent = 1.0;
            double maxPrecent = 0.0;

            (int X, int Y) minCoord = (-1, -1);
            (int X, int Y) maxCoord = (-1, -1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (fieldData[x, y].IsRevealed || fieldData[x, y].TotalCombos == 0)
                    {
                        continue;
                    }

                    double coordPrecent = (double)fieldData[x, y].TotalFlagged / fieldData[x, y].TotalCombos;

                    if (coordPrecent < minPrecent)
                    {
                        minPrecent = coordPrecent;
                        minCoord = (x, y);
                    }
                    else if (coordPrecent > maxPrecent)
                    {
                        maxPrecent = coordPrecent;
                        maxCoord = (x, y);
                    }
                }
            }

            if (minPrecent < 1 - maxPrecent)
            {
                Field.Reveal(minCoord.X, minCoord.Y);
                log.AddMove(minCoord.X, minCoord.Y, Move.Reveal);
            }
            else
            {
                Field.Flag(maxCoord.X, maxCoord.Y);
                log.AddMove(maxCoord.X, maxCoord.Y, Move.Flag);
            }
        }

        private List<(int X, int Y)> GetHiddenUnused(int x, int y)
        {
            return GetHidden(x, y).Where(coord => fieldData[coord].UsedInCombo == false).ToList();
        }

        protected override void Reset()
        {
            log.Clear();

            HasLost = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldData[x, y].IsSolved = false;
                    fieldData[x, y].TryAdvanced = true;
                    fieldData[x, y].TotalCombos = 0;
                    fieldData[x, y].TotalFlagged = 0;
                }
            }
        }

        // todo: find a better way of finging what has changed.
        private void UpdateFieldChanges(MineFieldBase oldField)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (oldField[x, y] != Field[x, y])
                    {
                        if (fieldData[x, y].IsValue)
                        {
                            UpdateCoordChanges(x, y);
                        }
                        else
                        {
                            foreach ((int x2, int y2) in GetUnsolved(x, y))
                            {
                                UpdateCoordChanges(x2, y2);
                            }
                        }
                    }
                }
            }

        }

        private void UpdateCoordChanges(int x, int y)
        {
            if (fieldData[x, y].TryAdvanced)
            {
                return;
            }

            fieldData[x, y].TryAdvanced = true;
            fieldData[x, y].TotalCombos = 0;
            fieldData[x, y].TotalFlagged = 0;

            foreach ((int x2, int y2) in GetUnsolved(x, y))
            {
                UpdateCoordChanges(x2, y2);
            }
        }
    }
}
