using System;
using System.Collections.Generic;
using Minefield;

namespace Minesolver
{
    public class Solver
    {
        private readonly Field field;
        private readonly FieldData fieldData;

        public int MaxDepth { get; set; } = 15;

        public Solver(Field field)
        {
            this.field = field;
            fieldData = new FieldData(field);
        }

        public void Solve()
        {
            while (field.Game.Result == GameResult.None)
            {
                for (int row = 0; row < field.Height; row++)
                {
                    for (int col = 0; col < field.Width; col++)
                    {
                        SolveFrom(fieldData[row, col]);
                    }
                }

                if (field.Game.Result == GameResult.Won) return;

                BestGuess();      
            }
        }

        private void SolveFrom(Coord origin)
        {
            if (origin.Value < 1) return;

            HashSet<Coord> oldAffected = new() { origin };
            HashSet<Coord> newAffected = new();

            field.OnMove += UpdateAffected;

            while (true)
            {
                foreach (Coord C in oldAffected)
                {
                    if (C.Value < 1) continue;

                    if (C.NumFlags == C.Value)
                    {
                        foreach (Coord nCoord in C.Adjacent)
                        {
                            if (nCoord.Value != Field.Hidden) continue;

                            field.Reveal(nCoord.Row, nCoord.Col);
                        }
                    }
                    else if (C.NumHidden == C.Value - C.NumFlags)
                    {
                        foreach (Coord nCoord in C.Adjacent)
                        {
                            if (nCoord.Value != Field.Hidden) continue;

                            field.Flag(nCoord.Row, nCoord.Col);
                        }
                    }
                }

                if (newAffected.Count == 0)
                {
                    field.OnMove -= UpdateAffected;
                    return;
                }

                Swap(ref oldAffected, ref newAffected);

                newAffected.Clear();
            }

            void UpdateAffected(Field sender, MoveArgs e)
            {
                Coord C = fieldData[e.Row, e.Col];

                if (e.Move == Move.Reveal)
                {
                    newAffected.Add(C);
                    newAffected.UnionWith(C.Adjacent);
                }

                if (e.Move == Move.Flag)
                {
                    newAffected.UnionWith(C.Adjacent);
                }
            }
        }

        private void BestGuess()
        {
            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    fieldData[row, col].Combo.Reset();
                }
            }

            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    Coord coord = fieldData[row, col];

                    GenCombo(coord, 0);

                    if (coord.Combo.MinePrec == 1)
                    {
                        field.Flag(row, col);
                        return;
                    }

                    if (coord.Combo.ValuePrec == 1)
                    {
                        field.Reveal(row, col);
                        return;
                    }
                }
            }

            double maxMine = 0;
            double maxVal = 0;

            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    if (fieldData[row, col].Value != Field.Hidden) continue;

                    Combo combo = fieldData[row, col].Combo;

                    maxMine = Math.Max(maxMine, combo.MinePrec);
                    maxVal = Math.Max(maxVal, combo.ValuePrec);
                }
            }

            double max = Math.Max(maxMine, maxVal);

            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    if (fieldData[row, col].Value != Field.Hidden) continue;

                    Combo C = fieldData[row, col].Combo;                   

                    if (C.MinePrec >= max)
                    {
                        field.Flag(row, col);
                        return;
                    }

                    if (C.ValuePrec >= max)
                    {
                        field.Reveal(row, col);
                        return;
                    }
                }
            }
        }

        private bool GenCombo(Coord origin, int depth)
        {
            if (origin.Value < 1) return true;

            if (origin.IsValid() == false) return false;

            foreach(Coord C in origin.Adjacent)
            {
                if (C.Value == 0) continue;

                if (C.IsValid() == false) return false;
            }

            if (origin.NumHidden == 0) return true;

            if (depth > MaxDepth) return true;

            HashSet<Coord> affected = new();

            foreach(var C in origin.Adjacent)
            {
                if (C.Value != Field.Hidden) continue;

                affected.UnionWith(C.Adjacent);
            }

            affected.ExceptWith(origin.Adjacent);

            (var mineCombos, var valCombos) = ComboLib.GetCombos(origin);

            bool hasValid = false;

            for (int iCombo = 0; iCombo < mineCombos.Count; iCombo++)
            {
                bool isValid = true;

                ApplyCombo(origin.Adjacent, mineCombos[iCombo], valCombos[iCombo]);

                foreach(Coord C in affected)
                {
                    isValid &= GenCombo(C, depth + 1);
                }

                if (isValid)
                {
                    hasValid = true;

                    for (int j = 0; j < origin.NumAdj; j++)
                    {
                        if (mineCombos[iCombo][j]) origin.Adjacent[j].Combo.NumFlagged++;

                        if (valCombos[iCombo][j]) origin.Adjacent[j].Combo.NumOpened++;
                    }
                }

                RemoveCombo(origin.Adjacent, mineCombos[iCombo], valCombos[iCombo]);
            }

            return hasValid;            
        }

        private void Swap<T>(ref T first, ref T second)
        {
            T tmp = first;

            first = second;

            second = tmp;
        }

        private void ApplyCombo(Coord[] adjCoords, bool[] mineCombo, bool[] valCombo)
        {
            if (adjCoords.Length != mineCombo.Length) throw new ArgumentOutOfRangeException(nameof(mineCombo));

            if (adjCoords.Length != valCombo.Length) throw new ArgumentOutOfRangeException(nameof(valCombo));

            for (int i = 0; i < adjCoords.Length; i++)
            {
                if (mineCombo[i]) adjCoords[i].Value = Field.Mine;

                else if (valCombo[i]) adjCoords[i].Value = 0;
            }
        }

        private void RemoveCombo(Coord[] adjCoords, bool[] mineCombo, bool[] valCombo)
        {
            if (adjCoords.Length != mineCombo.Length) throw new ArgumentOutOfRangeException(nameof(mineCombo));

            if (adjCoords.Length != valCombo.Length) throw new ArgumentOutOfRangeException(nameof(valCombo));

            for (int i = 0; i < adjCoords.Length; i++)
            {
                if (mineCombo[i]) adjCoords[i].Value = Field.Hidden;

                else if (valCombo[i]) adjCoords[i].Value = Field.Hidden;
            }
        }
    }
}
