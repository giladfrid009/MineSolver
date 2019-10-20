using System.Collections.Generic;
using System.Linq;
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

            while (true)
            {
                MineFieldBase oldField = Field.Clone();

                log.Combine(solverAdvanced.Solve());

                UpdateField(oldField);

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
                        if (fieldData[x, y].TryAdvanced == false)
                            continue;

                        CalcTotals(x, y);

                        fieldData[x, y].TryAdvanced = false;
                    }
                }

                if (PerformBestMove() == false)
                {
                    return log.Clone();
                }

                if (HasLost)
                {
                    break;
                }

                UpdateField(oldField);
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

        private bool PerformBestMove()
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

            if(minPrecent == 0.0 && maxPrecent == 1.0)
            {            
                throw new System.Exception();
            }

            if(minPrecent == 1.0 && maxPrecent == 0.0)
            {
                return false;
            }

            if (minPrecent < 1 - maxPrecent)
            {
                Field.Reveal(minCoord.X, minCoord.Y);
                log.AddMove(minCoord.X, minCoord.Y, Move.Reveal);

                // todo: test
                if(HasLost == false)
                {
                    testVal += minPrecent;
                }
                else
                {
                    testVal -= 1 - minPrecent;
                }
            }
            else
            {
                Field.Flag(maxCoord.X, maxCoord.Y);
                log.AddMove(maxCoord.X, maxCoord.Y, Move.Flag);
            }

            return true;
        }
    }
}
