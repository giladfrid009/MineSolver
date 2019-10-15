using System;
using System.Collections.Generic;
using System.Linq;
using Minesolver.Solvers.Utils;
using Minesolver.Solvers.Utils.Advanced;

namespace Minesolver.Solvers
{
    class CoordDataAdvancedPrecent : CoordDataAdvanced
    {
        public int TotalNumFlagged = 0;
        public double PrecentMine = 0;
    }

    class SolverAdvancedPrecent : SolverBase<CoordDataAdvancedPrecent>
    {
        // todo: how to make that it will inherit solverAdvanced, yet will still use coordDataAdvancedPrecent ??

        private readonly SolverAdvanced solverAdvanced;
        private readonly ComboLibrary comboLibrary;

        public SolverAdvancedPrecent(MineFieldBase field, SolverAdvanced solverAdvanced) : base (field)
        {
            this.solverAdvanced = solverAdvanced;
            comboLibrary = new ComboLibrary();
        }

        public override SolveLog Solve()
        {
            log.Clear();

            while (fieldData.IsSolved == false)
            {
                log.Combine(solverAdvanced.Solve());

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (fieldData[x, y].IsSolved || fieldData[x, y].IsValue == false)
                            continue;

                        TryCombosRecursive(x, y);
                    }
                }

                (int X, int Y) minCoord = (-1, -1);
                (int X, int Y) maxCoord = (-1, -1);

                double minPrecent = 1;
                double maxPrecent = 0;

                // calcing precentage
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (fieldData[x, y].IsSolved || fieldData[x, y].IsValue == false)
                            continue;

                        var hidden = GetHidden(x, y);

                        int total = 0;

                        foreach(var coord in hidden)
                        {
                            total += fieldData[coord].TotalNumFlagged;
                        }

                        foreach (var coord in hidden)
                        {
                            fieldData[coord].PrecentMine =  (double)fieldData[coord].TotalNumFlagged / total;

                            if(fieldData[coord].PrecentMine < minPrecent)
                            {
                                minPrecent = fieldData[coord].PrecentMine;
                                minCoord = coord;
                            }
                            else if (fieldData[coord].PrecentMine > maxPrecent)
                            {
                                maxPrecent = fieldData[coord].PrecentMine;
                                maxCoord = coord;
                            }
                        }
                    }
                }

                if(1 - maxPrecent < minPrecent)
                {
                    Console.WriteLine(string.Format("FLAGGED MAX: ({0}, {1}) : {2}%", maxCoord.X, maxCoord.Y, maxPrecent * 100));

                    field.Flag(maxCoord.X, maxCoord.Y);

                }
                else
                {
                    Console.WriteLine(string.Format("REVEALED MIN: ({0}, {1}) : {2}%", minCoord.X, minCoord.Y, minPrecent * 100));

                    field.Reveal(minCoord.X, minCoord.Y);
                }


                ResetFieldData();                
            }

            return log;
        }

        private bool TryCombosRecursive(int x, int y)
        {
            bool foundValid = false;

            int nFlagsToComplete = field[x, y] - fieldData[x, y].NumMines;

            var hidden = GetHiddenUnused(x, y);

            if (hidden.Count < nFlagsToComplete)
            {
                return false;
            }

            var combos = comboLibrary[hidden.Count, nFlagsToComplete];

            var unsolved = GetUnsolved(x, y);

            foreach (var combo in combos)
            {
                var effected = new HashSet<(int, int)>(unsolved);

                var mines = ApplyCombo(combo, hidden);

                foreach (var (x2, y2) in mines)
                {
                    effected.UnionWith(GetValues(x2, y2));
                }

                bool currentValid = true;

                foreach (var (x2, y2) in effected)
                {
                    if (IsCoordValid(x2, y2) == false)
                    {
                        currentValid = false;
                        break;
                    }
                }

                RemoveCombo(combo, hidden);

                if (currentValid)
                {
                    foundValid = true;
                    UpdateFieldData(combo, hidden);
                }
            }

            return foundValid;
        }

        private bool IsCoordValid(int x, int y)
        {
            if (fieldData[x, y].IsValue == false)
                return false;

            int nMines = fieldData[x, y].NumMines;

            if (nMines > field[x, y])
                return false;

            if (fieldData[x, y].IsSolved)
                return true;

            if (nMines < field[x, y] && TryCombosRecursive(x, y) == false)
                return false;

            return true;
        }

        private List<(int X, int Y)> GetHiddenUnused(int x, int y)
        {
            return GetHidden(x, y).Where(coord => fieldData[coord].UsedInCombo == false).ToList();
        }

        private List<(int X, int Y)> ApplyCombo(Combo combo, List<(int X, int Y)> coords)
        {
            List<(int, int)> miness = new List<(int, int)>();

            if (coords.Count != combo.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < combo.Length; i++)
            {
                int x = coords[i].X;
                int y = coords[i].Y;

                fieldData[x, y].UsedInCombo = true;

                if (combo[i])
                {
                    field.Flag(x, y);
                    miness.Add(coords[i]);
                }
            }

            return miness;
        }

        private void RemoveCombo(Combo combo, List<(int X, int Y)> coords)
        {
            if (coords.Count != combo.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < combo.Length; i++)
            {
                int x = coords[i].X;
                int y = coords[i].Y;

                fieldData[x, y].UsedInCombo = false;

                if (combo[i])
                {
                    field.Unflag(x, y);
                }
            }
        }

        private void UpdateFieldData(Combo combo, List<(int X, int Y)> coords)
        {
            if (coords.Count != combo.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < combo.Length; i++)
            {
                int x = coords[i].X;
                int y = coords[i].Y;

                if (combo[i])
                {
                    fieldData[x, y].TotalNumFlagged++;
                }
            }
        }

        private void ResetFieldData()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldData[x, y].TryAdvanced = true;
                    fieldData[x, y].TotalNumFlagged = 0;
                    fieldData[x, y].PrecentMine = 0;
                }
            }
        }

    }
}
