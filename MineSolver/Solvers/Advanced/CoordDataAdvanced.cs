using Minesolver.Solvers.Basic;

namespace Minesolver.Solvers.Advanced
{
    public class CoordDataAdvanced : CoordData
    {
        public bool ForceFlag = false;
        public bool ForceReveal = false;
        public bool TryAdvanced = true;
        public uint TotalFlagged = 0;
        public uint TotalCombos = 0;

        public bool IsForced => ForceFlag || ForceReveal;
        public override bool IsFlagged => ForceFlag || base.IsFlagged;
        public override bool IsRevealed => ForceReveal || base.IsRevealed;
        public override bool IsValue => !ForceReveal && base.IsValue;

        public virtual void Reset()
        {
            ForceFlag = false;
            ForceReveal = false;
            TryAdvanced = true;
            TotalFlagged = 0;
            TotalCombos = 0;
        }
    }
}
