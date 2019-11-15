using System.Collections.Generic;
using System.Linq;
using Minesolver.Solvers.Advanced;

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
                var oldState = new FieldState(Field);

                progress = false;

                log.Combine(solverBasic.Solve());

                ResetFieldData(oldState);

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

            if (fieldData[x, y].IsSolved || (fieldData[x, y].IsValue == false))
            {
                return false;
            }

            CalcTotals(x, y);

            var hidden = GetHidden(x, y);
            var solved = new List<(int, int)>();

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
            }

            var affected = new HashSet<(int, int)>();

            foreach ((int x2, int y2) in solved)
            {
                ResetCoordData(x2, y2, affected);
            }

            foreach (var coord in hidden)
            {
                fieldData[coord].TotalCombos = 0;
                fieldData[coord].TotalFlagged = 0;
            }       

            return solved.Count > 0;
        }

        private void CalcTotals(int x, int y)
        {
            int nFlagsToComplete = Field[x, y] - fieldData[x, y].NumMines;

            List<(int X, int Y)> hidden = GetHiddenUnused(x, y);

            Combo[] combos = comboLibrary[hidden.Count, nFlagsToComplete];

            HashSet<(int X, int Y)> affected = new HashSet<(int, int)>();

            foreach ((int x2, int y2) in hidden)
            {
                affected.UnionWith(GetValues(x2, y2));
            }

            foreach (Combo combo in combos)
            {
                combo.Apply(Field, fieldData, hidden);

                if (IsComboValid(affected))
                {
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
        }

        protected override bool IsCoordValid(int x, int y)
        {
            int nMines = fieldData[x, y].NumMines;

            if (nMines > Field[x, y])
            {
                return false;
            }

            if (nMines < Field[x, y] && AreValidCombos(x, y) == false)
            {
                return false;
            }

            return true;
        }

        private bool AreValidCombos(int x, int y)
        {
            List<(int X, int Y)> hidden = GetHiddenUnused(x, y);

            if(hidden.Count == 0)
            {
                return false;
            }

            int nFlagsToComplete = Field[x, y] - fieldData[x, y].NumMines;

            if (hidden.Count < nFlagsToComplete)
            {
                return false;
            }

            HashSet<(int X, int Y)> affected = new HashSet<(int X, int Y)>();

            foreach ((int x2, int y2) in hidden)
            {
                affected.UnionWith(GetValues(x2, y2));
            }

            Combo[] combos = comboLibrary[hidden.Count, nFlagsToComplete];

            foreach (Combo combo in combos)
            {
                combo.Apply(Field, fieldData, hidden);

                if (affected.All(coord => IsCoordValid(coord.X, coord.Y)))
                {
                    combo.Remove(Field, fieldData, hidden);
                    return true;
                }

                combo.Remove(Field, fieldData, hidden);
            }

            return false;
        }
    }
}
