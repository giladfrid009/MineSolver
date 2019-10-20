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

        protected void UpdateField(MineFieldBase oldField)
        {
            HashSet<(int, int)> affected = new HashSet<(int, int)>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (oldField[x, y] != Field[x, y])
                    {
                        UpdateCoord(x, y, affected);
                    }
                }
            }
        }

        protected void UpdateCoord(int x, int y, HashSet<(int, int)> affected)
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
        }

        protected override void Reset()
        {
            log.Clear();

            // todo: potential problem of continuing solving after resseting hasLost to false.
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
