using System;
using System.Linq;
using System.Collections.Generic;
using MineSolver.Solvers.Utils.Complex;
using MineSolver.Solvers.Utils;

namespace MineSolver.Solvers
{

    public class Complex : SolverBase<CoordInfoComplex>
    {
        private readonly SolverBase<CoordInfo> solverSimple;
        private readonly ComboLib comboLibrary;

        // todo: maybe make an interface for a secondary solver which depends on another solver?
        public Complex(MineFieldBase field, SolverBase<CoordInfo> solverSimple) : base(field)
        {
            this.solverSimple = solverSimple;
            comboLibrary = new ComboLib();
        }

        private void Reset()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldInfo[x, y].TryComplex = true;
                }
            }
        }

        public override SolveLog Solve()
        {
            Reset();

            SolveLog log = new SolveLog();

            bool progress;

            while(true)
            {
                var oldField = field.Copy();

                progress = false;

                log.Combine(solverSimple.Solve());

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (SolveCoord(x, y, log))
                        {
                            progress = true;
                            x = width;
                            y = height;
                        }
                    }
                }

                if (progress == false)
                    break;

                UpdateFieldInfo(oldField);
            }

            return log;
        }

        private bool SolveCoord(int x, int y, SolveLog log)
        {
            if (fieldInfo[x, y].TryComplex == false)
            {
                return false;
            }

            fieldInfo[x, y].TryComplex = false;

            if (fieldInfo[x, y].IsSolved || (fieldInfo[x, y].IsValue == false))
            {
                return false;
            }

            var combos = GetValidCombos(x, y);            
            var common = GetCommonCombo(combos);
            var hidden = fieldInfo[x, y].GetHidden();

            return ApplyCommonCombo(hidden, common, log);
        }

        private List<Combo> GetValidCombos(int x, int y)
        {
            List<Combo> valid = new List<Combo>();

            int nFlagsToComplete = field[x, y] - fieldInfo[x, y].NumMines;

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

                foreach (var mine in mines)
                {
                    effected.UnionWith(fieldInfo[mine].GetValues());
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

        private bool ApplyCommonCombo(List<(int X, int Y)> coords, bool?[] common, SolveLog log)
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
            if (fieldInfo[x, y].IsValue == false)
                return false;

            int nMines = fieldInfo[x, y].NumMines;

            if (nMines > field[x, y])
                return false;

            if (fieldInfo[x, y].IsSolved)
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

                fieldInfo[x, y].UsedInCombo = true;

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

                fieldInfo[x, y].UsedInCombo = false;

                if (combo[i])
                {
                    field.Unflag(x, y);
                }
            }
        }

        private List<(int X, int Y)> GetHiddenUnused(int x, int y)
        {
            return fieldInfo[x, y].GetHidden().Where(coord => fieldInfo[coord].UsedInCombo == false).ToList();
        }

        private void UpdateFieldInfo(MineFieldBase oldField)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (oldField[x, y] != field[x, y])
                    {
                        if (fieldInfo[x, y].IsValue)
                        {
                            EnableTryComplex(x, y);
                        }
                        else
                        {
                            foreach (var (x2, y2) in GetUnsolved(x, y))
                            {
                                EnableTryComplex(x2, y2);
                            }
                        }
                    }
                }
            }

        }

        private void EnableTryComplex(int x, int y)
        {
            if (fieldInfo[x, y].TryComplex)
                return;

            fieldInfo[x, y].TryComplex = true;

            foreach (var (x2, y2) in GetUnsolved(x, y))
            {
                EnableTryComplex(x2, y2);
            }
        }
    }
}
