using System.Collections.Generic;

namespace Minesolver.Solvers
{
    public class CoordData
    {
        public List<(int X, int Y)> Neighbors { get; }

        public CoordData(int x, int y, FieldBase field)
        {
            Neighbors = field.GetNeighbors(x, y);
        }
    }
}
