using System.Collections.Generic;
using System.Linq;

namespace Minesolver.Solvers.Advanced
{
    public abstract class SolverAdvancedBase : SolverBase<CoordDataAdvanced>
    {
        protected readonly ComboLibrary comboLibrary;

        public SolverAdvancedBase(MineFieldBase field) : base(field)
        {
            comboLibrary = new ComboLibrary();
        }

        protected List<(int X, int Y)> GetHiddenUnused(int x, int y)
        {
            return GetHidden(x, y).Where(coord => fieldData[coord].UsedInCombo == false).ToList();
        }

        protected void ResetFieldData(MineFieldBase oldField)
        {
            HashSet<(int, int)> processed = new HashSet<(int, int)>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (oldField[x, y] != Field[x, y])
                    {
                        ResetCoordData(x, y, processed);
                    }
                }
            }
        }

        protected void ResetCoordData(int x, int y, HashSet<(int, int)> processed)
        {
            if (fieldData[x, y].IsValue && fieldData[x, y].IsSolved == false)
            {
                ResetCoordRecursive(x, y, processed);
            }
            else
            {
                foreach ((int x2, int y2) in GetUnsolved(x, y))
                {
                    ResetCoordRecursive(x2, y2, processed);
                }
            }
        }

        private void ResetCoordRecursive(int x, int y, HashSet<(int X, int Y)> processed)
        {
            if (processed.Contains((x, y)))
            {
                return;
            }

            processed.Add((x, y));

            fieldData[x, y].TryAdvanced = true;
            fieldData[x, y].TotalFlagged = 0;
            fieldData[x, y].TotalCombos = 0;

            foreach ((int x2, int y2) in GetUnsolved(x, y))
            {
                ResetCoordRecursive(x2, y2, processed);
            }

            foreach ((int x2, int y2) in GetHidden(x, y))
            {
                if (processed.Contains((x2, y2)))
                    continue;

                processed.Add((x2, y2));

                foreach ((int x3, int y3) in GetUnsolved(x2, y2))
                {
                    ResetCoordRecursive(x3, y3, processed);
                }
            }
        }

        protected bool IsComboValid(HashSet<(int X, int Y)> affected)
        {
            foreach (var (x, y) in affected)
            {
                if (IsCoordValid(x, y) == false)
                    return false;
            }

            return true;
        }

        protected abstract bool IsCoordValid(int x, int y);

        protected override void Reset()
        {
            log.Clear();

            HasLost = false;

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
    }
}
