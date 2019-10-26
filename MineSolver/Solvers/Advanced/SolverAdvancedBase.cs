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

        protected void UpdateFieldData(MineFieldBase oldField)
        {
            HashSet<(int, int)> affected = new HashSet<(int, int)>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (oldField[x, y] != Field[x, y])
                    {
                        UpdateCoordData(x, y, affected);
                    }
                }
            }
        }

        protected void UpdateCoordData(int x, int y, HashSet<(int, int)> affected)
        {
            if (fieldData[x, y].IsValue)
            {
                UpdateCoordRecursive(x, y, affected);
            }
            else
            {
                foreach ((int x2, int y2) in GetUnsolved(x, y))
                {
                    UpdateCoordRecursive(x2, y2, affected);
                }
            }
        }

        private void UpdateCoordRecursive(int x, int y, HashSet<(int X, int Y)> coords)
        {
            if (coords.Contains((x, y)))
            {
                return;
            }

            coords.Add((x, y));

            fieldData[x, y].TryAdvanced = true;
            fieldData[x, y].TotalFlagged = 0;
            fieldData[x, y].TotalCombos = 0;

            foreach ((int x2, int y2) in GetUnsolved(x, y))
            {
                UpdateCoordRecursive(x2, y2, coords);
            }

            foreach ((int x2, int y2) in GetHidden(x, y))
            {
                if (coords.Contains((x2, y2)))
                    continue;

                coords.Add((x2, y2));

                foreach ((int x3, int y3) in GetUnsolved(x2, y2))
                {
                    UpdateCoordRecursive(x3, y3, coords);
                }
            }
        }

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
