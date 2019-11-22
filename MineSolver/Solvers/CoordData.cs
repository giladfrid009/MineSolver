using System.Collections.Generic;

namespace Minesolver.Solvers
{
    public class CoordData
    {
        public List<(int X, int Y)> Neighbors { get; private set; }

        public virtual void Initialize(int x, int y, FieldBase field)
        {
            Neighbors = field.GetNeighbors(x, y);
        }
    }
}
