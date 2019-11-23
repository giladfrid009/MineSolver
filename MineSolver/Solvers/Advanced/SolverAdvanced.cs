using Minesolver.Solvers.Advanced;
using System.Collections.Generic;

namespace Minesolver.Solvers
{
    internal class SolverAdvanced : SolverAdvancedBase
    {
        private readonly SolverBasic solverBasic;

        public SolverAdvanced(FieldBase field) : base(field)
        {
            solverBasic = new SolverBasic(field);
        }

        public override SolveLog Solve()
        {
            Reset();

            bool progress;

            while (true)
            {
                FieldState oldState = new FieldState(Field);

                progress = false;

                log.Combine(solverBasic.Solve());

                if (HasLost)
                {
                    return log.Clone();
                }

                ResetFieldChanges(oldState);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (SolveCoord(x, y))
                        {
                            if (HasLost)
                            {
                                return log.Clone();
                            }

                            progress = true;
                            x = width;
                            y = height;
                        }
                    }
                }

                if (progress == false)
                {
                    break;
                }
            }

            return log.Clone();
        }

        private bool SolveCoord(int x, int y)
        {
            if (fieldData[x, y].TryAdvanced == false)
            {
                return false;
            }

            fieldData[x, y].TryAdvanced = false;

            if (fieldData.IsSolved(x, y) || (fieldData.IsValue(x, y) == false))
            {
                return false;
            }

            CalcTotals(x, y);

            List<(int X, int Y)> hidden = GetHidden(x, y);
            List<(int, int)> solved = new List<(int, int)>();

            foreach ((int x2, int y2) in hidden)
            {
                if (fieldData[x2, y2].TotalFlagged == 0)
                {
                    Field.Reveal(x2, y2);
                    log.AddMove(x2, y2, Move.Reveal);
                    solved.Add((x2, y2));
                }
                else if (fieldData[x2, y2].TotalFlagged == fieldData[x2, y2].TotalCombos)
                {
                    Field.Flag(x2, y2);
                    log.AddMove(x2, y2, Move.Flag);
                    solved.Add((x2, y2));
                }

                if (HasLost)
                {
                    return false;
                }
            }       

            HashSet<(int, int)> affected = new HashSet<(int, int)>();

            foreach ((int x2, int y2) in solved)
            {
                ResetCoordData(x2, y2, affected);
            }

            foreach ((int X, int Y) coord in hidden)
            {
                fieldData[coord].Reset();
            }

            return solved.Count > 0;
        }

        private void CalcTotals(int x, int y)
        {
            int nFlagsMissing = Field[x, y] - fieldData.NumMines(x, y);

            List<(int X, int Y)> hidden = GetHidden(x, y);

            Combo[] combos = comboLibrary[hidden.Count, nFlagsMissing];

            HashSet<(int X, int Y)> affected = new HashSet<(int, int)>();

            foreach ((int x2, int y2) in hidden)
            {
                affected.UnionWith(GetValues(x2, y2));
            }

            foreach (Combo combo in combos)
            {
                combo.Apply(fieldData, hidden);

                if (IsComboValid(affected, 1))
                {
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
        }

        protected override bool IsCoordValid(int x, int y, uint depth)
        {
            int nMines = fieldData.NumMines(x, y);

            if (nMines > Field[x, y])
            {
                return false;
            }

            if (nMines < Field[x, y] && AreValidCombos(x, y, depth) == false)
            {
                return false;
            }

            return true;
        }

        private bool AreValidCombos(int x, int y, uint depth)
        {
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

            HashSet<(int X, int Y)> affected = new HashSet<(int X, int Y)>();

            foreach ((int x2, int y2) in hidden)
            {
                affected.UnionWith(GetValues(x2, y2));
            }

            Combo[] combos = comboLibrary[hidden.Count, nFlagsMissing];

            foreach (Combo combo in combos)
            {
                combo.Apply(fieldData, hidden);

                if (IsComboValid(affected, depth + 1))
                {
                    combo.Remove(fieldData, hidden);
                    return true;
                }

                combo.Remove(fieldData, hidden);
            }

            return false;
        }
    }
}
