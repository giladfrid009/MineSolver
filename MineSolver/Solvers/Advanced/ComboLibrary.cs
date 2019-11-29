using System.Collections.Generic;
using System.Linq;

namespace Minesolver.Solvers.Advanced
{
    public class ComboLibrary
    {
        private static readonly Combo[][][] combos;

        static ComboLibrary()
        {
            combos = new Combo[9][][];
            GenerateComboLibrary();
        }

        public Combo[] this[int nHidden, int nMines] => combos[nHidden][nMines];

        private static void GenerateComboLibrary()
        {
            for (int nHidden = 0; nHidden <= 8; nHidden++)
            {
                combos[nHidden] = new Combo[nHidden + 1][];

                IEnumerable<Combo> combosOfLength = GenerateCombosOfLength(nHidden, 0, new bool[nHidden]);

                for (int nMines = 0; nMines <= nHidden; nMines++)
                {
                    combos[nHidden][nMines] = combosOfLength.Where(C => C.NumMines == nMines).ToArray();
                }
            }
        }

        private static IEnumerable<Combo> GenerateCombosOfLength(int length, int index, bool[] combo)
        {
            if (index == length)
            {
                yield return new Combo(combo);
            }
            else
            {
                combo[index] = false;

                foreach (Combo comboFull in GenerateCombosOfLength(length, index + 1, combo))
                {
                    yield return comboFull;
                }

                combo[index] = true;

                foreach (Combo comboFull in GenerateCombosOfLength(length, index + 1, combo))
                {
                    yield return comboFull;
                }
            }
        }
    }

}
