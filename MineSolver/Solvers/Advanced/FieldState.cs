namespace Minesolver.Solvers.Advanced
{
    public class FieldState
    {
        public int Width { get; }
        public int Height { get; }

        private readonly int[,] fieldState;

        public int this[int x, int y] => fieldState[x, y];

        public FieldState(BaseField field)
        {
            Width = field.Width;
            Height = field.Height;

            fieldState = new int[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    fieldState[x, y] = field[x, y];
                }
            }
        }
    }
}
