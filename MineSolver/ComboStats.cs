namespace Minesolver
{
    internal class ComboStats
    {
        public int FlaggedCount { get; set; }
        public int OpenedCount { get; set; }

        public int Total => FlaggedCount + OpenedCount;
        public double MineOdds => Total != 0 ? (double)FlaggedCount / Total : 0;
        public double ValOdds => Total != 0 ? (double)OpenedCount / Total : 0;

        public void Reset()
        {
            FlaggedCount = 0;
            OpenedCount = 0;
        }
    }
}
