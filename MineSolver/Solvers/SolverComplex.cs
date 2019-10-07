using System;
using System.Collections.Generic;
using MineSolver.Solvers.Complex;

namespace MineSolver.Solvers
{
    public class SolverComplex : SolverSimple
    {
        private readonly ComboLibrary comboLibrary;
        private readonly bool[,] tryLogic;

        public SolverComplex(MineFieldBase field) : base(field)
        {
            comboLibrary = new ComboLibrary();
            tryLogic = new bool[width, height];
        }

        private void Reset()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tryLogic[x, y] = true;
                }
            }
        }

        public override SolveLog Solve()
        {
            Reset();

            SolveLog log = new SolveLog();

            bool progress;

            do
            {
                progress = false;

                log.Combine(base.Solve());

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (tryLogic[x, y] && SolveLogic(x, y, log))
                        {
                            progress = true;
                            x = width;
                            y = height;
                        }
                    }
                }
            }
            while (progress);

            return log;
        }

        private bool SolveLogic(int x, int y, SolveLog log)
        {
            if (solveStatus[x, y] || field[x, y] < 0)
            {
                return false;
            }
         
            var combos = GetValidCombos(x, y);

            if (combos.Count == 0)
            {
                return false;
            }

            var hidden = field.GetHidden(x, y);
            var common = Combo.FindCommon(combos);

            tryLogic[x, y] = false;

            return ApplyCommon(hidden, common, log);
        }

        protected List<Combo> GetValidCombos(int x, int y)
        {
            List<Combo> validCombos = new List<Combo>();

            (int nHidden, int nFlags) = GetCoordInfo(x, y);

            var allCombos = comboLibrary[nHidden, field[x, y] - nFlags];
            var unsolved = GetUnsolved(x, y);
            var hidden = field.GetHidden(x, y);

            foreach(var combo in allCombos)
            {
                bool isValid = true;

                combo.Apply(field, hidden);             

                foreach(var (xUnsolved, yUnsolved) in unsolved)
                {
                    if (IsCoordValid(xUnsolved, yUnsolved) == false)
                    {
                        isValid = false;
                        break;
                    }
                }

                combo.Remove(field, hidden);

                if (isValid)
                {
                    validCombos.Add(combo);
                }
            }

            return validCombos;
        }

        public bool ApplyCommon(List<(int X, int Y)> coords, bool?[] common, SolveLog log)
        {
            if (coords.Count != common.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            bool progress = false;

            for (int i = 0; i < common.Length; i++)
            {
                if(common[i] == true)
                {
                    field.Flag(coords[i].X, coords[i].Y);

                    progress = true;
                    log.AddMove(coords[i].X, coords[i].Y, Move.Flag);

                    foreach ((int x, int y) in GetUnsolved(coords[i].X, coords[i].Y))
                    {
                        tryLogic[x, y] = true;
                    }
                }
                else if(common[i] == false)
                { 
                    field.Reveal(coords[i].X, coords[i].Y);

                    progress = true;
                    log.AddMove(coords[i].X, coords[i].Y, Move.Reveal);

                    tryLogic[coords[i].X, coords[i].Y] = true;

                    foreach((int x, int y) in GetUnsolved(coords[i].X, coords[i].Y))
                    {
                        tryLogic[x, y] = true;
                    }
                }
            }

            return progress;
        }

        private bool IsCoordValid(int x, int y)
        {
            if (field[x, y] < 0)
                return false;

            int nFlags = 0;

            foreach ((int xN, int yN) in field.GetNeighbors(x, y))
            {
                if (field[xN, yN] == -1)
                {
                    nFlags++;
                }
            }

            if (nFlags > field[x, y])
                return false;

            if (nFlags < field[x, y] && GetValidCombos(x, y).Count == 0)
                return false;

            return true;
        }
    }
}
