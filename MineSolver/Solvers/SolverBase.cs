﻿using System;
using System.Linq;
using System.Collections.Generic;
using Minesolver.Solvers.Utils;

namespace Minesolver.Solvers
{
    public abstract class SolverBase<TCoordData> where TCoordData : CoordData, new()
    {
        public MineFieldBase Field { get; }

        protected readonly FieldData<TCoordData> fieldData;
        protected readonly SolveLog log;
        protected readonly int width;
        protected readonly int height;

        public SolverBase(MineFieldBase field)
        {
            fieldData = new FieldData<TCoordData>(field);
            Field = field;
            log = new SolveLog();
            width = field.Width;
            height = field.Height;
        }

        public abstract SolveLog Solve();

        protected List<(int X, int Y)> RevealHidden(int x, int y)
        {
            var affected = new List<(int, int)>();

            var hidden = GetHidden(x, y);

            foreach (var (x2, y2) in hidden)
            {
                Field.Reveal(x2, y2);
                log.AddMove(x2, y2, Move.Reveal);
            }

            foreach (var (x2, y2) in hidden)
            {
                if (Field[x2, y2] == 0)
                {
                    var opened = GetAreaBounds(x2, y2);

                    affected.AddRange(opened);

                    foreach (var (x3, y3) in opened)
                    {
                        affected.AddRange(GetUnsolved(x3, y3));
                    }
                }
                else
                {
                    affected.Add((x2, y2));
                    affected.AddRange(GetUnsolved(x2, y2));
                }
            }

            return affected;
        }

        protected List<(int X, int Y)> FlagHidden(int x, int y)
        {
            var affected = new List<(int, int)>();

            var hidden = GetHidden(x, y);

            foreach (var (x2, y2) in hidden)
            {
                Field.Flag(x2, y2);
                log.AddMove(x2, y2, Move.Flag);
            }

            foreach (var (x2, y2) in hidden)
            {
                affected.AddRange(GetUnsolved(x2, y2));
            }

            return affected;
        }

        protected List<(int X, int Y)> GetHidden(int x, int y)
        {
            return fieldData[x, y].Neighbors.Where(coord => fieldData[coord].IsRevealed == false).ToList();
        }

        protected List<(int X, int Y)> GetValues(int x, int y)
        {
            return fieldData[x, y].Neighbors.Where(coord => fieldData[coord].IsValue).ToList();
        }

        protected List<(int X, int Y)> GetUnsolved(int x, int y)
        {
            return GetValues(x, y).Where(coord => fieldData[coord].IsSolved == false).ToList();
        }

        protected List<(int X, int Y)> GetAreaBounds(int x, int y)
        {           
            var coords = new HashSet<(int X, int Y)>();

            if (Field[x, y] != 0)
                throw new Exception();

            GetOpenedAreaRecursive(x, y, coords);

            return coords.Where(coord => Field[coord] != 0).ToList();
        }

        private void GetOpenedAreaRecursive(int x, int y, HashSet<(int, int)> coords)
        {
            if (coords.Contains((x, y)))
                return;

            coords.Add((x, y));

            if (Field[x, y] == 0)
            {
                foreach (var (x2, y2) in fieldData[x, y].Neighbors)
                {
                    GetOpenedAreaRecursive(x2, y2, coords);
                }
            }
        }

        protected virtual void Reset()
        {
            log.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldData[x, y].IsSolved = false;
                }
            }
        }
    }
}
