﻿using System.Collections.Generic;
using System.Linq;
using Minesolver.Solvers.Utils.Advanced;

namespace Minesolver.Solvers
{
    class SolverAdvanced : SolverBase<CoordDataAdvanced>
    {
        private readonly SolverBasic solverBasic;
        private readonly ComboLibrary comboLibrary;

        public SolverAdvanced(MineFieldBase field) : base(field)
        {
            solverBasic = new SolverBasic(field);
            comboLibrary = new ComboLibrary();
        }

        public override SolveLog Solve()
        {
            Reset();

            bool progress;

            while (true)
            {
                var oldField = Field.Clone();

                progress = false;

                log.Combine(solverBasic.Solve());

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (SolveCoord(x, y))
                        {
                            progress = true;
                            x = width;
                            y = height;
                        }
                    }
                }

                if (progress == false)
                    break;

                UpdateFieldChanges(oldField);
            }

            // then make a guess 

            return log.Clone();
        }

        private bool SolveCoord(int x, int y)
        {
            bool result = false;

            if (fieldData[x, y].TryAdvanced == false)
            {
                return result;
            }

            fieldData[x, y].TryAdvanced = false;

            if (fieldData[x, y].IsSolved || (fieldData[x, y].IsValue == false))
            {
                return result;
            }

            CalcTotals(x, y);

            var hidden = GetHidden(x, y);

            foreach (var (x2, y2) in hidden)
            {
                if (fieldData[x2, y2].TotalFlagged == 0)
                {
                    Field.Reveal(x2, y2);
                    log.AddMove(x2, y2, Move.Reveal);
                    result = true;
                }
                else if (fieldData[x2, y2].TotalFlagged == fieldData[x2, y2].TotalCombos)
                {
                    Field.Flag(x2, y2);
                    log.AddMove(x2, y2, Move.Flag);
                    result = true;
                }
            }

            foreach (var coord in hidden)
            {
                fieldData[coord].TotalCombos = 0;
                fieldData[coord].TotalFlagged = 0;
            }

            return result;
        }


        private void CalcTotals(int x, int y)
        {
            int nFlagsToComplete = Field[x, y] - fieldData[x, y].NumMines;

            var hidden = GetHiddenUnused(x, y);

            var combos = comboLibrary[hidden.Count, nFlagsToComplete];

            var effected = new HashSet<(int X, int Y)>();

            foreach (var (x2, y2) in hidden)
            {
                effected.UnionWith(GetValues(x2, y2));
            }

            foreach (var combo in combos)
            {
                combo.Apply(Field, fieldData, hidden);

                if (effected.All(coord => IsCoordValid(coord.X, coord.Y)))
                {
                    foreach (var coord in hidden)
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
        }

        private bool AreValidCombos(int x, int y)
        {
            bool result = false;

            int nFlagsToComplete = Field[x, y] - fieldData[x, y].NumMines;

            var hidden = GetHiddenUnused(x, y);

            if (hidden.Count < nFlagsToComplete)
            {
                return result;
            }

            var effected = new HashSet<(int X, int Y)>();

            foreach (var (x2, y2) in hidden)
            {
                effected.UnionWith(GetValues(x2, y2));
            }

            var combos = comboLibrary[hidden.Count, nFlagsToComplete];

            foreach (var combo in combos)
            {
                combo.Apply(Field, fieldData, hidden);                

                if (effected.All(coord => IsCoordValid(coord.X, coord.Y)))
                {
                    result = true;

                    combo.Remove(Field, fieldData, hidden);

                    break;
                }

                combo.Remove(Field, fieldData, hidden);
            }

            return result;
        }

        private bool IsCoordValid(int x, int y)
        {
            int nMines = fieldData[x, y].NumMines;

            if (nMines > Field[x, y])
                return false;

            if (nMines < Field[x, y] && AreValidCombos(x, y) == false)
                return false;

            return true;
        }

        private List<(int X, int Y)> GetHiddenUnused(int x, int y)
        {
            return GetHidden(x, y).Where(coord => fieldData[coord].UsedInCombo == false).ToList();
        }

        protected override void Reset()
        {
            log.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldData[x, y].IsSolved = false;
                    fieldData[x, y].TryAdvanced = true;
                    fieldData[x, y].TotalFlagged = 0;
                    fieldData[x, y].TotalCombos = 0;
                }
            }
        }

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
                            foreach (var (x2, y2) in GetUnsolved(x, y))
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
                return;

            fieldData[x, y].TryAdvanced = true;

            foreach (var (x2, y2) in GetUnsolved(x, y))
            {
                UpdateCoordChanges(x2, y2);
            }
        }

    }
}