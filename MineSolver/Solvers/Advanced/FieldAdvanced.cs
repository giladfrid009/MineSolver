﻿namespace Minesolver.Solvers.Advanced
{
    public class FieldAdvanced : Field<CoordAdvanced>
    {
        public FieldAdvanced(BaseField field) : base(field)
        {

        }

        public override int NumHidden(int x, int y)
        {
            int num = 0;

            foreach ((int x2, int y2) in coordsData[x, y].Neighbors)
            {
                if (coordsData[x2, y2].IsForced == false && IsRevealed(x2, y2) == false)
                {
                    num++;
                }
            }

            return num;
        }

        public override int NumMines(int x, int y)
        {
            int num = 0;

            foreach ((int x2, int y2) in coordsData[x, y].Neighbors)
            {
                if (IsFlagged(x2, y2) || coordsData[x2, y2].ForceFlag)
                {
                    num++;
                }
            }

            return num;
        }
    }
}