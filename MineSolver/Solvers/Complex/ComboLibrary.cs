using System.Collections.Generic;
using System.Linq;

namespace MineSolver.Solvers.Complex
{
    public class ComboLibrary
    {
        private Combo[][][] combos;

        public ComboLibrary()
        {
            GenerateComboLibrary();
        }

        public Combo[] this[int nHidden, int nFlags]
        {
            get => combos[nHidden][nFlags];
        }

        private void GenerateComboLibrary()
        {
            combos = new Combo[9][][];

            for (int nHidden = 0; nHidden <= 8; nHidden++)
            {
                combos[nHidden] = new Combo[nHidden + 1][];

                var combosOfLength = GenerateCombosOfLength(nHidden, 0, new bool[nHidden]);

                for (int nFlags = 0; nFlags <= nHidden; nFlags++)
                {
                    combos[nHidden][nFlags] = combosOfLength.Where(C => C.NumFlags == nFlags).ToArray();
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
