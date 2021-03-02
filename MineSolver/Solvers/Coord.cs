using System.Collections.Generic;

namespace Minesolver.Solvers
{
    public class Coord
    {
        public List<(int X, int Y)> Neighbors { get; }

        public Coord(int x, int y, BaseField field)
        {
            Neighbors = field.GetNeighbors(x, y);
        }
    }
}
