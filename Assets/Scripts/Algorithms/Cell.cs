using System;

namespace Assets.Scripts.Auxilliary
{
    public class Cell
    {
        public int Y { get; private set; }
        public int X { get; private set; }
        public int G { get; set; }
        public int H { get; set; }
        public Cell Parent { get; set; }
        public Cell Child { get; set; }
        public bool IsStart { get; set; } = false;

        public Cell() { }

        public Cell(int y, int x, (int, int) endPoint, Cell parent = null)
        {
            Y = y;
            X = x;
            if (parent is not null)
            {
                Parent = parent;
                H = (Math.Abs(endPoint.Item1 - y) + Math.Abs(endPoint.Item2 - x)) * 10;
                if (Math.Abs(parent.X - this.X) + Math.Abs(parent.Y - this.Y) == 2) G = parent.G + 14;
                else G = parent.G + 10;
            }
            else IsStart = true;
        }
    }
}
