using System.Collections.Generic;
using System.Linq;
using System;
using Minesolver.Solvers.Advanced;

namespace Minesolver.Solvers
{
    internal class SolverAdvancedGuesser : SolverAdvancedBase
    {
        private readonly SolverAdvanced solverAdvanced;
        public double testVal = 0;

        public SolverAdvancedGuesser(MineFieldBase field) : base(field)
        {
            solverAdvanced = new SolverAdvanced(field);
        }

        public override SolveLog Solve()
        {
            Reset();

            while (fieldData.IsSolved == false)
            {
                MineFieldBase oldField = Field.Clone();

                log.Combine(solverAdvanced.Solve());

                UpdateFieldData(oldField);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (fieldData[x, y].TryAdvanced == false)
                            continue;

                        CalcTotals(x, y);

                        fieldData[x, y].TryAdvanced = false;
                    }
                }

                var (xMin, yMin) = FindMinimum();

                if(xMin == -1)
                {
                    break;
                }

                Field.Reveal(xMin, yMin);
                log.AddMove(xMin, yMin, Move.Reveal);

                if (HasLost)
                {
                    break;
                }

                UpdateCoordData(xMin, yMin, new HashSet<(int, int)>());             
            }

            return log.Clone();
        }

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
                        Console.SetCursorPosition(x, y);                     
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
