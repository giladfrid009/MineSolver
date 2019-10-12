using System;
using System.Linq;
using System.Collections.Generic;
using MineSolver.Solvers.Complex;

namespace MineSolver.Solvers
{

    public class SolverComplex : SolverBase<CoordInfoComplex>
    {
        private readonly SolverSimple solverSimple;
        private readonly ComboLibrary comboLibrary;

        public SolverComplex(IMIneField field) : base(field)
        {
            solverSimple = new SolverSimple(field);
            comboLibrary = new ComboLibrary();
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

            do
            {
                var oldField = field.Copy();

                progress = false;

                log.Combine(solverSimple.Solve());

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (SolveLogic(x, y, log))
                        {
                            progress = true;
                            x = width;
                            y = height;
                        }
                    }
                }

                if (progress == false)
                    break;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (oldField[x, y] != field[x, y])
                        {
                            if (fieldInfo[x, y].IsValue)                                
                            {
                                UpdateAffected(x, y);
                            }
                            else
                            {
                                foreach (var (x2, y2) in fieldInfo[x, y].ValueCoords)
                                {
                                    UpdateAffected(x2, y2);
                                }
                            }
                        }
                    }
                }
            }
            while (true);

            return log;
        }

        private bool SolveLogic(int x, int y, SolveLog log)
        {
            if (fieldInfo[x, y].TryComplex == false)
            {
                return false;
            }

            fieldInfo[x, y].TryComplex = false;

            var combos = GetValidCombos(x, y);            
            var common = GetCommonCombo(combos);
            var hidden = fieldInfo[x, y].HiddenCoords;

            return ApplyCommonCombo(hidden, common, log);
        }

        // todo: doesn't cover all cases somewhy. why?!
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

                var bombs = ApplyCombo(combo, hidden);

                foreach (var bomb in bombs)
                {
                    effected.UnionWith(fieldInfo[bomb].ValueCoords);
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

        public bool ApplyCommonCombo(List<(int X, int Y)> coords, bool?[] common, SolveLog log)
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

            int nFlags = fieldInfo[x, y].NumMines;

            if (nFlags > field[x, y])
                return false;

            if (fieldInfo[x, y].IsSolved)
                return true;

            if (nFlags < field[x, y] && GetValidCombos(x, y).Count == 0)
                return false;

            return true;
        }

        public bool?[] GetCommonCombo(List<Combo> combos)
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

        public List<(int X, int Y)> ApplyCombo(Combo combo, List<(int X, int Y)> coords)
        {
            List<(int, int)> bombs = new List<(int, int)>();

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
                    bombs.Add(coords[i]);
                }
            }

            return bombs;
        }

        public void RemoveCombo(Combo combo, List<(int X, int Y)> coords)
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

        public List<(int X, int Y)> GetHiddenUnused(int x, int y)
        {
            return fieldInfo[x, y].HiddenCoords.Where(coord => fieldInfo[coord].UsedInCombo == false).ToList();
        }

        private void UpdateAffected(int x, int y)
        {
            fieldInfo[x, y].TryComplex = true;

            HashSet<(int, int)> affected = new HashSet<(int, int)>(GetAffected(x, y));

            while (affected.Count > 0)
            {
                HashSet<(int, int)> affectedNew = new HashSet<(int, int)>();

                foreach (var (x2, y2) in affected)
                {
                    fieldInfo[x2, y2].TryComplex = true;
                    affectedNew.UnionWith(GetAffected(x2, y2));
                }

                affected = affectedNew;
            }
        }

        private List<(int X, int Y)> GetAffected(int x, int y)
        {
            return GetUnsolved(x, y).Where(coord => fieldInfo[coord].TryComplex == false).ToList();
        }

        private void GetAffected(IMIneField fieldOld)
        {
            HashSet<(int, int)> affected = new HashSet<(int, int)>();           

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (field[x, y] != fieldOld[x, y])                        
                    {
                        if (fieldInfo[x, y].IsValue)
                        {
                            GetAffectedHelper(x, y, affected);                           
                        }
                        else if (fieldInfo[x, y].IsMine)
                        {
                            foreach (var (x2, y2) in fieldInfo[x, y].ValueCoords)
                            {
                                GetAffectedHelper(x2, y2, affected);
                            }
                        }
                    }
                }
            }
        }      
        
        private void GetAffectedHelper(int x, int y, HashSet<(int, int)> affectedAll)
        {
            HashSet<(int, int)> affectedOld = new HashSet<(int, int)> { (x, y) };

            while (true)
            {
                affectedOld.ExceptWith(affectedAll);

                if (affectedOld.Count == 0)
                    return;

                affectedAll.UnionWith(affectedOld);

                HashSet<(int, int)> affectedNew = new HashSet<(int, int)>();

                foreach (var coord in affectedOld)
                {
                    affectedNew.UnionWith(fieldInfo[coord].ValueCoords);
                }

                affectedOld = affectedNew;
            }
        }
    }
}
