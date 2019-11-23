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

        public CoordDataAdvanced(int x, int y, FieldBase field) : base(x, y, field)
        {

        }

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
