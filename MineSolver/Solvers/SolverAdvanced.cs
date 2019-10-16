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

        protected override void Reset()
        {
            log.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldData[x, y].IsSolved = false;
                    fieldData[x, y].TryAdvanced = true;
                }
            }
        }

        public override SolveLog Solve()
        {
            Reset();

            bool progress;

            while(true)
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

            var (common, result) = Combo.GetCommonState(combos);

            if (result == false)
                return false;

            var hidden = GetHidden(x, y);

            ApplyCommonState(common, hidden);

            return true;
        }

        private List<Combo> GetValidCombos(int x, int y)
        {
            List<Combo> valid = new List<Combo>();

            int nFlagsToComplete = Field[x, y] - fieldData[x, y].NumMines;

            var hidden = GetHiddenUnused(x, y);

            if (hidden.Count < nFlagsToComplete)
            {
                return valid;
            }

            var combos = comboLibrary[hidden.Count, nFlagsToComplete];

            foreach(var combo in combos)
            {
                var effected = new HashSet<(int X, int Y)>();

                combo.Apply(Field, fieldData, hidden);

                foreach (var (x2, y2) in hidden)
                {
                    effected.UnionWith(GetValues(x2, y2));
                }

                if (effected.All(coord => IsCoordValid(coord.X, coord.Y)))
                {
                    valid.Add(combo);
                }

                combo.Remove(Field, fieldData, hidden);           
            }

            return valid;
        }

        private List<(int X, int Y)> GetHiddenUnused(int x, int y)
        {
            return GetHidden(x, y).Where(coord => fieldData[coord].UsedInCombo == false).ToList();
        }

        private bool IsCoordValid(int x, int y)
        {
            int nMines = fieldData[x, y].NumMines;

            if (nMines > Field[x, y])
            {
                return false;
            }
            else if (nMines < Field[x, y] && GetValidCombos(x, y).Count == 0)
            {
                return false;
            }

            return true;
        }

        private void ApplyCommonState(bool?[] commonState, List<(int X, int Y)> coords)
        {
            if (coords.Count != commonState.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < commonState.Length; i++)
            {
                if (commonState[i] == null)
                    continue;

                var (x, y) = coords[i];

                if(commonState[i] == true)
                {
                    Field.Flag(x, y);
                    log.AddMove(x, y, Move.Flag);             
                }
                else
                { 
                    Field.Reveal(x, y);
                    log.AddMove(x, y, Move.Reveal);
                }
            }
        }

        private void UpdateFieldData(MineFieldBase oldField)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (oldField[x, y] != Field[x, y])
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
