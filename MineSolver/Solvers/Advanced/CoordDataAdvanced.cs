using Minesolver.Solvers.Basic;

namespace Minesolver.Solvers.Advanced
{
    public class CoordDataAdvanced : CoordData
    {
        public bool TryAdvanced = true;
        public bool UsedInCombo = false;
        public uint TotalFlagged = 0;
        public uint TotalCombos = 0;
    }
}
