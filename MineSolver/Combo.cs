namespace Minesolver
{
    internal class Combo
    {
        public int NumFlagged { get; set; }
        public int NumOpened { get; set; }

        public double MinePrec => (NumFlagged + NumOpened) != 0 ? (double)NumFlagged / (NumFlagged + NumOpened) : 0;
        public double ValuePrec => (NumFlagged + NumOpened) != 0 ? (double)NumOpened / (NumFlagged + NumOpened) : 0;

        public void Reset()
        {
            NumFlagged = 0;
            NumOpened = 0;
        }
    }
}
