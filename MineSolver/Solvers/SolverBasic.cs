﻿using System.Collections.Generic;
using Minesolver.Solvers.Utils;

namespace Minesolver.Solvers
{
    public class SolverBasic : SolverBase<CoordData>
    {
        public SolverBasic(MineFieldBase field) : base(field)
        {

        }

        public override SolveLog Solve()
        {
            Reset();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pending = new List<(int, int)> { (x, y) };

                    while (pending.Count > 0)
                    {
                        var pendingNew = new List<(int, int)>();

                        foreach (var (x2, y2) in pending)
                        {
                            pendingNew.AddRange(SolveCoord(x2, y2));
                        }

                        pending = pendingNew;
                    }
                }
            }

            return log.Clone();
        }

        private List<(int X, int Y)> SolveCoord(int x, int y)
        {
            if (fieldData[x, y].IsSolved || (fieldData[x, y].IsValue == false))
            {
                return new List<(int, int)>();
            }

            int nHidden = fieldData[x, y].NumHidden;
            int nMines = fieldData[x, y].NumMines;

            if (Field[x, y] == nMines)
            {
                return RevealHidden(x, y);
            }
            else if (nHidden == Field[x, y] - nMines)
            {
                return FlagHidden(x, y);               
            }

            return new List<(int, int)>();
        }
    }
}