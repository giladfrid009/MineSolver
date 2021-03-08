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
                SolveSimple();

                if (field.Game.Result != GameResult.None) return;

                bool madeMove = GenCombos();

                if (field.Game.Result != GameResult.None) return;

                if (madeMove) continue;

                BestGuess();
            }
        }

        private void SolveSimple()
        {
            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    SolveSimple(fieldData[row, col]);
                }
            }
        }

        private void SolveSimple(Coord origin)
        {
            if (origin.Value < 1) return;

            HashSet<Coord> oldAffected = new() { origin };
            HashSet<Coord> newAffected = new() { origin };

            field.OnMove += UpdateAffected;

            while (newAffected.Count != 0 && field.Game.Result == GameResult.None)
            {
                foreach (Coord coord in oldAffected)
                {
                    if (coord.Value < 1) continue;

                    if (coord.NumFlags == coord.Value)
                    {
                        foreach (Coord nCoord in coord.Adjacent)
                        {
                            if (nCoord.Value != Field.Hidden) continue;

                            field.Reveal(nCoord.Row, nCoord.Col);
                        }
                    }
                    else if (coord.NumHidden == coord.Value - coord.NumFlags)
                    {
                        foreach (Coord nCoord in coord.Adjacent)
                        {
                            if (nCoord.Value != Field.Hidden) continue;

                            field.Flag(nCoord.Row, nCoord.Col);
                        }
                    }
                }

                Swap(ref oldAffected, ref newAffected);

                newAffected.Clear();
            }

            field.OnMove -= UpdateAffected;            

            void UpdateAffected(Field sender, MoveArgs e)
            {
                Coord coord = fieldData[e.Row, e.Col];

                if (e.Move == Move.Reveal)
                {
                    newAffected.Add(coord);
                    newAffected.UnionWith(coord.Adjacent);
                }

                if (e.Move == Move.Flag)
                {
                    newAffected.UnionWith(coord.Adjacent);
                }
            }
        }

        private bool GenCombos()
        {
            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    fieldData[row, col].Stats.Reset();
                }
            }

            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    if (fieldData[row, col].Value < 1) continue;

                    GenCombos(fieldData[row, col], 0);

                    foreach (Coord coord in fieldData[row, col].Adjacent)
                    {
                        if (coord.Stats.MineOdds == 1)
                        {
                            field.Flag(coord.Row, coord.Col);                           
                            return true;
                        }

                        if (coord.Stats.ValOdds == 1)
                        {
                            field.Reveal(coord.Row, coord.Col);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void BestGuess()
        {
            double maxMine = 0;
            double maxVal = 0;

            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    if (fieldData[row, col].Value != Field.Hidden) continue;

                    ComboStats stats = fieldData[row, col].Stats;

                    maxMine = Math.Max(maxMine, stats.MineOdds);
                    maxVal = Math.Max(maxVal, stats.ValOdds);
                }
            }

            double maxOdds = Math.Max(maxMine, maxVal);

            for (int row = 0; row < field.Height; row++)
            {
                for (int col = 0; col < field.Width; col++)
                {
                    if (fieldData[row, col].Value != Field.Hidden) continue;

                    ComboStats stats = fieldData[row, col].Stats;                   

                    if (stats.MineOdds >= maxOdds)
                    {
                        field.Flag(row, col);
                        return;
                    }

                    if (stats.ValOdds >= maxOdds)
                    {
                        field.Reveal(row, col);
                        return;
                    }
                }
            }
        }

        private bool GenCombos(Coord origin, int depth)
        {
            if (origin.Value < 1) return true;

            if (origin.IsValid() == false) return false;

            foreach(Coord coord in origin.Adjacent)
            {
                if (coord.Value == 0) continue;

                if (coord.IsValid() == false) return false;
            }

            if (origin.NumHidden == 0) return true;

            if (depth > MaxDepth) return true;

            HashSet<Coord> affected = new();

            foreach(var coord in origin.Adjacent)
            {
                if (coord.Value != Field.Hidden) continue;

                affected.UnionWith(coord.Adjacent);
            }

            affected.ExceptWith(origin.Adjacent);

            (var mineCombos, var valCombos) = ComboMgr.Generate(origin);

            bool hasValid = false;

            for (int iCombo = 0; iCombo < mineCombos.Count; iCombo++)
            {
                bool isValid = true;

                ComboMgr.Apply(origin, mineCombos[iCombo], valCombos[iCombo]);

                foreach(Coord affCoord in affected)
                {
                    isValid &= GenCombos(affCoord, depth + 1);
                }

                if (isValid)
                {
                    hasValid = true;

                    for (int i = 0; i < origin.NumAdj; i++)
                    {
                        if (mineCombos[iCombo][i]) origin.Adjacent[i].Stats.FlaggedCount++;

                        if (valCombos[iCombo][i]) origin.Adjacent[i].Stats.OpenedCount++;
                    }
                }

                ComboMgr.Remove(origin, mineCombos[iCombo], valCombos[iCombo]);
            }

            return hasValid;            
        }

        private static void Swap<T>(ref T first, ref T second)
        {
            T tmp = first;

            first = second;

            second = tmp;
        }
    }
}
