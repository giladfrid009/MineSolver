using System.Collections.Generic;
using System.Linq;

namespace Minesolver.Solvers.Utils.Advanced
{
    public class ComboLibrary
    {
        private Combo[][][] combos;

        public ComboLibrary()
        {
            GenerateComboLibrary();
        }

        public Combo[] this[int nHidden, int nMines]
        {
            get => combos[nHidden][nMines];
        }

        private void GenerateComboLibrary()
        {
            combos = new Combo[9][][];

            for (int nHidden = 0; nHidden <= 8; nHidden++)
            {
                combos[nHidden] = new Combo[nHidden + 1][];

                var combosOfLength = GenerateCombosOfLength(nHidden, 0, new bool[nHidden]);

                for (int nMines = 0; nMines <= nHidden; nMines++)
                {
                    combos[nHidden][nMines] = combosOfLength.Where(C => C.NumMines == nMines).ToArray();
                }
            }
        }

        private IEnumerable<Combo> GenerateCombosOfLength(int length, int index, bool[] combo)
        {
            if (index == length)
            {
                yield return new Combo(combo);
            }
            else
            {
                combo[index] = false;

                foreach (var comboFull in GenerateCombosOfLength(length, index + 1, combo))
                {
                    yield return comboFull;
                }

                combo[index] = true;

                foreach (var comboFull in GenerateCombosOfLength(length, index + 1, combo))
                {
                    yield return comboFull;
                }
            }
        }
    }

}
