using System;
using System.Collections.Generic;
using Minefield;

namespace Minesolver
{
    public class Solver
    {
        private readonly Field Field;
        private readonly CoordsData Coords;

        public int MaxDepth { get; set; } = 10;

        public Solver(Field field)
        {
            Field = field;
            Coords = new CoordsData(field);
        }
       
        public void Solve()
        {
            HashSet<Coord> hidden = new();
            HashSet<Coord> trySolve = new();

            CreateSets(hidden, trySolve);

            Field.OnMove += UpdateSets;

            while (Field.Game.Result == GameResult.None && Field.Game.NumHidden != 0)
            {
                BasicLogic(trySolve);

                bool madeMove = RegenCombos(hidden, trySolve);

                if (madeMove) continue;

                Guess(hidden);
            }

            Field.OnMove -= UpdateSets;

            void UpdateSets(Field sender, MoveArgs e)
            {
                Coord coord = Coords[e.Row, e.Col];                

                if(e.Move == Move.Unflag) hidden.Add(coord);

                else hidden.Remove(coord);

                if (coord.Value > 0)
                {
                    if (coord.NumHidden != 0) trySolve.Add(coord);

                    else trySolve.Remove(coord);
                }

                foreach (Coord adjCoord in coord.Adjacent)
                {
                    if (adjCoord.Value > 0)
                    {
                        if (adjCoord.NumHidden != 0) trySolve.Add(adjCoord);

                        else trySolve.Remove(adjCoord);
                    }
                }
            }
        }

        private void CreateSets(HashSet<Coord> hidden, HashSet<Coord> trySolve)
        {
            for (int row = 0; row < Coords.Height; row++)
            {
                for (int col = 0; col < Coords.Width; col++)
                {
                    Coord coord = Coords[row, col];

                    if (coord.Value == Field.Hidden)
                    {
                        hidden.Add(coord);
                    }
                    else if (coord.Value > 0 && coord.NumHidden != 0)
                    {
                        trySolve.Add(coord);
                    }
                }
            }
        }

        private void BasicLogic(HashSet<Coord> trySolve)
        {
            if (Field.Game.Result != GameResult.None) return;

            for (int row = 0; row < Field.Height; row++)
            {
                for (int col = 0; col < Field.Width; col++)
                {
                    if (trySolve.Contains(Coords[row, col]))
                    {
                        TrySolve(Coords[row, col]);
                    }
                }
            }
        }

        private void TrySolve(Coord origin)
        {
            if (origin.Value < 1 || origin.NumHidden == 0) return;

            HashSet<Coord> oldAffected = new() { origin };
            HashSet<Coord> newAffected = new();

            Field.OnMove += UpdateAffected;

            while (oldAffected.Count != 0 && Field.Game.Result == GameResult.None)
            {
                foreach (Coord coord in oldAffected)
                {
                    if (coord.Value < 1 || coord.NumHidden == 0) continue;

                    if (coord.NumFlags == coord.Value)
                    {
                        foreach (Coord adjCoord in coord.Adjacent)
                        {
                            if (adjCoord.Value != Field.Hidden) continue;

                            Field.Reveal(adjCoord.Row, adjCoord.Col);
                        }
                    }
                    else if (coord.NumHidden == coord.Value - coord.NumFlags)
                    {
                        foreach (Coord adjCoord in coord.Adjacent)
                        {
                            if (adjCoord.Value != Field.Hidden) continue;

                            Field.Flag(adjCoord.Row, adjCoord.Col);
                        }
                    }
                }

                Swap(ref oldAffected, ref newAffected);

                newAffected.Clear();
            }

            Field.OnMove -= UpdateAffected;

            void UpdateAffected(Field sender, MoveArgs e)
            {
                Coord coord = Coords[e.Row, e.Col];

                if (coord.Value > 0 && coord.NumHidden != 0) newAffected.Add(coord);

                foreach (Coord adjCoord in coord.Adjacent)
                {
                    if (adjCoord.Value > 0)
                    {
                        if (adjCoord.NumHidden != 0) newAffected.Add(adjCoord);

                        else newAffected.Remove(adjCoord);
                    }
                }
            }
        }

