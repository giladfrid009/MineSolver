using System.Collections.Generic;
using System.Linq;
using Minesolver.Solvers.Advanced;

namespace Minesolver.Solvers
{
    internal class SolverAdvanced : SolverAdvancedBase
    {
        private readonly SolverBasic solverBasic;

        public SolverAdvanced(MineFieldBase field) : base(field)
        {
            solverBasic = new SolverBasic(field);
        }

        public override SolveLog Solve()
        {
            Reset();

            bool progress;

            while (true)
            {
                MineFieldBase oldField = Field.Clone();

                progress = false;

                log.Combine(solverBasic.Solve());

                UpdateField(oldField);

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

                UpdateField(oldField);
            }

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

            if (fieldData[x, y].IsSolved || (fieldData[x, y].IsValue == false) || HasLost)
            {
                return result;
            }

            CalcTotals(x, y);

            List<(int X, int Y)> hidden = GetHidden(x, y);


            foreach ((int x2, int y2) in hidden)
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

            //HashSet<(int, int)> affected = new HashSet<(int, int)>();

            //foreach ((int x2, int y2) in hidden)
            //{
            //    if (fieldData[x2, y2].TotalFlagged == 0)
            //    {
            //        GetConnectedArea(x2, y2, affected);
            //    }
            //    else if (fieldData[x2, y2].TotalFlagged == fieldData[x2, y2].TotalCombos)
            //    {
            //        foreach (var (x3, y3) in GetUnsolved(x2, y2))
            //        {
            //            GetConnectedArea(x3, y3, affected);
            //        }
            //    }
            //}

            //foreach (var (x2, y2) in affected)
            //{
            //    ResetCoordData(x2, y2);
            //}

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

            List<(int X, int Y)> hidden = GetHiddenUnused(x, y);

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

        private bool IsCoordValid(int x, int y)
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
            bool result = false;

            int nFlagsToComplete = Field[x, y] - fieldData[x, y].NumMines;

            List<(int X, int Y)> hidden = GetHiddenUnused(x, y);

            if (hidden.Count < nFlagsToComplete)
            {
                return result;
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
                    result = true;

                    combo.Remove(Field, fieldData, hidden);

                    break;
                }

                combo.Remove(Field, fieldData, hidden);
            }

            return result;
        }
    }
}
