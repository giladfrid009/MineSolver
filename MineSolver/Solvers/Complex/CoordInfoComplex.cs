namespace MineSolver.Solvers.Complex
{
    public class CoordInfoComplex : CoordInfo
    {
        public bool TryComplex
        {
            get => tryComplexVar && IsValue && (IsSolved == false);
            set => tryComplexVar = value;
        }

        public bool UsedInCombo { get; set; } = false;

        private bool tryComplexVar = true;
    }
}
