using System;
using System.Linq;
using System.Collections.Generic;
using Minesolver.Solvers.Utils.Advanced;
using Minesolver.Solvers.Utils;

namespace Minesolver.Solvers
{
    public class SolverAdvanced : SolverBase<CoordDataAdvanced>
    {
        private readonly SolverBase<CoordData> solverBasic;
        private readonly ComboLibrary comboLibrary;

        public SolverAdvanced(MineFieldBase field, SolverBase<CoordData> solverBasic) : base(field)
        {
            this.solverBasic = solverBasic;
            comboLibrary = new ComboLibrary();
        }

        private void ResetFieldData()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldData[x, y].TryAdvanced = true;
                }
            }
        }

        public override SolveLog Solve()
        {
            log.Clear();

            ResetFieldData();

            bool progress;

            while(true)
            {
                var oldField = field.Clone();

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

                UpdateFieldData(oldField);
            }

            return log;
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

            var combos = GetValidCombos(x, y);            
            var common = GetCommonCombo(combos);
            var hidden = GetHidden(x, y);

            return ApplyCommonCombo(hidden, common);
        }

        private List<Combo> GetValidCombos(int x, int y)
        {
            List<Combo> valid = new List<Combo>();

            int nFlagsToComplete = field[x, y] - fieldData[x, y].NumMines;

            var hidden = GetHiddenUnused(x, y);

            if (hidden.Count < nFlagsToComplete)
            {
                return valid;
            }

            var combos = comboLibrary[hidden.Count, nFlagsToComplete];

            var unsolved = GetUnsolved(x, y);

            foreach(var combo in combos)
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
                    valid.Add(combo);
                }
            }

            return valid;
        }

        private bool ApplyCommonCombo(List<(int X, int Y)> coords, bool?[] common)
        {
            if (coords.Count != common.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            bool progress = false;

            for (int i = 0; i < common.Length; i++)
            {
                if (common[i] == null)
                    continue;

                int x = coords[i].X;
                int y = coords[i].Y;

                if(common[i] == true)
                {
                    field.Flag(x, y);

                    log.AddMove(x, y, Move.Flag);             
                }
                else
                { 
                    field.Reveal(x, y);

                    log.AddMove(x, y, Move.Reveal);
                }

                progress = true;
            }

            return progress;
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

            if (nMines < field[x, y] && GetValidCombos(x, y).Count == 0)
                return false;

            return true;
        }

        private bool?[] GetCommonCombo(List<Combo> combos)
        {
            var firstCombo = combos[0];
            var length = firstCombo.Length;

            bool?[] common = new bool?[length];

            for (int i = 0; i < length; i++)
            {
                common[i] = firstCombo[i];

                foreach (var combo in combos)
                {
                    if (combo[i] != common[i])
                    {
                        common[i] = null;
                        break;
                    }
                }
            }

            return common;
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

        private List<(int X, int Y)> GetHiddenUnused(int x, int y)
        {
            return GetHidden(x, y).Where(coord => fieldData[coord].UsedInCombo == false).ToList();
        }

        private void UpdateFieldData(MineFieldBase oldField)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (oldField[x, y] != field[x, y])
                    {
                        if (fieldData[x, y].IsValue)
                        {
                            UpdateCoordData(x, y);
                        }
                        else
                        {
                            foreach (var (x2, y2) in GetUnsolved(x, y))
                            {
                                UpdateCoordData(x2, y2);
                            }
                        }
                    }
                }
            }

        }

        private void UpdateCoordData(int x, int y)
        {
            if (fieldData[x, y].TryAdvanced)
                return;

            fieldData[x, y].TryAdvanced = true;

            foreach (var (x2, y2) in GetUnsolved(x, y))
            {
                UpdateCoordData(x2, y2);
            }
        }
    }
}
