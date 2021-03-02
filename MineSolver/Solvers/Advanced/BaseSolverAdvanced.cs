﻿using System.Collections.Generic;
using System.Linq;

namespace Minesolver.Solvers.Advanced
{
    public abstract class BaseSolverAdvanced : BaseSolver<FieldAdvanced, CoordAdvanced>
    {
        protected readonly ComboLibrary comboLibrary;

        public uint MaxDepth { get; set; } = 10;

        public BaseSolverAdvanced(BaseField field) : base(field)
        {
            comboLibrary = new ComboLibrary();
        }

        protected List<(int X, int Y)> GetNotForced(List<(int X, int Y)> coords)
        {
            return coords.Where(coord => fieldData[coord].IsForced == false).ToList();
        }

        protected void ResetFieldChanges(FieldState oldState)
        {
            HashSet<(int, int)> processed = new HashSet<(int, int)>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (oldState[x, y] != Field[x, y])
                    {
                        ResetCoordData(x, y, processed);
                    }
                }
            }
        }

        protected void ResetCoordData(int x, int y, HashSet<(int, int)> processed)
        {
            if (fieldData.IsValue(x, y) && fieldData.IsSolved(x, y) == false)
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

            fieldData[x, y].Reset();

            foreach ((int x2, int y2) in GetUnsolved(x, y))
            {
                ResetCoordRecursive(x2, y2, processed);
            }

            foreach ((int x2, int y2) in GetHidden(x, y))
            {
                if (processed.Contains((x2, y2)))
                {
                    continue;
                }

                processed.Add((x2, y2));

                foreach ((int x3, int y3) in GetUnsolved(x2, y2))
                {
                    ResetCoordRecursive(x3, y3, processed);
                }
            }
        }

        protected bool IsComboValid(HashSet<(int X, int Y)> affected, uint depth)
        {
            foreach ((int x, int y) in affected)
            {
                if (IsCoordValid(x, y, depth) == false)
                {
                    return false;
                }
            }

            return true;
        }

        protected abstract bool IsCoordValid(int x, int y, uint depth);

        protected override void Reset()
        {
            base.Reset();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldData[x, y].Reset();
                }
            }
        }
    }
}