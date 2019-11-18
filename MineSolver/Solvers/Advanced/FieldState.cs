namespace Minesolver.Solvers.Advanced
{
    public class FieldState
    {
        private readonly int[,] fieldState;

        public int this[int x, int y] { get => fieldState[x, y]; }

        public FieldState(FieldBase field)
        {
            int width = field.Width;
            int height = field.Height;

            fieldState = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldState[x, y] = field[x, y];
                }
            }
        }
    }
}