        private static void Swap<T>(ref T first, ref T second)
        {
            T tmp = first;
            first = second;
            second = tmp;
        }

        private bool RegenCombos(HashSet<Coord> hidden, HashSet<Coord> trySolve)
        {
            if (Field.Game.Result != GameResult.None) return false;

            foreach (Coord coord in hidden)
            {
                coord.Stats.Clear();
            }

            foreach(Coord coord in trySolve)
            {
                GenCombos(coord, 0);

                foreach (Coord adjCoord in coord.Adjacent)
                {
                    if (adjCoord.Value != Field.Hidden) continue;

                    if (adjCoord.Stats.MineOdds == 1)
                    {
                        Field.Flag(adjCoord.Row, adjCoord.Col);
                        return true;
                    }

                    if (adjCoord.Stats.ValueOdds == 1)
                    {
                        Field.Reveal(adjCoord.Row, adjCoord.Col);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool GenCombos(Coord origin, int depth)
        {
            if (origin.Value < 1) return true;

            if (origin.IsValid() == false) return false;

            if (origin.NumHidden == 0) return true;

            if (depth > MaxDepth) return true;

            HashSet<Coord> affected = new();

            foreach(Coord coord in origin.Adjacent)
            {
                if (coord.Value != Field.Hidden) continue;

                foreach (Coord adjCoord in coord.Adjacent)
                {
                    if (adjCoord.Value > 0) affected.Add(adjCoord);
                }
            }

            affected.Remove(origin);
            affected.ExceptWith(origin.Adjacent);

            (var mineCombos, var valCombos) = ComboMgr.Generate(origin);

            bool hasValid = false;

            for (int iCombo = 0; iCombo < mineCombos.Count; iCombo++)
            {
                ComboMgr.Apply(origin, mineCombos[iCombo], valCombos[iCombo]);

                bool isValid = true;

                foreach (Coord coord in origin.Adjacent)
                {
                    if (isValid == false) break;

                    if (coord.Value < 1) continue;

                    if (coord.IsValid() == false) isValid = false;
                }

                foreach (Coord coord in affected)
                {
                    if (isValid == false) break;

                    isValid = GenCombos(coord, depth + 1);
                }

                if (isValid)
                {
                    hasValid = true;

                    for (int i = 0; i < origin.NumAdj; i++)
                    {
                        if (mineCombos[iCombo][i])
                        {
                            origin.Adjacent[i].Stats.FlaggedCount++;
                        }

                        else if (valCombos[iCombo][i])
                        {
                            origin.Adjacent[i].Stats.OpenedCount++;
                        }
                    }
                }

                ComboMgr.Remove(origin, mineCombos[iCombo], valCombos[iCombo]);
            }

            return hasValid;            
        }

        private void Guess(HashSet<Coord> hidden)
        {
            if (Field.Game.Result != GameResult.None) return;

            double maxMine = 0;
            double maxValue = 0;

            foreach (Coord coord in hidden)
            {
                maxMine = Math.Max(maxMine, coord.Stats.MineOdds);

                maxValue = Math.Max(maxValue, coord.Stats.ValueOdds);
            }

            double maxOdds = Math.Max(maxMine, maxValue);

            foreach (Coord coord in hidden)
            {
                if (coord.Stats.MineOdds >= maxOdds && Field.Game.NumFlags < Field.Game.TotalMines)
                {
                    Field.Flag(coord.Row, coord.Col);
                    return;
                }

                if (coord.Stats.ValueOdds >= maxOdds)
                {
                    Field.Reveal(coord.Row, coord.Col);
                    return;
                }
            }
        }        
    }
}
