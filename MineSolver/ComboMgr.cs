﻿using System;
using System.Collections.Generic;
using System.Linq;
using Minefield;

namespace Minesolver
{
    internal static class ComboMgr
    {
        private static readonly bool[][][][] AllCombos;

        private const int MaxAdj = 8;

        static ComboMgr()
        {
            AllCombos = new bool[MaxAdj + 1][][][];

            for (int nAdj = 0; nAdj <= MaxAdj; nAdj++)
            {
                AllCombos[nAdj] = new bool[nAdj + 1][][];

                for (int nFlags = 0; nFlags <= nAdj; nFlags++)
                {
                    AllCombos[nAdj][nFlags] = GenCombos(0, nFlags, new bool[nAdj]).ToArray();
                }
            }
        }

        private static List<bool[]> GenCombos(int index, int nTrue, bool[] current)
        {
            if (nTrue <= 0) return new List<bool[]> { current.ToArray() };

            if (index > current.Length - 1) return new List<bool[]>();

            List<bool[]> combos = new();

            current[index] = false;

            combos.AddRange(GenCombos(index + 1, nTrue, current));

            current[index] = true;

            combos.AddRange(GenCombos(index + 1, nTrue - 1, current));

            current[index] = false;

            return combos;
        }

        private static bool[][] GetMines(int numAdj, int numFlags)
        {
            if (numAdj > MaxAdj || numAdj < 0) throw new ArgumentOutOfRangeException(nameof(numAdj));

            if (numFlags > numAdj || numFlags < 0) throw new ArgumentOutOfRangeException(nameof(numFlags));

            return AllCombos[numAdj][numFlags];
        }

        private static bool IsValid(Coord origin, bool[] mineCombo)
        {
            Coord[] adjCoords = origin.Adjacent;

            if (mineCombo.Length != adjCoords.Length) return false;

            for (int i = 0; i < adjCoords.Length; i++)
            {
                if (mineCombo[i] && adjCoords[i].Value != Field.Hidden) return false;
            }

            return true;
        }

        public static (List<bool[]> MineCombos, List<bool[]> ValCombos) Generate(Coord coord)
        {
            if (coord.Value < 0) return (new List<bool[]>(), new List<bool[]>());

            bool[][] allCombos = GetMines(coord.NumAdj, coord.Value - coord.NumFlags);

            List<bool[]> mineCombos = new();
            List<bool[]> valCombos = new();

            foreach (bool[] mCombo in allCombos)
            {
                if (IsValid(coord, mCombo) == false) continue;

                mineCombos.Add(mCombo);

                bool[] vCombo = new bool[mCombo.Length];

                for (int j = 0; j < mCombo.Length; j++)
                {
                    if (coord.Adjacent[j].Value == Field.Hidden && mCombo[j] == false)
                    {
                        vCombo[j] = true;
                    }
                }

                valCombos.Add(vCombo);
            }

            return (mineCombos, valCombos);
        }

        public static void Apply(Coord coord, bool[] mineCombo, bool[] valCombo)
        {
            Coord[] adjCoords = coord.Adjacent;

            if (adjCoords.Length != mineCombo.Length) throw new ArgumentOutOfRangeException(nameof(mineCombo));

            if (adjCoords.Length != valCombo.Length) throw new ArgumentOutOfRangeException(nameof(valCombo));

            for (int i = 0; i < adjCoords.Length; i++)
            {
                if (mineCombo[i]) adjCoords[i].Value = Field.Mine;

                else if (valCombo[i]) adjCoords[i].Value = 0;
            }
        }

        public static void Remove(Coord coord, bool[] mineCombo, bool[] valCombo)
        {
            Coord[] adjCoords = coord.Adjacent;

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
