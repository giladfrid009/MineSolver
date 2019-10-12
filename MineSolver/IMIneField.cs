namespace MineSolver
{
    public interface IMIneField 
    {
        
        int Width { get; }
        int Height { get; }
        int ValMine { get; }
        int ValHidden { get; }

        int this[int x, int y] { get; }

        int this[(int x, int y) coord] { get; }

        void Flag(int x, int y);

        void Unflag(int x, int y);

        bool Reveal(int x, int y);

        IMIneField Copy();
    }
}
